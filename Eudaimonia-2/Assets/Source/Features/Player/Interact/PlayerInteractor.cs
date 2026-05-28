using Core;
using Features.Interactable;
using Features.Player.Interact.Blowing;
using Features.UI;
using System;
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

        public event Action<string> OnInteractableFound;
        public event Action OnInteractableLost;

        private GameInput _gameInput;
        private NoteUI _noteUI;
        private DialogueUI _dialogueUI;
        private IInteractable _currentInteractable;

        private bool _isHintActive;

        [Inject]
        public void Init(InputManager inputManager, NoteUI noteUIController, PlayerInventory playerInventory, DialogueUI dialogueUI)
        {
            Inventory = playerInventory;
            _gameInput = inputManager.GameInput;
            _noteUI = noteUIController;
            _dialogueUI = dialogueUI;

            _gameInput.Player.Interact.performed += TryInteract;
        }

        private void Awake()
        {
            Grabber = GetComponent<PlayerGrabber>();
        }

        private void OnDestroy()
        {
            if (_gameInput != null)
                _gameInput.Player.Interact.performed -= TryInteract;
        }

        private void Update()
        {
            if (Grabber.IsHoldingItem)
            {
                if (_currentInteractable != null)
                {
                    _currentInteractable = null;
                    HideHintSafe();
                }
                return;
            }

            if (_noteUI.IsNoteOpen || _dialogueUI.IsActive)
            {
                HideHintSafe();
                return;
            }

            if (viewProvider.IsLookingAtLayer(interactableLayer, interactionDistance))
            {
                IInteractable interactable = viewProvider.CurrentHit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    if (_currentInteractable != interactable)
                    {
                        _currentInteractable = interactable;
                        OnInteractableFound?.Invoke(_currentInteractable.HintText);
                        _isHintActive = true;
                    }
                    return;
                }
            }

            if (_currentInteractable != null)
            {
                _currentInteractable = null;
                HideHintSafe();
            }
        }

        private void TryInteract(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (Grabber.IsHoldingItem)
            {
                Grabber.DropItem();
                return;
            }

            if (_currentInteractable != null && !_noteUI.IsNoteOpen)
            {
                _currentInteractable.Interact(this);
            }
        }

        private void HideHintSafe()
        {
            if (_isHintActive)
            {
                OnInteractableLost?.Invoke();
                _isHintActive = false;
            }
        }
    }
}