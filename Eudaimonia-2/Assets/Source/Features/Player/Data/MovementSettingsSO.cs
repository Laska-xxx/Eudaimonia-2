using UnityEngine;

namespace Features.Player.Data
{
    [CreateAssetMenu(fileName = "MovementSettingsData", menuName = "Player/MovementSettings")]
    public class MovementSettingsSO : ScriptableObject
    {
        [field: SerializeField] public float standingHeight { get; private set; } = 2;
        [field: SerializeField] public float squatHeight { get; private set; } = 1;
        [field: SerializeField] public float eysStandingHeight { get; private set; } = 0.9f;
        [field: SerializeField] public float eysSquatHeight { get; private set; } = 0.4f;
        [field: SerializeField] public float maxStamina { get; private set; } = 15;

        [field: SerializeField] public float walkSpeed { get; private set; } = 5;
        [field: SerializeField] public float sprintSpeed { get; private set; } = 7;

        [field: SerializeField] public float jumpForce { get; private set; } = 3;

        [field: SerializeField] public float squatSpeed { get; private set; } = 3;

        [field: SerializeField] public float squatTransitionSpeed { get; private set; } = 6;

        [field: SerializeField] public float climbSpeed { get; private set; } = 3;
        [field: SerializeField] public float gravity = -15;
        [field: SerializeField] public float pushForce { get; private set; } = 0.2f;
    }
}