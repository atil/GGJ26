// TODO
// - sfx
// - build

using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private Transform _cameraStart;
        [SerializeField] private Transform _cameraLook;
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
        [SerializeField] private TextMeshProUGUI _stepsText;
        [SerializeField] private Color[] _levelColors;
        [SerializeField] private AnimationCurve _playerMoveCurve;
        [SerializeField] private AnimationCurve _goDestroyCurve;
        [SerializeField] private AnimationCurve _cameraIntroCurve;

        const int GridSize = 5;

        List<char[,]> _levels = new();
        Vector2Int _playerPos = new(0, 0);

        GridCell[,,] _grid;
        Item _heldKey = null;
        const float ItemDepthOffset = 0.1f;
        const float PlayerDepthOffset = 0.05f;

        const int NumDoors = 5;
        int _openedDoors = 0;
        bool _stuck = false;

        public void Setup() { }

        private void Cleanup()
        {
            _levels.Clear();
            if (_heldKey != null) Destroy(_heldKey.Go);
            _heldKey = null;
            _grid = null;
            _playerPos = new(0, 0);
            _openedDoors = 0;
            _root.MoveCount = 0;
            _stepsText.text = "";
            _stuck = false;

            foreach (Transform t in _tilesParent) Destroy(t.gameObject);
            foreach (Transform t in _itemsParent) Destroy(t.gameObject);
        }

        public void ResetGame()
        {
            _resetButton.gameObject.SetActive(true);
            Cursor.visible = false;
            Cleanup();

            _resetButton.interactable = true;
            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', '.', 'K' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', 'W', 'W', 'W' },
                { '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', 'D' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', '.', '.' },
                { '.', '.', 'W', '.', '.' },
                { '.', 'W', '.', 'D', '.' },
                { '.', 'W', 'W', '.', '.' },
                { 'K', '.', 'W', 'W', '.' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', 'D', '.', '.' },
                { '.', 'W', 'W', '.', '.' },
                { '.', '.', 'W', '.', 'W' },
                { '.', '.', 'W', '.', 'W' },
                { '.', '.', 'K', '.', '.' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { 'K', '.', '.', '.', 'W' },
                { '.', 'W', '.', 'W', '.' },
                { '.', '.', 'W', '.', '.' },
                { '.', 'W', 'W', '.', '.' },
                { '.', '.', '.', '.', 'D' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { '.', '.', '.', 'W', '.' },
                { '.', 'D', 'W', 'W', '.' },
                { 'W', '.', '.', '.', '.' },
                { '.', '.', 'W', 'K', '.' },
                { '.', '.', '.', 'W', '.' },
            });

            _levels.Add(new char[GridSize, GridSize]
            {
                { 'W', 'W', 'W', 'W', 'W' },
                { 'W', 'D', 'W', 'W', 'W' },
                { 'W', 'W', 'W', 'W', 'W' },
                { 'W', 'W', 'W', 'W', 'W' },
                { 'W', 'W', 'W', 'W', 'W' },
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

            _openedDoors = 0;

            PlayCameraIntro();
        }

        void PlayCameraIntro()
        {
            const float CameraIntroDuration = 2.3f;
            Transform camTr = _root.Camera.transform;
            Vector3 camDefaultPos = camTr.position;
            camTr.position = _cameraStart.position;
            camTr.forward = _cameraLook.position - _cameraStart.position;

            _jamkit.Tween(new TweenMove(camTr, camDefaultPos, CameraIntroDuration, _cameraIntroCurve));
            _jamkit.Tween(new TweenRotate(camTr, Vector3.zero, CameraIntroDuration, _cameraIntroCurve));
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                OnGameDone();
            }
#endif

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnResetClicked();
            }

            if (_stuck) return;

            Vector2Int delta = Vector2Int.zero;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) delta.y += 1;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) delta.y -= 1;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) delta.x -= 1;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) delta.x += 1;

            if (delta == Vector2Int.zero) return;

            void PlayNopeEffect(Vector2Int nopeCell)
            {
                const float NopeDuration = 0.05f;
                Vector3 src = new Vector3(_playerPos.x, _playerPos.y, -PlayerDepthOffset);
                Vector3 nopeGoPos = new Vector3(nopeCell.x, nopeCell.y, -PlayerDepthOffset);
                Vector3 dir = (nopeGoPos - src).normalized;
                Vector3 tweenTarget = src + dir * 0.3f;
                _jamkit.TweenSeq(new TweenBase[]
                {
                    new TweenMove(_playerTransform, tweenTarget, NopeDuration, _playerMoveCurve),
                    new TweenMove(_playerTransform, src, NopeDuration, _playerMoveCurve),
                });
            }

            Vector2Int newPos = _playerPos + delta;

            if (newPos.x < 0 || newPos.x >= GridSize || newPos.y < 0 || newPos.y >= GridSize) // Grid edges
            {
                PlayNopeEffect(newPos);
                return; 
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
                PlayNopeEffect(newPos);
                return;
            }
            if (newCell.Item.Type == 'D' && (_heldKey == null || _heldKey.Level != newCell.Item.Level))
            {
                PlayNopeEffect(newPos);
                return; // Locked door
            }

            //
            // Move occurred
            //

            GridCell prevCell = GetTopCellAt(_playerPos.x, _playerPos.y);
            DestroyGo(prevCell.TileGo);
            prevCell.IsDestroyed = true;

            if (newCell.Item.Type == 'K') // Grab key
            {
                const float KeyTweenDuration = 0.5f;
                if (_heldKey != null) // Leave the held key on the revealed cell
                {
                    GridCell revealedCell = GetTopCellAt(_playerPos.x, _playerPos.y);
                    if (revealedCell.Item.Type != '.') // oops
                    {
                        _stuck = true;
                        _jamkit.TweenSeq(new TweenBase[]
                        {
                            new TweenDelay(2.0f),
                            new TweenCallback(() => OnResetClicked())
                        });
                    }

                    revealedCell.Item = _heldKey;
                    Vector3 tweenTarget = new(revealedCell.Coord.y, revealedCell.Coord.z, revealedCell.Coord.x - ItemDepthOffset);
                    _jamkit.Tween(new TweenMove(_heldKey.Go.transform, tweenTarget, KeyTweenDuration, AnimationCurve.EaseInOut(0, 0, 1, 1)));
                }

                _heldKey = newCell.Item;
                newCell.Item = null;
                _jamkit.Tween(new TweenMove(_heldKey.Go.transform, _heldKeySlot.position, KeyTweenDuration, AnimationCurve.EaseInOut(0, 0, 1, 1)));
            }
            else if (newCell.Item.Type == 'D') // Go through the door
            {
                int doorLevel = newCell.Item.Level;

                Debug.Assert(_heldKey != null && _heldKey.Level == newCell.Item.Level);
                DestroyGo(newCell.Item.Go);
                newCell.Item = null;
                DestroyGo(_heldKey.Go);
                _heldKey = null;

                // Destroy walls on this level
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        GridCell cell = _grid[doorLevel, i, j];
                        if (cell.Item != null && cell.Item.Type == 'W')
                        {
                            DestroyGo(cell.Item.Go);
                            cell.Item = new Item('.', doorLevel, null);
                        }
                    }
                }

                // Last door is opened
                _openedDoors++;
                if (_openedDoors >= NumDoors)
                {
                    OnGameDone();
                }

            }

            _playerPos = newPos;

            const float PlayerMoveTweenDuration = 0.3f;
            Vector3 newPlayerGoPos = new(_playerPos.x, _playerPos.y, -PlayerDepthOffset);
            _jamkit.Tween(new TweenMove(_playerTransform, newPlayerGoPos, PlayerMoveTweenDuration, _playerMoveCurve));

            _root.MoveCount++;
            if (_openedDoors < NumDoors) _stepsText.text = $"[{_root.MoveCount}]";
        }

        void DestroyGo(GameObject go)
        {
            const float DestroyEffectDuration = 0.2f;
            _jamkit.TweenSeq(new TweenBase[]
            {
                new TweenScale(go.transform, Vector3.one * 0.01f, DestroyEffectDuration, _goDestroyCurve),
                new TweenCallback(() => Destroy(go))
            });
        }

        void OnGameDone()
        {
            Cursor.visible = true;
            _resetButton.gameObject.SetActive(false);
            _stepsText.text = "";
            _root.OnGameDone();
        }

        public void OnResetClicked()
        {
            _stuck = true;
            _root.OnSplashClickedPlay();
            _resetButton.interactable = false;
        }
    }
}