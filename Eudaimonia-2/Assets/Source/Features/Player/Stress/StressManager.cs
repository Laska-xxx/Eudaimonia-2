using Features.Player.Data;
using UnityEngine;
using Zenject;

namespace Features.Player.Stress
{
    public class StressManager : ITickable
    {
        public float CurrentStress => _currentStress;

        private float _currentStress;
        private float _maxStress = 100f;
        private float _lastSignaledStress = -1f;

        private bool _isMaxStressReached = false;

        private readonly SignalBus _signalBus;
        private readonly float _passiveReduce;

        private const float UpdateThreshold = 0.5f;

        [Inject]
        public StressManager(SignalBus signalBus, StressSettingsSO stressSettings)
        {
            _signalBus = signalBus;
            _maxStress = stressSettings.MaxStress;
            _passiveReduce = stressSettings.PassiveReduce;
        }

        public void Tick()
        {
            ChangeStressValue(_passiveReduce * Time.deltaTime);
        }

        public void AddStress(float amount)
        {
            ChangeStressValue(amount);
        }

        public void ReduceStress(float amount)
        {
            ChangeStressValue(-amount);
        }

        private void ChangeStressValue(float delta)
        {
            _currentStress = Mathf.Clamp(_currentStress + delta, 0f, _maxStress);
            CheckStatesAndFireSignals();
        }

        private void CheckStatesAndFireSignals()
        {
            if (_currentStress >= _maxStress && !_isMaxStressReached)
            {
                _isMaxStressReached = true;
                _signalBus.Fire<StressMaxReachedSignal>();
            }
            
            else if (_currentStress < _maxStress && _isMaxStressReached)
            {
                _isMaxStressReached = false;
                _signalBus.Fire<StressDroppedBelowMaxSignal>();
            }

            if (Mathf.Abs(_currentStress - _lastSignaledStress) >= UpdateThreshold ||
                _currentStress == 0f ||
                _currentStress == _maxStress)
            {
                _lastSignaledStress = _currentStress;
                _signalBus.Fire(new StressChangedSignal { NormalizedStress = _currentStress / _maxStress });
            }
        }
    }
}