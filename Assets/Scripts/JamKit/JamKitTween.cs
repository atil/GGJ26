using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum TweenUpdateResult
    {
        None, Running, Finished
    }

    public abstract class TweenBase
    {
        protected float Time { get; set; }
        protected float Duration { get; private set; }

        public abstract void Begin();
        public abstract TweenUpdateResult Tick(float dt);
        public bool HasStarted() => Time > 0;

        protected TweenBase(float duration)
        {
            Duration = duration;
            Time = 0;
        }
    }

    public class TweenMove : TweenBase
    {
        private readonly Transform _transform;
        private readonly Vector3 _target;
        private readonly AnimationCurve _curve;

        private Vector3 _start;

        public TweenMove(Transform transform, Vector3 target, float duration, AnimationCurve curve) : base(duration)
        {
            _transform = transform;
            _target = target;
            _curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public override void Begin()
        {
            _start = _transform.position;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            float t = _curve.Evaluate(Time / Duration);
            _transform.position = Vector3.Lerp(_start, _target, t);

            return TweenUpdateResult.Running;
        }
    }

    public class TweenMoveDiscrete : TweenBase
    {
        private readonly Transform _transform;
        private readonly Vector3 _target;
        private readonly AnimationCurve _curve;
        private readonly float _interval = 0.2f;

        private Vector3 _start;
        private int _step = 0;

        public TweenMoveDiscrete(Transform transform, Vector3 target, float duration, float interval, AnimationCurve curve) : base(duration)
        {
            _transform = transform;
            _target = target;
            _interval = interval;
            _curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public override void Begin()
        {
            _start = _transform.position;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            if (Time > _interval * _step)
            {
                _step++;
                float t = _curve.Evaluate(Time / Duration);
                _transform.position = Vector3.Lerp(_start, _target, t);
            }

            return TweenUpdateResult.Running;
        }
    }

    public class TweenMoveLocalY : TweenBase
    {
        private readonly Transform _transform;
        private readonly AnimationCurve _curve;
        private readonly float _targetY;

        private Vector3 _start;
        private Vector3 _target;

        public TweenMoveLocalY(Transform transform, float targetY, float duration, AnimationCurve curve) : base(duration)
        {
            _transform = transform;
            _targetY = targetY;
            _curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);

        }
        public override void Begin()
        {
            _start = _transform.localPosition;
            _target = _start;
            _target.y = _targetY;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            float t = _curve.Evaluate(Time / Duration);
            _transform.localPosition = Vector3.Lerp(_start, _target, t);

            return TweenUpdateResult.Running;
        }
    }

    public class TweenRotate : TweenBase
    {
        private readonly Transform _transform;
        private readonly Quaternion _target;
        private readonly AnimationCurve _curve;

        private Quaternion _start;

        public TweenRotate(Transform transform, Vector3 target, float duration, AnimationCurve curve) : base(duration)
        {
            _transform = transform;
            _target = Quaternion.Euler(target);
            _curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public override void Begin()
        {
            _start = _transform.rotation;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            float t = _curve.Evaluate(Time / Duration);
            _transform.rotation = Quaternion.Slerp(_start, _target, t);

            return TweenUpdateResult.Running;
        }
    }

    public class TweenScale : TweenBase
    {
        private readonly Transform _transform;
        private readonly Vector3 _target;
        private readonly AnimationCurve _curve;

        private Vector3 _start;

        public TweenScale(Transform transform, Vector3 target, float duration, AnimationCurve curve) : base(duration)
        {
            _transform = transform;
            _target = target;
            _curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public override void Begin()
        {
            _start = _transform.localScale;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            float t = _curve.Evaluate(Time / Duration);
            _transform.localScale = Vector3.Lerp(_start, _target, t);

            return TweenUpdateResult.Running;
        }
    }

    public class TweenImageFade : TweenBase
    {
        private readonly Image _image;
        private readonly float _target;
        private Color _colorStart;
        private Color _colorFinish;

        public TweenImageFade(Image image, float target, float duration) : base(duration)
        {
            _image = image;
            _target = target;
        }

        public override void Begin()
        {
            _colorStart = _image.color;
            _colorFinish = _colorStart;
            _colorFinish.a = _target;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            _image.color = Color.Lerp(_colorStart, _colorFinish, Time / Duration);
            return TweenUpdateResult.Running;
        }
    }

    public class TweenSpriteFade : TweenBase
    {
        private readonly SpriteRenderer _sprite;
        private readonly float _target;
        private Color _colorStart;
        private Color _colorFinish;

        public TweenSpriteFade(SpriteRenderer sprite, float target, float duration) : base(duration)
        {
            _sprite = sprite;
            _target = target;
        }

        public override void Begin()
        {
            _colorStart = _sprite.color;
            _colorFinish = _colorStart;
            _colorFinish.a = _target;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            _sprite.color = Color.Lerp(_colorStart, _colorFinish, Time / Duration);
            return TweenUpdateResult.Running;
        }
    }

    public class TweenImageColor : TweenBase
    {
        private readonly Image _image;
        private readonly Color _target;
        private Color _colorStart;

        public TweenImageColor(Image image, Color target, float duration) : base(duration)
        {
            _image = image;
            _target = target;
        }

        public override void Begin()
        {
            _colorStart = _image.color;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            _image.color = Color.Lerp(_colorStart, _target, Time / Duration);
            return TweenUpdateResult.Running;
        }
    }

    public class TweenAudioFade : TweenBase
    {
        private readonly AudioSource _audioSource;
        private readonly float _target;

        private float _start;

        public TweenAudioFade(AudioSource audioSource, float target, float duration) : base(duration)
        {
            _audioSource = audioSource;
            _target = target;
        }

        public override void Begin()
        {
            _start = _audioSource.volume;
        }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }

            _audioSource.volume = Mathf.Lerp(_start, _target, Time / Duration);

            return TweenUpdateResult.Running;
        }
    }

    public class TweenDelay : TweenBase
    {
        public TweenDelay(float duration) : base(duration) { }
        public override void Begin() { }

        public override TweenUpdateResult Tick(float dt)
        {
            Time += dt;
            if (Time >= Duration)
            {
                return TweenUpdateResult.Finished;
            }
            return TweenUpdateResult.Running;
        }
    }

    public class TweenCallback : TweenBase
    {
        private readonly Action _callback;

        public TweenCallback(Action callback) : base(0)
        {
            _callback = callback;
        }

        public override void Begin() { }

        public override TweenUpdateResult Tick(float dt)
        {
            _callback?.Invoke();
            return TweenUpdateResult.Finished;
        }
    }

    public partial class JamKit
    {
        private class Sequence
        {
            private TweenBase[] _tweens;
            private int _current = 0;

            public Sequence(TweenBase[] tweens)
            {
                _tweens = tweens;
            }

            public TweenUpdateResult Tick(float dt)
            {
                if (!_tweens[_current].HasStarted())
                {
                    _tweens[_current].Begin();
                }

                TweenUpdateResult result = _tweens[_current].Tick(dt);
                if (result == TweenUpdateResult.Finished)
                {
                    _current++;
                    if (_current == _tweens.Length)
                    {
                        return TweenUpdateResult.Finished;
                    }

                    return TweenUpdateResult.Running;
                }

                return TweenUpdateResult.Running;
            }
        }

        private readonly List<TweenBase> _singleTweens = new();
        private readonly List<Sequence> _sequences = new();

        public void Tween(TweenBase tween)
        {
            _singleTweens.Add(tween);
        }

        public void TweenSeq(TweenBase[] tweens)
        {
            _sequences.Add(new Sequence(tweens));
        }

        private void UpdateTweens(float dt)
        {
            for (int i = _singleTweens.Count - 1; i >= 0; i--)
            {
                if (!_singleTweens[i].HasStarted())
                {
                    _singleTweens[i].Begin();
                }

                TweenUpdateResult result = _singleTweens[i].Tick(dt);
                if (result == TweenUpdateResult.Finished)
                {
                    _singleTweens.RemoveAt(i);
                }
            }

            for (int i = _sequences.Count - 1; i >= 0; i--)
            {
                TweenUpdateResult result = _sequences[i].Tick(dt);
                if (result == TweenUpdateResult.Finished)
                {
                    _sequences.RemoveAt(i);
                }

            }
        }
    }
}