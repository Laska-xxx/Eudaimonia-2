using Features.Player.Data;
using Features.Player.Move.MoveStates;
using Core;
using DG.Tweening;
using Features.Interactable.Environment;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Features.Player.Move
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask ladderLayer;
        [SerializeField] private Transform viewHandler;
        [SerializeField] private PlayerViewProvider viewProvider;
        [SerializeField] private PlayerAnimator playerAnimator;

        public MovementStateEnum CurrentState => _validator.CurrentState;
        private MovementSettingsSO _settings;
        private CharacterController _characterController;
        private PlayerMovementValidator _validator;
        private GameInput _gameInput;
        private Vector2 _moveInput;
        private Collider _currentLadder;
        private SignalBus _signalBus;
        private float _lastStamina = -1f;
        private Tween _heightTween;
        private bool _isMoving;

        [Inject]
        private void Init(InputManager inputManager, MovementSettingsSO movementSettings, SignalBus signalBus)
        {
            _gameInput = inputManager.GameInput;
            _settings = movementSettings;
            _signalBus = signalBus;

            _gameInput.Player.Jump.performed += OnJump;
            _gameInput.Player.Squat.performed += OnSquat;
            _gameInput.Player.Squat.canceled += OnSquatCanceled;
            _gameInput.Player.Sprint.performed += OnSprint;
            _gameInput.Player.Sprint.canceled += OnSprintCanceled;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _validator = new PlayerMovementValidator(_settings);
            _validator.OnStateChanged += OnMovementStateChanged;
        }

        private void OnDisable()
        {
            _validator.OnStateChanged -= OnMovementStateChanged;

            if (_gameInput != null)
            {
                _gameInput.Player.Jump.performed -= OnJump;
                _gameInput.Player.Squat.performed -= OnSquat;
                _gameInput.Player.Squat.canceled -= OnSquatCanceled;
                _gameInput.Player.Sprint.performed -= OnSprint;
                _gameInput.Player.Sprint.canceled -= OnSprintCanceled;
            }
        }

        private void Update()
        {
            UpdateStates();
            HandleSquatState();
            Move();

            if (!(_lastStamina == _validator.CurrentStamina))
            {
                _lastStamina = _validator.CurrentStamina;

                _signalBus.Fire(new StaminaChangedSignal{NormalizedStamina = _validator.CurrentStamina / _settings.maxStamina});
            }
        }

        private void UpdateStates()
        {
            _moveInput = _gameInput.Player.Move.ReadValue<Vector2>();

            bool isLookingAtLadder = false;

            if (_currentLadder != null)
            {
                Vector3 lookXZ = new Vector3(viewProvider.LookDirection.x, 0f, viewProvider.LookDirection.z);

                Vector3 ladderCenter = _currentLadder.bounds.center;
                Vector3 toLadderXZ = new Vector3(ladderCenter.x - transform.position.x, 0f, ladderCenter.z - transform.position.z);

                if (lookXZ.sqrMagnitude > 0.001f && toLadderXZ.sqrMagnitude > 0.001f)
                {
                    float dot = Vector3.Dot(lookXZ.normalized, toLadderXZ.normalized);

                    isLookingAtLadder = dot > 0.2f;
                }
                else
                {
                    isLookingAtLadder = true;
                }

                if (!isLookingAtLadder && viewProvider.IsLookingAtLayer(ladderLayer))
                {
                    isLookingAtLadder = true;
                }
            }

            _validator.UpdateStatesAndStamina(_moveInput, Time.deltaTime, isLookingAtLadder);

            bool currentlyMoving = _moveInput.sqrMagnitude > 0.01f && _characterController.isGrounded;
            if (currentlyMoving != _isMoving)
            {
                _isMoving = currentlyMoving;
                playerAnimator?.UpdateHeadbob(_validator.CurrentState, _isMoving);
            }
        }

        private void OnMovementStateChanged(MovementStateEnum newState)
        {
            bool isSquatting = newState == MovementStateEnum.Squatting;

            playerAnimator.AnimateSquat(isSquatting);

            playerAnimator.UpdateHeadbob(newState, _isMoving);

            _heightTween?.Kill();

            float targetHeight = isSquatting ? _settings.squatHeight : _settings.standingHeight;

            float duration = 3f / _settings.squatTransitionSpeed;

            _heightTween = DOTween.To(() => _characterController.height, x => _characterController.height = x, targetHeight, duration)

                .SetEase(Ease.OutQuad)

                .SetLink(gameObject);
        }

        private void Move()
        {
            _validator.ApplyGravity(_characterController.isGrounded, Time.deltaTime);

            Vector3 velocity = _validator.CalculateVelocity(_moveInput, transform.right, transform.forward, viewHandler.forward);

            _characterController.Move(velocity * Time.deltaTime);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (_characterController.isGrounded) _validator.Jump();
        }
        private void OnSquat(InputAction.CallbackContext ctx)
        {
            _validator.TrySquat();
        }

        private void OnSquatCanceled(InputAction.CallbackContext ctx)
        {
            _validator.TryStandUp();
        }
        private void OnSprint(InputAction.CallbackContext ctx)
        {
            _validator.SetSprint(true);
        }
        private void OnSprintCanceled(InputAction.CallbackContext ctx)
        {
            _validator.SetSprint(false);
        }


        private void HandleSquatState()
        {
            if (_validator.WantsToStandUp && CanStandUp())
            {
                _validator.ConfirmStandUp();
            }
        }

        private bool CanStandUp()
        {
            float radius = _characterController.radius * 0.9f;
            float headOffset = (_settings.standingHeight / 2f) - radius;
            Vector3 targetHeadCenter = transform.position + Vector3.up * headOffset;

            return !Physics.CheckSphere(targetHeadCenter, radius, obstacleLayer, QueryTriggerInteraction.Ignore);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((ladderLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("ladder");
                _currentLadder = other;
                _validator.SetNearLadder(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((ladderLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                if (other == _currentLadder)
                {
                    _currentLadder = null;
                    _validator.SetNearLadder(false);
                }
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.TryGetComponent<GrabbableItem>(out GrabbableItem item))
            {
                if (hit.moveDirection.y < -0.3f) return;

                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                var body = item.gameObject.GetComponent<Rigidbody>();
                if (body != null) body.AddForce(pushDir * _settings.pushForce, ForceMode.VelocityChange);
            }
        }
    }
}