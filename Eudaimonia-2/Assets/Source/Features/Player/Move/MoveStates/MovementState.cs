using Features.Player.Data;
using UnityEngine;

namespace Features.Player.Move.MoveStates
{
    public abstract class MovementState
    {
        protected readonly MovementSettingsSO Settings;

        public MovementStateEnum StateEnum { get; protected set; }

        protected MovementState(MovementSettingsSO settings)
        {
            Settings = settings;
        }

        public abstract Vector3 CalculateVelocity(Vector2 input, Vector3 right, Vector3 forward, Vector3 lookDirection, float verticalVelocity);

        public virtual void ApplyGravity(ref float verticalVelocity, bool isGrounded, float deltaTime)
        {
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity += Settings.gravity * deltaTime;
            }
        }

        public virtual bool CanJump() => true;

        public virtual float UpdateStamina(float currentStamina, float deltaTime)
        {
            return Mathf.Min(Settings.maxStamina, currentStamina + deltaTime);
        }
    }
}