using Features.Player.Data;
using Features.Player.Move.MoveStates;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using System;

namespace Features.Player.Move
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform viewHandler;
        [SerializeField] private PlayerAnimator playerAnimator;
        [SerializeField] private LadderDetector ladderDetector;

        public MovementStateEnum CurrentState => _validator.CurrentState;
        public bool IsMoving { get; private set; }

        public event Action OnJumped;
        public event Action OnLanded;
        public event Action<MovementStateEnum> OnStateChanged;

        private MovementSettingsSO _settings;
        private CharacterController _characterController;
        private PlayerMovementValidator _validator;
        private GameInput _gameInput;
        private Vector2 _moveInput;
        private SignalBus _signalBus;
        private float _lastStamina = -1f;
        private bool _wasGrounded;

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
            _validator.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            _validator.OnStateChanged -= HandleStateChanged;

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

            CheckGroundedStatus();
            CheckStamina();
        }

        private void HandleStateChanged(MovementStateEnum newState)
        {
            OnStateChanged?.Invoke(newState);
        }

        private void UpdateStates()
        {
            _moveInput = _gameInput.Player.Move.ReadValue<Vector2>();

            _validator.SetNearLadder(ladderDetector.IsNearLadder);
            _validator.UpdateStatesAndStamina(_moveInput, Time.deltaTime, ladderDetector.CheckLookingAtLadder());

            IsMoving = _moveInput.sqrMagnitude > 0.01f && (_characterController.isGrounded || _validator.CurrentState == MovementStateEnum.Climbing);
        }

        private void CheckGroundedStatus()
        {
            bool isGroundedNow = _characterController.isGrounded;
            if (isGroundedNow && !_wasGrounded) OnLanded?.Invoke();
            _wasGrounded = isGroundedNow;
        }

        private void CheckStamina()
        {
            if (Mathf.Abs(_lastStamina - _validator.CurrentStamina) > 0.001f)
            {
                _lastStamina = _validator.CurrentStamina;
                _signalBus.Fire(new StaminaChangedSignal { NormalizedStamina = _validator.CurrentStamina / _settings.maxStamina });
            }
        }

        private void Move()
        {
            _validator.ApplyGravity(_characterController.isGrounded, Time.deltaTime);

            Vector3 velocity = _validator.CalculateVelocity(_moveInput, transform.right, transform.forward, viewHandler.forward);

            _characterController.Move(velocity * Time.deltaTime);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (_characterController.isGrounded)
            {
                _validator.Jump();
                OnJumped?.Invoke();
            }
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
    }
}