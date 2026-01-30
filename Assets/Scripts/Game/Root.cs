using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CrossSceneData
    { 
    }

    public class Root : MonoBehaviour
    {
        [SerializeField] private bool _quickStart = false;

        [SerializeField] private JamKit _jamKit;
        [SerializeField] private Camera _camera;
        [SerializeField] private Image _coverImage;

        [SerializeField] private Splash _splash;
        [SerializeField] private GameMain _gameMain;
        [SerializeField] private Intermission _intermission;

        private void Start()
        {
            _splash.Setup();
            _gameMain.Setup();
            _intermission.Setup();

            if (_quickStart)
            {
                InitGame();
                _jamKit.Tween(new TweenImageColor(_coverImage, Color.clear, 0.3f));
                return;
            }

            _camera.backgroundColor = _jamKit.Globals.SplashSceneCameraBackgroundColor;
            _jamKit.Tween(new TweenImageColor(_coverImage, Color.clear, _jamKit.Globals.SceneTransitionParams.Duration));
            _splash.gameObject.SetActive(true);
        }

        public void OnSplashClickedPlay()
        {
            _jamKit.TweenSeq(new TweenBase[]
            {
                new TweenImageColor(_coverImage, _jamKit.Globals.SceneTransitionParams.Color, _jamKit.Globals.SceneTransitionParams.Duration),
                new TweenCallback(InitGame),
                new TweenImageColor(_coverImage, Color.clear, _jamKit.Globals.SceneTransitionParams.Duration)

            });
        }

        private void InitGame()
        {
            _camera.backgroundColor = _jamKit.Globals.GameSceneCameraBackgroundColor;
            _splash.gameObject.SetActive(false);
            _gameMain.gameObject.SetActive(true);
            _gameMain.ResetGame();
        }

        public void OnGameDone()
        {
            _jamKit.TweenSeq(new TweenBase[]
            {
                new TweenImageColor(_coverImage, _jamKit.Globals.SceneTransitionParams.Color, _jamKit.Globals.SceneTransitionParams.Duration),
                new TweenCallback(() =>
                {
                    _camera.backgroundColor = _jamKit.Globals.IntermissionCameraBackgroundColor;
                    _gameMain.gameObject.SetActive(false);
                    _intermission.gameObject.SetActive(true);
                    _intermission.ResetIntermission();
                }),
                new TweenImageColor(_coverImage, Color.clear, _jamKit.Globals.SceneTransitionParams.Duration)
            });
        }

        public void OnIntermissionClickedPlay()
        {
            _jamKit.TweenSeq(new TweenBase[]
            {
                new TweenImageColor(_coverImage, _jamKit.Globals.SceneTransitionParams.Color, _jamKit.Globals.SceneTransitionParams.Duration),
                new TweenCallback(() =>
                {
                    _intermission.gameObject.SetActive(false);
                    _gameMain.gameObject.SetActive(true);
                    _gameMain.ResetGame();
                }),
                new TweenImageColor(_coverImage, Color.clear, _jamKit.Globals.SceneTransitionParams.Duration)
            });
        }
    }
}