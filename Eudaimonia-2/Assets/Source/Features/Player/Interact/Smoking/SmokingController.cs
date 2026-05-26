using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Features.Player.Interact.Smoking
{
    public class SmokingController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject cigaretteObj;
        [SerializeField] private Animator cigaretteAnimator;
        [SerializeField] private ParticleSystem cigaretteLitVfx; // Опционально: дымок от самой тлеющей сигареты
        [SerializeField] private ParticleSystem exhaleVfx;       // Выдох дыма изо рта

        [Header("Settings")]
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

            _gameInput.Player.EquipCigarette.performed += ToggleCigarette;
            _gameInput.Player.Smoke.started += StartInhale;
            _gameInput.Player.Smoke.canceled += StopInhale;
        }

        private void OnDisable()
        {
            if (_gameInput != null)
            {
                _gameInput.Player.EquipCigarette.performed -= ToggleCigarette;
                _gameInput.Player.Smoke.started -= StartInhale;
                _gameInput.Player.Smoke.canceled -= StopInhale;
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

        private void ToggleCigarette(InputAction.CallbackContext ctx)
        {
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

        private void StartInhale(InputAction.CallbackContext ctx)
        {
            if (_currentState != SmokeState.Idle) return;

            print("Start Smok");
            _currentState = SmokeState.Starting;
            cigaretteAnimator.SetTrigger("Start Smok");
        }

        private void StopInhale(InputAction.CallbackContext ctx)
        {
            if (_currentState == SmokeState.Starting)
            {
                _currentState = SmokeState.Idle;

                cigaretteAnimator.Play("CigaretteIdle");
            }
            else if (_currentState == SmokeState.Smoking)
            {
                ExhaleSmoke();
            }
        }

        private void ExhaleSmoke()
        {
            _stressManager.ReduceStress(stressReliefAmount);
            EndSmoking();

            Unequip();
        }

        private void TriggerCough()
        {
            _signalBus.Fire(new CoughFromSmokingSignal { stress = stressReduceAmount });
            _stressManager.AddStress(stressReduceAmount);
            EndSmoking();

            Unequip();
        }

        private void EndSmoking()
        {
            _inventory.GetCigarette();
            exhaleVfx.Play();
            cigaretteLitVfx.Stop();
        }

        private void Unequip()
        {
            if (_inventory.TryConsumeCigarette())
            {
                _currentState = SmokeState.Idle;
                return;
            }
            _currentState = SmokeState.Unequipped;
            cigaretteObj.SetActive(false);
        }
    }
}