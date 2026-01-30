using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public partial class JamKit
    {
        [Header("Sfx")]
        [SerializeField] private AudioSource _commonAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private SfxDatabase _database;

        private float _musicVolume = 0.15f;

        private bool _isMusicPaused;

        private void StartSfx()
        {
            _musicVolume = _musicAudioSource.volume;
        }

        public void FollowTransform(Transform t)
        {
            transform.SetParent(t);
        }

        private bool TryGetClip(string clipName, out AudioClip clip)
        {
            clip = null;
            foreach (AudioClip audioClip in _database.Clips)
            {
                if (audioClip == null)
                {
                    Debug.LogWarning("There's a null clip in the sfx database");
                    continue;
                }

                if (audioClip.name == clipName)
                {
                    clip = audioClip;
                    return true;
                }
            }

            Debug.LogError($"Audioclip not found: {clipName}");
            return false;
        }

        public void StartMusic(string clipName, bool isLooped)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                _musicAudioSource.loop = isLooped;
                _musicAudioSource.clip = clip;
                _musicAudioSource.volume = _isMusicPaused ? 0f : _musicVolume;
                _musicAudioSource.Play();
            }
        }

        public void PlayRandom(string clipPrefix)
        {
            List<AudioClip> clips = _database.Clips.Where(x => x.name.StartsWith(clipPrefix)).ToList();

            if (clips.Count > 0)
            {
                _commonAudioSource.PlayOneShot(clips.GetRandom());
            }
            else
            {
                Debug.LogError($"Couldn't find any clips with prefix {clipPrefix}");
            }
        }

        public void PlaySfx(string clipName)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                _commonAudioSource.PlayOneShot(clip);
            }
        }

        public void FadeOutMusic(float duration)
        {
            Tween(new TweenAudioFade(_musicAudioSource, 0, duration));
        }

        public void PlayOneShot(string clipName, Vector3 position, float volume = 1.0f)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }

        private void UpdateSfx()
        {
            if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.LeftControl))
            {
                if (_musicAudioSource.volume > 0.01f)
                {
                    _musicAudioSource.volume = 0f;
                    _isMusicPaused = true;
                }
                else
                {
                    _musicAudioSource.volume = _musicVolume;
                    _isMusicPaused = false;
                }
            }
        }
    }
}