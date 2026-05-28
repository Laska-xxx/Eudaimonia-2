using UnityEngine;

namespace Features.Audio.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class UIAudioController : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            _audioSource.spatialBlend = 0f;
            _audioSource.playOnAwake = false;
        }

        public void PlaySound(AudioDataSO audioData)
        {
            if (audioData == null) return;
            audioData.PlayOneShot(_audioSource);
        }
    }
}