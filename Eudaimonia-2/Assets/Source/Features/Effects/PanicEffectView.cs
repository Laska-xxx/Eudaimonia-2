using DG.Tweening;
using Features.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

public class PanicEffectView : MonoBehaviour
{
    [SerializeField] private Volume postProcessVolume;

    [Header("Panic Settings")]
    [SerializeField] private float panicVignetteIntensity = 0.6f;
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float pulseMagnitude = 0.1f;
    [SerializeField] private float panicPixelationFactor = 0.1f;
    [SerializeField] private float pixelationTransitionSpeed = 2f;

    private SignalBus _signalBus;
    private Vignette _vignette;
    private PS1VolumeComponent _ps1Effect;

    private bool _isPanicking;
    private float _defaultVignetteIntensity;
    private float _defaultPixelationIntensity;

    private Tweener _vignetteTween;
    private Tweener _pixelationTween;

    [Inject]
    private void Init(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<StressChangedSignal>(OnStressChanged);
    }

    private void OnDisable()
    {
        _signalBus?.TryUnsubscribe<StressChangedSignal>(OnStressChanged);

        _vignetteTween?.Kill();
        _pixelationTween?.Kill();
    }

    private void Start()
    {
        if (postProcessVolume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
            _defaultVignetteIntensity = _vignette.intensity.value;
        }

        if (postProcessVolume.profile.TryGet(out PS1VolumeComponent pS1Volume))
        {
            _ps1Effect = pS1Volume;
            _defaultPixelationIntensity = _ps1Effect.pixelationFactor.value;
        }
    }

    private void OnStressChanged(StressChangedSignal signal)
    {
        if (signal.NormalizedStress >= 0.99f && !_isPanicking)
            StartPanic();
        else if (signal.NormalizedStress < 0.99f && _isPanicking)
            StopPanic();
    }

    private void StartPanic()
    {
        _isPanicking = true;

        if (_vignette != null)
        {
            _vignette.intensity.value = panicVignetteIntensity;

            _vignetteTween = DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
                panicVignetteIntensity + pulseMagnitude, pulseDuration)
                .SetLoops(-1, LoopType.Yoyo) 
                .SetEase(Ease.InOutSine); 
        }

        if (_ps1Effect != null)
        {
            _pixelationTween?.Kill();
            _pixelationTween = DOTween.To(() => _ps1Effect.pixelationFactor.value, x => _ps1Effect.pixelationFactor.value = x,
                panicPixelationFactor, pixelationTransitionSpeed);
        }
    }

    private void StopPanic()
    {
        _isPanicking = false;

        _vignetteTween?.Kill();
        if (_vignette != null)
        {
            DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, _defaultVignetteIntensity, 0.5f);
        }

        _pixelationTween?.Kill();
        if (_ps1Effect != null)
        {
            _pixelationTween = DOTween.To(() => _ps1Effect.pixelationFactor.value, x => _ps1Effect.pixelationFactor.value = x,
                _defaultPixelationIntensity, 0.5f);
        }
    }
}
