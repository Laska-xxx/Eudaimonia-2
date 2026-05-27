using DG.Tweening;
using Features.Player.Interact.Blowing;
using Features.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Features.Effects
{
    public class CoughEffectView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI coughText;
        [SerializeField] private List<string> coughPhrases;
        
        [Header("Cough Settings")]
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private float effectDuration = 2f;
        [SerializeField] private Color coughVignetteColor = Color.red;
        [SerializeField] private float pulseDuration = 0.15f;
        [SerializeField] private float coughMaxIntensity = 0.5f;

        private SignalBus _signalBus;
        private Vignette _vignette;
        private TextAnimator _textAnimator;

        private Color _defaultVignetteColor;
        private float _defaultVignetteIntensity;

        private Tweener _coughTween;
        private Sequence _coughSequence;

        [Inject]
        private void Init(SignalBus signalBus , TextAnimator textAnimator)
        {
            _signalBus = signalBus;
            _textAnimator = textAnimator;
            _signalBus.Subscribe<CoughFromBlowingSignal>(PlayCoughEffect);
        }

        private void OnDisable()
        {
            _signalBus?.TryUnsubscribe<CoughFromBlowingSignal>(PlayCoughEffect);
            _coughTween?.Kill();
            _coughSequence?.Kill();
        }

        private void Start()
        {
            coughText.gameObject.SetActive(false);
            coughText.color = Color.red;

            if (postProcessVolume.profile.TryGet(out Vignette vignette))
            {
                _vignette = vignette;
                _defaultVignetteColor = _vignette.color.value;
                _defaultVignetteIntensity = _vignette.intensity.value;
            }
        }

        public void PlayCoughEffect(CoughFromBlowingSignal signal)
        {
            _coughTween?.Kill();
            _coughSequence?.Kill();
            ResetEffects();

            string curPhrase = coughPhrases[Random.Range(0, coughPhrases.Count)];
            coughText.gameObject.SetActive(true);
            _textAnimator.StartTyping(curPhrase, coughText);

            if (_vignette != null)
            {
                _vignette.color.Override(coughVignetteColor);

                _coughTween = DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
                    coughMaxIntensity, pulseDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutQuad);
            }

            _coughSequence = DOTween.Sequence();
            _coughSequence.AppendInterval(effectDuration)
                          .OnComplete(ResetEffects);
        }

        private void ResetEffects()
        {
            _coughTween?.Kill();
            coughText.gameObject.SetActive(false);

            if (_vignette != null)
            {
                _vignette.color.Override(_defaultVignetteColor);
                _vignette.intensity.Override(_defaultVignetteIntensity);
            }
        }
    }
}