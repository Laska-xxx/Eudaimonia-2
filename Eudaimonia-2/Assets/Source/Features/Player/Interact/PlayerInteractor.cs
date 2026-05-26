using UnityEngine;
using Core;
using Features.Player.Interact.Smoking;
using Features.Interactable;
using Features.UI;
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
        private NoteUIController _noteUIController;
        private IInteractable _currentInteractable;

        [Inject]
        public void Init(InputManager inputManager, HintUI interactionUI, NoteUIController noteUIController, PlayerInventory playerInventory)
        {
            Inventory = playerInventory;
            _gameInput = inputManager.GameInput;
            _interactionUI = interactionUI;
            _noteUIController = noteUIController;

            _gameInput.Player.Interact.performed += ctx => TryInteract();
        }

        private void Awake()
        {
            Grabber = GetComponent<PlayerGrabber>();
        }

        private void OnDisable()
        {
            if (_gameInput != null)
                _gameInput.Player.Interact.performed -= ctx => TryInteract();
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

        private void TryInteract()
        {
           if (Grabber.IsHoldingItem)
            {
                Grabber.DropItem();
                return;
            }

            if (_currentInteractable != null && !_noteUIController.IsNoteOpen)
            {
                _currentInteractable.Interact(this);
            }
        }
    }
}