using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Features.Audio.Effects
{
    public class PanicEffectAudio : MonoBehaviour
    {
        [Header("Sound Settings")]
        [SerializeField] private AudioDataSO heartBeat;
        [SerializeField] private float audioFadeDuration = 1f;

        private AudioSource _audioSource;
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

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void StartPanicAudio()
        {
            _audioTween?.Kill();

            _audioSource.volume = 0f;
            _audioSource.loop = true;
            heartBeat.Play(_audioSource);

            _audioTween = _audioSource.DOFade(heartBeat.Volume, audioFadeDuration)
                .SetEase(Ease.InOutSine);
        }

        private void StopPanicAudio()
        {
            _audioTween?.Kill();

            _audioTween = _audioSource.DOFade(0f, audioFadeDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _audioSource.Stop());
        }
    }
}
