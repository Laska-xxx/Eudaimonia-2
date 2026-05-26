using Features.Player;
using Features.Player.Move;
using Features.Player.Interact.Blowing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Features.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Slider stressSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TextMeshProUGUI ciggaretCountText;

        private SignalBus _signalBus;

        [Inject]
        private void Init(SignalBus signalBus)
        {
            _signalBus = signalBus;

            _signalBus.Subscribe<StressChangedSignal>(UpdateStressSlider);
            _signalBus.Subscribe<SoapBubblesCountChangedSignal>(UpdateCiggaretText);
            _signalBus.Subscribe<StaminaChangedSignal>(UpdateStaminaSlider);
        }

        private void OnDisable()
        {
            if (_signalBus != null)
            {
                _signalBus.TryUnsubscribe<StressChangedSignal>(UpdateStressSlider);
                _signalBus.TryUnsubscribe<SoapBubblesCountChangedSignal>(UpdateCiggaretText);
                _signalBus.TryUnsubscribe<StaminaChangedSignal>(UpdateStaminaSlider);
            }
        }

        private void UpdateStressSlider(StressChangedSignal signal)
        {
            stressSlider.value = signal.NormalizedStress;
        }

        private void UpdateStaminaSlider(StaminaChangedSignal signal)
        {
            staminaSlider.value = signal.NormalizedStamina;
        }

        private void UpdateCiggaretText(SoapBubblesCountChangedSignal signal)
        {
            ciggaretCountText.text = signal.NewCount.ToString();
        }
    }
}