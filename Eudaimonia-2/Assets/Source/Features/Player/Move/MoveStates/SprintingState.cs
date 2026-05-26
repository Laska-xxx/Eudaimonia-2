using Features.Player.Data;
using UnityEngine;

namespace Features.Player.Move.MoveStates
{
    public class SprintingState : MovementState
    {
        public SprintingState(MovementSettingsSO settings) : base(settings) { StateEnum = MovementStateEnum.Sprinting; }

        public override Vector3 CalculateVelocity(Vector2 input, Vector3 right, Vector3 forward, Vector3 lookDirection, float verticalVelocity)
        {
            Vector3 moveDirection = (right * input.x + forward * input.y).normalized;
            Vector3 velocity = moveDirection * Settings.sprintSpeed;
            velocity.y = verticalVelocity;
            return velocity;
        }

        public override float UpdateStamina(float currentStamina, float deltaTime)
        {
            return Mathf.Max(0f, currentStamina - deltaTime);
        }
    }
}