using UnityEngine;

namespace Game
{
    public class GameMain : MonoBehaviour
    {
        [SerializeField] private Root _root;

        public void Setup()
        {
        }

        public void ResetGame()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                _root.OnGameDone();
            }
        }
    }
}