using UnityEngine;
using DG.Tweening;

namespace Features.UI.DoTween
{
    public class AnimatedIcon : MonoBehaviour
    {
        public enum IdleAnimation
        {
            None,
            Bounce,
            Pulse,
            Wobble,
            Float,
            Shake,
            Spin,
            Heartbeat
        }

        [SerializeField] private IdleAnimation idleAnimation = IdleAnimation.Bounce;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float strength = 10f;
        [SerializeField] private Ease ease = Ease.InOutSine;

        private Tween _currentTween;
        private Vector3 _startPos;
        private Vector3 _startScale;
        private Quaternion _startRotation;

        private void Awake()
        {
            _startPos = transform.localPosition;
            _startScale = transform.localScale;
            _startRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            PlayAnimation(idleAnimation);
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        public void PlayAnimation(IdleAnimation anim)
        {
            StopAnimation();
            idleAnimation = anim;

            _currentTween = anim switch
            {
                IdleAnimation.Bounce => CreateBounce(),
                IdleAnimation.Pulse => CreatePulse(),
                IdleAnimation.Wobble => CreateWobble(),
                IdleAnimation.Float => CreateFloat(),
                IdleAnimation.Shake => CreateShake(),
                IdleAnimation.Spin => CreateSpin(),
                IdleAnimation.Heartbeat => CreateHeartbeat(),
                _ => null
            };
        }

        public void StopAnimation()
        {
            _currentTween?.Kill();
            _currentTween = null;
            transform.localPosition = _startPos;
            transform.localScale = _startScale;
            transform.localRotation = _startRotation;
        }

        // Подпрыгивает вверх-вниз
        private Tween CreateBounce()
        {
            return transform
                .DOLocalMoveY(_startPos.y + strength, duration * 0.5f)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // Равномерно пульсирует масштабом
        private Tween CreatePulse()
        {
            float scale = 1f + strength * 0.01f;
            return transform
                .DOScale(_startScale * scale, duration * 0.5f)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // Качается влево-вправо по Z
        private Tween CreateWobble()
        {
            return transform
                .DOLocalRotate(new Vector3(0, 0, strength), duration * 0.5f)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // Плавно плывёт вверх-вниз (мягче bounce)
        private Tween CreateFloat()
        {
            return transform
                .DOLocalMoveY(_startPos.y + strength * 0.5f, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // Случайный шейк позиции
        private Tween CreateShake()
        {
            return DOTween.Sequence()
                .Append(transform.DOShakePosition(duration, strength * 0.1f, 20, 90, false, true))
                .AppendInterval(0.1f)
                .SetLoops(-1);
        }

        // Постоянно крутится
        private Tween CreateSpin()
        {
            return transform
                .DOLocalRotate(new Vector3(0, 0, -360f), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        // Два быстрых пульса, пауза — как сердцебиение
        private Tween CreateHeartbeat()
        {
            float scale = 1f + strength * 0.015f;
            return DOTween.Sequence()
                .Append(transform.DOScale(_startScale * scale, duration * 0.1f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(_startScale, duration * 0.1f).SetEase(Ease.InQuad))
                .Append(transform.DOScale(_startScale * scale, duration * 0.1f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(_startScale, duration * 0.1f).SetEase(Ease.InQuad))
                .AppendInterval(duration * 0.6f)
                .SetLoops(-1);
        }
    }
}