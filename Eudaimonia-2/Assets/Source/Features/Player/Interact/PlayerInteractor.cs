using Core;
using Features.Interactable;
using Features.Player.Interact.Blowing;
using Features.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Features.Player.Interact
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private PlayerViewProvider viewProvider;

        public PlayerInventory Inventory { get; private set; }
        public PlayerGrabber Grabber { get; private set; }

        private GameInput _gameInput;
        private HintUI _interactionUI;
        private NoteUI _noteUI;
        private IInteractable _currentInteractable;

        [Inject]
        public void Init(InputManager inputManager, HintUI interactionUI, NoteUI noteUIController, PlayerInventory playerInventory)
        {
            Inventory = playerInventory;
            _gameInput = inputManager.GameInput;
            _interactionUI = interactionUI;
            _noteUI = noteUIController;

            _gameInput.Player.Interact.performed += TryInteract;
        }

        private void Awake()
        {
            Grabber = GetComponent<PlayerGrabber>();
        }

        private void OnDisable()
        {
            if (_gameInput != null)
                _gameInput.Player.Interact.performed -= TryInteract;
        }

        private void Update()
        {
            if (viewProvider.IsLookingAtLayer(interactableLayer, interactionDistance))
            {
                IInteractable interactable = viewProvider.CurrentHit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    if (_currentInteractable != interactable)
                    {
                        _currentInteractable = interactable;
                        _interactionUI.ShowHint(_currentInteractable.HintText);
                    }
                    return;
                }
            }

            if (_currentInteractable != null)
            {
                _currentInteractable = null;
                _interactionUI.HideHint();
            }
        }

        private void TryInteract(InputAction.CallbackContext ctx)
        {
            print("try interact");
           if (Grabber.IsHoldingItem)
            {
                Grabber.DropItem();
                return;
            }

            print(_currentInteractable);
            if (_currentInteractable != null && !_noteUI.IsNoteOpen)
            {
                _currentInteractable.Interact(this);
            }
        }
    }
}