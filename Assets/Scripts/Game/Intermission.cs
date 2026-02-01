using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Intermission : MonoBehaviour
    {
        [SerializeField] private Root _root;
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _otherText;

        public void Setup()
        {
            _playButton.onClick.AddListener(OnClickedPlayButton);
        }

        public void ResetIntermission()
        {
            _titleText.text = $"YOU PASSED THROUGH\nFIVE ROOMS\nWITH {_root.MoveCount} STEPS";

            const int OurBest = 72;
            if (_root.MoveCount > OurBest)
            {
                _otherText.text = "BETTER IS POSSIBLE";
            }
            else if (_root.MoveCount == OurBest)
            {
                _otherText.text = "DECENT";
            }
            else
            {
                _otherText.text = "IMPRESSIVE\nBETTER THAN US";
            }
        }

        public void OnClickedPlayButton()
        {
            if (!_playButton.interactable) return;

            _playButton.interactable = false;
            _root.OnIntermissionClickedPlay();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) OnClickedPlayButton();
        }

    }
}