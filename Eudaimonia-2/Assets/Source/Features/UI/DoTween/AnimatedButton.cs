using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Features.UI.DoTween
{
    public class AnimatedButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Press")]
        [SerializeField] private bool _enablePressAnimation = true;
        [SerializeField] private float _pressScaleFactor = 0.92f;
        [SerializeField] private float _pressDuration = 0.25f;

        [Header("Hover")]
        [SerializeField] private bool _enableHoverAnimation = true;
        [SerializeField] private float _hoverScaleFactor = 1.06f;
        [SerializeField] private float _hoverDuration = 0.2f;

        [Header("Punch")]
        [SerializeField] private bool _enablePunchAnimation = true;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.15f, 0.15f, 0f);
        [SerializeField] private float _punchDuration = 0.35f;
        [SerializeField] private int _punchVibrato = 6;

        [Header("Shake")]
        [SerializeField] private bool _enableShakeAnimation = false;
        [SerializeField] private Vector3 _shakeStrength = new Vector3(5f, 0f, 0f);
        [SerializeField] private float _shakeDuration = 0.4f;
        [SerializeField] private int _shakeVibrato = 10;

        private bool _isPressed;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_enableHoverAnimation || _isPressed) return;
            transform.DOKill();
            transform.DOScale(_hoverScaleFactor, _hoverDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_enableHoverAnimation) return;
            _isPressed = false;
            transform.DOKill();
            transform.DOScale(1f, _hoverDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_enablePressAnimation) return;
            _isPressed = true;
            transform.DOKill();
            transform.DOScale(_pressScaleFactor, _pressDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_enablePressAnimation) return;
            _isPressed = false;
            transform.DOKill();
            float targetScale = _enableHoverAnimation ? _hoverScaleFactor : 1f;
            transform.DOScale(targetScale, _pressDuration).SetEase(Ease.OutBack);
        }

        public void PlayClickAnimation()
        {
            if (!_enablePunchAnimation) return;
            transform.DOKill();
            transform.DOPunchScale(_punchScale, _punchDuration, _punchVibrato)
                .OnComplete(() => transform.DOScale(1f, 0.1f));
        }

        public void PlayShakeAnimation()
        {
            if (!_enableShakeAnimation) return;
            transform.DOKill();
            transform.DOShakePosition(_shakeDuration, _shakeStrength, _shakeVibrato);
        }
    }
}