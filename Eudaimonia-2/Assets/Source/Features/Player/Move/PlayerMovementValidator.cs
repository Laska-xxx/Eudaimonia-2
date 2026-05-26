using System;
using System.Collections.Generic;
using UnityEngine;
using Features.Player.Move.MoveStates;
using Features.Player.Data;

namespace Features.Player.Move
{
    public class PlayerMovementValidator
    {
        public MovementStateEnum CurrentState => _currentStrategy.StateEnum;
        public bool WantsToStandUp { get; private set; }
        public float CurrentStamina { get; private set; }
        public bool IsSquating => CurrentState == MovementStateEnum.Squatting;

        public event Action<MovementStateEnum> OnStateChanged;

        private readonly MovementSettingsSO _settings;
        private MovementState _currentStrategy;
        private readonly Dictionary<MovementStateEnum, MovementState> _strategies;

        private float _verticalVelocity;
        private bool _canToSprint;
        private bool _isNearLadder;

        public PlayerMovementValidator(MovementSettingsSO settings)
        {
            _settings = settings;
            CurrentStamina = _settings.maxStamina;

            _strategies = new Dictionary<MovementStateEnum, MovementState>
            {
                { MovementStateEnum.Walking, new WalkingState(settings) },
                { MovementStateEnum.Squatting, new SquattingState(settings) },
                { MovementStateEnum.Sprinting, new SprintingState(settings) },
                { MovementStateEnum.Climbing, new ClimbingState(settings) }
            };

            SetState(MovementStateEnum.Walking);
        }

        private void SetState(MovementStateEnum newState)
        {
            if (_currentStrategy?.StateEnum == newState) return;

            _currentStrategy = _strategies[newState];
            OnStateChanged?.Invoke(newState);
        }

        public void TrySquat()
        {
            if (CurrentState == MovementStateEnum.Climbing) return;

            SetState(MovementStateEnum.Squatting);
            WantsToStandUp = false;
            _canToSprint = false;
        }

        public void TryStandUp() => WantsToStandUp = true;

        public void ConfirmStandUp() => SetState(MovementStateEnum.Walking);

        public void SetSprint(bool isSprinting)
        {
            if (CurrentState == MovementStateEnum.Squatting || CurrentState == MovementStateEnum.Climbing) return;
            _canToSprint = isSprinting;
        }

        public void SetNearLadder(bool isNear)
        {
            _isNearLadder = isNear;

            if (!_isNearLadder && CurrentState == MovementStateEnum.Climbing)
            {
                SetState(WantsToStandUp ? MovementStateEnum.Walking : (IsSquating ? MovementStateEnum.Squatting : MovementStateEnum.Walking));
            }
        }

        public void UpdateStatesAndStamina(Vector2 moveInput, float deltaTime, bool isLookingLadder)
        {
            if (_isNearLadder && Mathf.Abs(moveInput.y) > 0.1f && isLookingLadder)
            {
                SetState(MovementStateEnum.Climbing);
                _canToSprint = false;
            }
            else if (CurrentState == MovementStateEnum.Climbing && !isLookingLadder)
            {
                SetState(WantsToStandUp ? MovementStateEnum.Walking : (IsSquating ? MovementStateEnum.Squatting : MovementStateEnum.Walking));
            }

            bool isMoving = moveInput.magnitude > 0.1f;
            bool canSprint = _canToSprint && isMoving && CurrentStamina > 0f && CurrentState != MovementStateEnum.Squatting && CurrentState != MovementStateEnum.Climbing;

            if (canSprint && CurrentState == MovementStateEnum.Walking)
            {
                SetState(MovementStateEnum.Sprinting);
            }
            else if (CurrentState == MovementStateEnum.Sprinting && !canSprint)
            {
                SetState(MovementStateEnum.Walking);
            }

            CurrentStamina = _currentStrategy.UpdateStamina(CurrentStamina, deltaTime);
        }

        public void ApplyGravity(bool isGrounded, float deltaTime)
        {
            _currentStrategy.ApplyGravity(ref _verticalVelocity, isGrounded, deltaTime);
        }

        public void Jump()
        {
            if (!_currentStrategy.CanJump()) return;

            _verticalVelocity = Mathf.Sqrt(_settings.jumpForce * -2f * _settings.gravity);
        }

        public Vector3 CalculateVelocity(Vector2 input, Vector3 right, Vector3 forward, Vector3 lookDirection)
        {
            return _currentStrategy.CalculateVelocity(input, right, forward, lookDirection, _verticalVelocity);
        }
    }
}