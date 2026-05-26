using Features.Player.Data;
using UnityEngine;
using Zenject;

namespace Features.Player
{
    public class StressManager : ITickable
    {
        private float _currentStress;
        private float _maxStress = 100f;

        private readonly SignalBus _signalBus;
        private readonly float _passiveReduce = 1f;

        public StressManager(SignalBus signalBus, StressSettingsSO stressSettings)
        {
            _signalBus = signalBus;
            _maxStress = stressSettings.MaxStress;
            _passiveReduce = stressSettings.PassiveReduce;
        }

        public void Tick()
        {
            AddStress(_passiveReduce * Time.deltaTime);
        }

        public void AddStress(float amount)
        {
            _currentStress = Mathf.Clamp(_currentStress + amount, 0f, _maxStress);
            FireStressSignal();
        }

        public void ReduceStress(float amount)
        {
            _currentStress = Mathf.Clamp(_currentStress - amount, 0f, _maxStress);

            if (_currentStress <= 0)
                _currentStress = 0f;

            FireStressSignal();
        }

        private void FireStressSignal()
        {
            _signalBus.Fire(new StressChangedSignal { NormalizedStress = _currentStress / _maxStress });
        }
    }
}