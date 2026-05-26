using UnityEngine;

namespace Features.Player.Data
{
    [CreateAssetMenu(fileName = "StressSettingsData", menuName = "Player/StressSettings")]
    public class StressSettingsSO : ScriptableObject
    {
        [field: SerializeField] public float MaxStress { get; private set; } = 100f;
        [field: SerializeField] public float PassiveReduce { get; private set; } = 1f;
    }
}