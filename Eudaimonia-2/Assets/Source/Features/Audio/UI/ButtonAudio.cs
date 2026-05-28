using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Features.Audio.UI
{
    public class ButtonAudio : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
            IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Down Button")]
        [SerializeField] private bool enableDownButtonSound = false;
        [SerializeField] private AudioDataSO downButtonAudio;

        [Header("Up Button")]
        [SerializeField] private bool enableUpButtonSound = false;
        [SerializeField] private AudioDataSO upButtonAudio;

        [Header("Enter Button")]
        [SerializeField] private bool enableEnterButtonSound = false;
        [SerializeField] private AudioDataSO enterButtonAudio;

        [Header("Exit Button")]
        [SerializeField] private bool enableExitButtonSound = false;
        [SerializeField] private AudioDataSO exitButtonAudio;

        private UIAudioController _audioController;
        private bool _isPressed;

        [Inject]
        private void Init(UIAudioController audioController)
        {
            _audioController = audioController;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!enableEnterButtonSound || _isPressed) return;
            _audioController.PlaySound(enterButtonAudio);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!enableExitButtonSound) return;
            _audioController.PlaySound(exitButtonAudio);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enableDownButtonSound) return;
            _isPressed = true;
            _audioController.PlaySound(downButtonAudio);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!enableUpButtonSound) return;
            _isPressed = false;
            _audioController.PlaySound(upButtonAudio);
        }
    }
}