using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameMain : MonoBehaviour
    {
        [SerializeField] private Root _root;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _keyPrefab;
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private Transform _topLeft;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Color[] _levelColors;

        const int GridSize = 5;

        List<string[]> _levels = new();
        Vector2Int _playerPos = new (0, 0);

        (int, char)[,] grid = new (int, char)[GridSize, GridSize];
        GameObject[,] _tileGos = new GameObject[GridSize, GridSize];

        public void Setup()
        {
            _levels.Add(new string[]
            {
                "....K",
                ".....",
                ".....",
                ".....",
                "....D",
            });
            _levels.Add(new string[]
            {
                ".....",
                ".....",
                ".K.D.",
                ".....",
                ".....",
            });
            _levels.Add(new string[]
            {
                "..K..",
                ".....",
                ".....",
                ".....",
                "..D..",
            });
        }

        public void ResetGame()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    grid[i, j] = (0, '.');

                    GameObject tileGo = Instantiate(_tilePrefab, _tilesParent);
                    tileGo.transform.position = new Vector3(i, j, 0);
                    tileGo.GetComponent<SpriteRenderer>().material.color = _levelColors[0];
                    _tileGos[i, j] = tileGo;
                }
            }
            _playerTransform.position = new Vector3(_playerPos.x, _playerPos.y, -0.1f);

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
                return;
            }

            int newDepth = grid[_playerPos.x, _playerPos.y].Item1 + 1;
            grid[_playerPos.x, _playerPos.y].Item1 = newDepth;

            GameObject go = _tileGos[_playerPos.x, _playerPos.y];
            int newColorIndex = newDepth;
            if (newColorIndex < 0) newColorIndex = _levelColors.Length - 1;
            go.GetComponent<SpriteRenderer>().material.color = _levelColors[newColorIndex];
            _playerPos = newPos;
            _playerTransform.position += new Vector3(delta.x, delta.y);

        }
    }
}