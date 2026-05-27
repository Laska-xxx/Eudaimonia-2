using Features.Player.Data;
using DG.Tweening;
using Features.Player.Move.MoveStates;
using UnityEngine;
using Zenject;
using Features.Player.Move;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform viewLevel;
    [SerializeField] private Transform motionPivot;

    [Header("Walk Animation")]
    [SerializeField] private float walkAmplitude = 0.4f;
    [SerializeField] private float walkSpeed = 0.3f;

    [Header("Run Animation")]
    [SerializeField] private float sprintAmplitude = 0.5f;
    [SerializeField] private float sprintSpeed = 0.1f;

    [Header("Run Animation")]
    [SerializeField] private float squatAmplitude = 0.3f;
    [SerializeField] private float squatSpeed = 0.4f;

    private Tween _squatTween;
    private Tween _bobTween;
    private Vector3 _cameraBasePos;
    private MovementSettingsSO _settings;
    private bool _wasMoving;

    [Inject]
    private void Init(MovementSettingsSO movementSettings)
    {
        _settings = movementSettings;
    }

    private void Awake()
    {
        _cameraBasePos = motionPivot.localPosition;
    }

    private void OnEnable()
    {
        playerMovement.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        playerMovement.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (playerMovement != null && _wasMoving != playerMovement.IsMoving)
        {
            _wasMoving = playerMovement.IsMoving;
            UpdateHeadbob(playerMovement.CurrentState, _wasMoving);
        }
    }

    private void HandleStateChanged(MovementStateEnum newState)
    {
        bool isSquatting = newState == MovementStateEnum.Squatting;
        AnimateSquat(isSquatting);

        UpdateHeadbob(newState, playerMovement.IsMoving);
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
        float duration = walkSpeed;

        if (state == MovementStateEnum.Sprinting)
        {
            amplitude = sprintAmplitude;
            duration = sprintSpeed;
        }
        else if (state == MovementStateEnum.Squatting)
        {
            amplitude = squatAmplitude;
            duration = squatSpeed;
        }

        Sequence bobSequence = DOTween.Sequence();

        bobSequence.Append(motionPivot.DOLocalMoveY(_cameraBasePos.y, 0.15f)
            .SetEase(Ease.InOutSine));

        bobSequence.Append(motionPivot.DOLocalMoveY(_cameraBasePos.y + amplitude, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));

        bobSequence.SetLink(gameObject);
        _bobTween = bobSequence;
    }
}
