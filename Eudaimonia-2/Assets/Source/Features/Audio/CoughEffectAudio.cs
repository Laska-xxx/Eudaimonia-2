using Features.Player.Interact.Blowing;
using UnityEngine;
using Zenject;

namespace Features.Audio
{
    public class CoughEffectAudio : MonoBehaviour
    {
        [SerializeField] private AudioDataSO coughSound;

        private SignalBus _signalBus;
        private AudioSource _audioSource;

        private void Init(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<CoughFromBlowingSignal>(PlayOnCough);
        }

        private void OnDisable()
        {
            _signalBus?.TryUnsubscribe<CoughFromBlowingSignal>(PlayOnCough);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void PlayOnCough(CoughFromBlowingSignal signal)
        {
            coughSound.Play(_audioSource);
        }
    }
}