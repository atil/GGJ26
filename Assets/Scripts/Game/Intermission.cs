using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Intermission : MonoBehaviour
    {
        [SerializeField] private Root _root;
        [SerializeField] private Button _playButton;

        public void Setup()
        {
            _playButton.onClick.AddListener(OnClickedPlayButton);
        }

        public void ResetIntermission()
        {

        }

        public void OnClickedPlayButton()
        {
            _playButton.interactable = false;
            _root.OnIntermissionClickedPlay();
        }

    }
}