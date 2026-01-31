using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit { }
}

namespace Game
{
    public record Item(char Type, int Level, GameObject Go);

    public class GridCell
    {
        public Vector3Int Coord;
        public Item Item;
        public GameObject TileGo;
        public GameObject ItemGo;
        public bool IsDestroyed;
    }

    public class GameMain : MonoBehaviour
    {
        [SerializeField] private Root _root;
        [SerializeField] private JamKit _jamkit;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _keyPrefab;
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private Transform _heldKeySlot;
        [SerializeField] private Transform _topLeft;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Color[] _levelColors;

        const int GridSize = 5;

        List<char[,]> _levels = new();
        Vector2Int _playerPos = new(0, 0);

        GridCell[,,] _grid;
        Item _heldKey = null;
        const float ItemDepthOffset = 0.1f;

        public void Setup() { }

        private void Cleanup()
        {
            _levels.Clear();
            if (_heldKey != null) Destroy(_heldKey.Go);
            _heldKey = null;
            _grid = null;
            _playerPos = new(0, 0);

            foreach (Transform t in _tilesParent) Destroy(t.gameObject);
            foreach (Transform t in _itemsParent) Destroy(t.gameObject);
        }

        public void ResetGame()
        {
            Cleanup();

            _resetButton.interactable = true;
            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', '.', 'K' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', 'W', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', 'D' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', 'K', '.', 'D', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', 'D', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', 'K', '.', '.' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { 'K', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', 'D' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', '.', '.' },
                { '.', 'D', '.', '.', '.' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', 'K', '.' },
                { '.', '.', '.', '.', '.' },
            });

            _grid = new GridCell[_levels.Count, GridSize, GridSize];

            for (int level = 0; level < _levels.Count; level++)
            {
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        char ch = _levels[level][i, j];

                        GameObject tileGo = Instantiate(_tilePrefab, _tilesParent);
                        tileGo.transform.position = new Vector3(i, j, level);
                        tileGo.GetComponent<SpriteRenderer>().material.color = _levelColors[level];

                        GameObject itemGo = null;
                        GameObject itemPrefab = null;
                        if (ch == 'K') itemPrefab = _keyPrefab;
                        if (ch == 'D') itemPrefab = _doorPrefab;
                        if (ch == 'W') itemPrefab = _wallPrefab;

                        if (itemPrefab != null)
                        {
                            itemGo = Instantiate(itemPrefab, new Vector3(i, j, level - ItemDepthOffset), Quaternion.identity);
                            itemGo.transform.SetParent(_itemsParent);
                            itemGo.GetComponent<SpriteRenderer>().material.color = _levelColors[level];
                        }

                        _grid[level, i, j] = new GridCell() { Coord = new(level, i, j), TileGo = tileGo, ItemGo = itemGo, Item = new(ch, level, itemGo), IsDestroyed = false };

                    }
                }
            }
            _playerTransform.position = new Vector3(_playerPos.x, _playerPos.y, -0.2f);

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                _root.OnGameDone();
            }

            Vector2Int delta = Vector2Int.zero;
            if (Input.GetKeyDown(KeyCode.W)) delta.y += 1;
            if (Input.GetKeyDown(KeyCode.S)) delta.y -= 1;
            if (Input.GetKeyDown(KeyCode.A)) delta.x -= 1;
            if (Input.GetKeyDown(KeyCode.D)) delta.x += 1;

            if (delta == Vector2Int.zero) return;

            Vector2Int newPos = _playerPos + delta;

            if (newPos.x < 0 || newPos.x >= GridSize || newPos.y < 0 || newPos.y >= GridSize)
            {
                return; // Grid edges
            }

            GridCell GetTopCellAt(int i, int j)
            {
                for (int level = 0; level < _levels.Count; level++)
                {
                    GridCell cell = _grid[level, i, j];
                    if (!cell.IsDestroyed)
                    {
                        return cell;
                    }
                }
                return null;
            }

            GridCell newCell = GetTopCellAt(newPos.x, newPos.y);
            if (newCell.Item.Type == 'W')
            {
                return;
            }
            if (newCell.Item.Type == 'D' && (_heldKey == null || _heldKey.Level != newCell.Item.Level))
            {
                return; // Locked door
            }

            //
            // Move occurred
            //

            GridCell prevCell = GetTopCellAt(_playerPos.x, _playerPos.y);
            Destroy(prevCell.TileGo);
            prevCell.IsDestroyed = true;

            if (newCell.Item.Type == 'K') // Grab key
            {
                if (_heldKey != null) // Leave the held key on the revealed cell
                {
                    GridCell revealedCell = GetTopCellAt(_playerPos.x, _playerPos.y);
                    revealedCell.Item = _heldKey;
                    Vector3 tweenTarget = new(revealedCell.Coord.y, revealedCell.Coord.z, revealedCell.Coord.x - ItemDepthOffset);
                    _jamkit.Tween(new TweenMove(_heldKey.Go.transform, tweenTarget, 0.5f, AnimationCurve.EaseInOut(0, 0, 1, 1)));
                }

                _heldKey = newCell.Item;
                newCell.Item = null;
                _jamkit.Tween(new TweenMove(_heldKey.Go.transform, _heldKeySlot.position, 0.5f, AnimationCurve.EaseInOut(0, 0, 1, 1)));
            }
            else if (newCell.Item.Type == 'D') // Go through the door
            {
                int doorLevel = newCell.Item.Level;

                Debug.Assert(_heldKey != null && _heldKey.Level == newCell.Item.Level);
                Destroy(newCell.Item.Go);
                newCell.Item = null;
                Destroy(_heldKey.Go);
                _heldKey = null;

                // destroy walls on this level
                for (int i = 0; i < GridSize; i++) 
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        GridCell cell = _grid[doorLevel, i, j];
                        if (cell.Item != null && cell.Item.Type == 'W')
                        {
                            Destroy(cell.Item.Go);
                            cell.Item = new Item('.', doorLevel, null );
                        }
                    }
                }

            }

            _playerPos = newPos;
            _playerTransform.position += new Vector3(delta.x, delta.y);

        }

        public void OnResetClicked()
        {
            _root.OnSplashClickedPlay();
            _resetButton.interactable = false;
        }
    }
}