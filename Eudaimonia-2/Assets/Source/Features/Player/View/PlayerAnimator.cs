using Features.Player.Data;
using DG.Tweening;
using Features.Player.Move.MoveStates;
using UnityEngine;
using Zenject;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Transform viewLevel;
    [SerializeField] private Transform motionPivot;

    [Header("Walk Animation")]
    [SerializeField] private float walkAmplitude = 0.3f;
    [SerializeField] private float walkSpeed = 1f;

    [Header("Run Animation")]
    [SerializeField] private float sprintAmplitude = 0.45f;
    [SerializeField] private float sprintSpeed = 0.5f;

    [Header("Run Animation")]
    [SerializeField] private float squatAmplitude = 0.25f;
    [SerializeField] private float squatSpeed = 1.2f;

    private Tween _squatTween;
    private Tween _bobTween;
    private Vector3 _cameraBasePos;
    private MovementSettingsSO _settings;

    [Inject]
    private void Init(MovementSettingsSO movementSettings)
    {
        _settings = movementSettings;
    }

    private void Awake()
    {
        _cameraBasePos = motionPivot.localPosition;
    }

    public void AnimateSquat(bool isSquatting)
    {
        _squatTween?.Kill();

        float targetEyeHeight = isSquatting ? _settings.eysSquatHeight : _settings.eysStandingHeight;
        float duration = 3f / _settings.squatTransitionSpeed;

        _squatTween = viewLevel.DOLocalMoveY(targetEyeHeight, duration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }

    public void UpdateHeadbob(MovementStateEnum state, bool isMoving)
    {
        _bobTween?.Kill();

        if (!isMoving || state == MovementStateEnum.Climbing)
        {
            _bobTween = motionPivot.DOLocalMoveY(_cameraBasePos.y, 0.2f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);
            return;
        }

        float amplitude = walkAmplitude;
        float speed = walkSpeed;

        if (state == MovementStateEnum.Sprinting)
        {
            amplitude = sprintAmplitude;
            speed = sprintSpeed;
        }
        else if (state == MovementStateEnum.Squatting)
        {
            amplitude = squatAmplitude;
            speed = squatSpeed;
        }

        _bobTween = motionPivot.DOLocalMoveY(_cameraBasePos.y + amplitude, speed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
