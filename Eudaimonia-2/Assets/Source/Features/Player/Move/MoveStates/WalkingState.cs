using Features.Player.Data;
using UnityEngine;

namespace Features.Player.Move.MoveStates
{
    public class WalkingState : MovementState
    {
        public WalkingState(MovementSettingsSO settings) : base(settings) { StateEnum = MovementStateEnum.Walking; }

        public override Vector3 CalculateVelocity(Vector2 input, Vector3 right, Vector3 forward, Vector3 lookDirection, float verticalVelocity)
        {
            Vector3 moveDirection = (right * input.x + forward * input.y).normalized;
            Vector3 velocity = moveDirection * Settings.walkSpeed;
            velocity.y = verticalVelocity;
            return velocity;
        }
    }
}