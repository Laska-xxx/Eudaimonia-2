using DG.Tweening;
using Features.Player.Data;
using Features.Player.Move;
using Features.Player.Move.MoveStates;
using UnityEngine;
using Zenject;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;

    private CharacterController _characterController;
    private MovementSettingsSO _settings;
    private Tween _heightTween;

    [Inject]
    private void Init(MovementSettingsSO movementSettings)
    {
        _settings = movementSettings;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _playerMovement.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        _playerMovement.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(MovementStateEnum newState)
    {
        _heightTween?.Kill();

        bool isSquatting = newState == MovementStateEnum.Squatting;
        float targetHeight = isSquatting ? _settings.squatHeight : _settings.standingHeight;
        float duration = 3f / _settings.squatTransitionSpeed;

        _heightTween = DOTween.To(
            () => _characterController.height,
            x => _characterController.height = x,
            targetHeight,
            duration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }
}
