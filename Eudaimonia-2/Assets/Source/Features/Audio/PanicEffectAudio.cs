using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Features.Audio
{
    public class PanicEffectAudio : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private AudioSource audioSource;

        [Header("Sound Settings")]
        [SerializeField] private AudioDataSO heartBeat;
        [SerializeField] private float audioVolume = 1f;
        [SerializeField] private float audioFadeDuration = 1f;

        private SignalBus _signalBus;
        private Tweener _audioTween;

        [Inject]
        private void Init(SignalBus signalBus)
        {
            _signalBus = signalBus;

            _signalBus.Subscribe<StressMaxReachedSignal>(StartPanicAudio);
            _signalBus.Subscribe<StressDroppedBelowMaxSignal>(StopPanicAudio);
        }

        private void OnDisable()
        {
            if (_signalBus != null)
            {
                _signalBus.TryUnsubscribe<StressMaxReachedSignal>(StartPanicAudio);
                _signalBus.TryUnsubscribe<StressDroppedBelowMaxSignal>(StopPanicAudio);
            }

            _audioTween?.Kill();
        }

        private void StartPanicAudio()
        {
            _audioTween?.Kill();

            audioSource.volume = 0f;
            audioSource.loop = true;
            heartBeat.Play(audioSource);

            _audioTween = audioSource.DOFade(audioVolume, audioFadeDuration)
                .SetEase(Ease.InOutSine);
        }

        private void StopPanicAudio()
        {
            _audioTween?.Kill();

            _audioTween = audioSource.DOFade(0f, audioFadeDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => audioSource.Stop());
        }
    }
}
