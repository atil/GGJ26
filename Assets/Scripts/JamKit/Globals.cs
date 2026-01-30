using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class SceneTransitionParams
    {
        [SerializeField] private Color _Color;
        public Color Color => _Color;

        [SerializeField] private float _Duration;
        public float Duration => _Duration;

        [SerializeField] private AnimationCurve _Curve;
        public AnimationCurve Curve => _Curve;

        [SerializeField] private bool _IsDiscrete;
        public bool IsDiscrete => _IsDiscrete;
    }

    [CreateAssetMenu(menuName = "Torreng/Globals")]
    public class Globals : ScriptableObject
    {
        [SerializeField] private float _discreteTickInteval;
        public float DiscreteTickInterval => _discreteTickInteval;

        [SerializeField] private SceneTransitionParams _sceneTransitionParams;
        public SceneTransitionParams SceneTransitionParams => _sceneTransitionParams;

        [SerializeField] private Color _splashSceneCameraBackgroundColor;
        public Color SplashSceneCameraBackgroundColor => _splashSceneCameraBackgroundColor;

        [SerializeField] private Color _gameSceneCameraBackgroundColor;
        public Color GameSceneCameraBackgroundColor => _gameSceneCameraBackgroundColor;

        [SerializeField] private Color _intermissionCameraBackgroundColor;
        public Color IntermissionCameraBackgroundColor => _intermissionCameraBackgroundColor;

        [SerializeField] private Color _buildSplashBackgroundColor;
        public Color BuildSplashBackgroundColor => _buildSplashBackgroundColor;
    }
}
