using Features.Player.Data;
using UnityEngine;

namespace Features.Player.Move.MoveStates
{
    public class ClimbingState : MovementState
    {
        public ClimbingState(MovementSettingsSO settings) : base(settings) { StateEnum = MovementStateEnum.Climbing; }

        public override Vector3 CalculateVelocity(Vector2 input, Vector3 right, Vector3 forward, Vector3 lookDirection, float verticalVelocity)
        {
            float verticalLook = lookDirection.y >= 0 ? 1f : -1f;

            Vector3 climbDir = Vector3.zero;
            climbDir.y = input.y * verticalLook;

            return climbDir.normalized * Settings.climbSpeed;
        }

        public override void ApplyGravity(ref float verticalVelocity, bool isGrounded, float deltaTime)
        {
            verticalVelocity = 0f;
        }

        public override bool CanJump() => false;
    }
}