using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Splash : MonoBehaviour
    {
        [SerializeField] private Root _root;
        [SerializeField] private Button _playButton;

        public void Setup()
        {
            _playButton.onClick.AddListener(OnClickedPlayButton);
        }

        public void OnClickedPlayButton()
        {
            _playButton.interactable = false;
            _root.OnSplashClickedPlay();
        }
    }
}