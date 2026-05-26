using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Features.Player.Stress;

namespace Features.Player.Interact.Blowing
{
    public class BlowingController : MonoBehaviour
    {
        [SerializeField] private GameObject cigaretteObj;
        [SerializeField] private Animator cigaretteAnimator;
        [SerializeField] private ParticleSystem cigaretteLitVfx; 
        [SerializeField] private ParticleSystem exhaleVfx;   

        [SerializeField] private float maxInhaleTime = 5f;
        [SerializeField] private float stressReliefAmount = 15f;
        [SerializeField] private float stressReduceAmount = 10f;

        private PlayerInventory _inventory;
        private StressManager _stressManager;
        private GameInput _gameInput;
        private SignalBus _signalBus;

        private enum SmokeState { Unequipped, Idle, Starting, Smoking }
        private SmokeState _currentState = SmokeState.Unequipped;

        private float _inhaleTimer;

        [Inject]
        private void Init(InputManager inputManager, PlayerInventory inventory, StressManager stressManager, SignalBus signalBus)
        {
            _gameInput = inputManager.GameInput;
            _inventory = inventory;
            _stressManager = stressManager;
            _signalBus = signalBus;

            _gameInput.Player.EquipCigarette.performed += ToggleSoapBubbles;
            _gameInput.Player.Smoke.started += StartBlow;
            _gameInput.Player.Smoke.canceled += StopBlow;
        }

        private void OnDisable()
        {
            if (_gameInput != null)
            {
                _gameInput.Player.EquipCigarette.performed -= ToggleSoapBubbles;
                _gameInput.Player.Smoke.started -= StartBlow;
                _gameInput.Player.Smoke.canceled -= StopBlow;
            }
        }

        private void Update()
        {
            if (_currentState == SmokeState.Starting)
            {
                AnimatorStateInfo stateInfo = cigaretteAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("CigaretteSmoking"))
                {
                    _currentState = SmokeState.Smoking;
                    _inhaleTimer = 0f;

                    cigaretteLitVfx.Play();
                }
            }
            else if (_currentState == SmokeState.Smoking)
            {
                _inhaleTimer += Time.deltaTime;

                if (_inhaleTimer >= maxInhaleTime)
                {
                    TriggerCough();
                }
            }
        }

        private void ToggleSoapBubbles(InputAction.CallbackContext ctx)
        {
            print(_currentState);
            if (_currentState == SmokeState.Starting || _currentState == SmokeState.Smoking) return;

            if (_currentState == SmokeState.Idle)
            {
                Unequip();
            }
            else if (_currentState == SmokeState.Unequipped)
            {
                if (_inventory.TryConsumeCigarette())
                {
                    _currentState = SmokeState.Idle;
                    cigaretteObj.SetActive(true);
                    cigaretteAnimator.Play("CigaretteIdle");
                    print("Ecip");
                }
                
            }
        }

        private void StartBlow(InputAction.CallbackContext ctx)
        {
            if (_currentState != SmokeState.Idle) return;

            print("Start Smok");
            _currentState = SmokeState.Starting;
            cigaretteAnimator.SetTrigger("Start Smok");
        }

        private void StopBlow(InputAction.CallbackContext ctx)
        {
            if (_currentState == SmokeState.Starting)
            {
                _currentState = SmokeState.Idle;

                cigaretteAnimator.Play("CigaretteIdle");
            }
            else if (_currentState == SmokeState.Smoking)
            {
                ExhaleBlow();
            }
        }

        private void ExhaleBlow()
        {
            _stressManager.ReduceStress(stressReliefAmount);
            EndBlowing();
        }

        private void TriggerCough()
        {
            _signalBus.Fire(new CoughFromBlowingSignal { stress = stressReduceAmount });
            _stressManager.AddStress(stressReduceAmount);
            EndBlowing();
        }

        private void EndBlowing()
        {
            _inventory.GetCigarette();
            exhaleVfx.Play();
            cigaretteLitVfx.Stop();

            if (_inventory.TryConsumeCigarette())
            {
                _currentState = SmokeState.Idle;
                cigaretteAnimator.Play("CigaretteIdle");
                return;
            }

            Unequip();
        }

        private void Unequip()
        {
            _currentState = SmokeState.Unequipped;
            cigaretteObj.SetActive(false);
        }
    }
}