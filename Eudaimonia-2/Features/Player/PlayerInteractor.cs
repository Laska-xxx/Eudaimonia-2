using Core;
using Features.Interactable;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 2.5f;
    [SerializeField] private LayerMask _interactableLayer;

    // Ссылки на другие компоненты игрока, чтобы передавать их предметам
    public PlayerInventoryModel Inventory { get; private set; }
    public PlayerGrabber Grabber { get; private set; }

    private GameInput _gameInput;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        Inventory = new PlayerInventoryModel();
        Grabber = GetComponent<PlayerGrabber>();
    }

    private void OnEnable()
    {
        _gameInput = InputManager.Instance.GameInput;
        _gameInput.Player.Interact.performed += ctx => TryInteract(); // Предполагается экшен Interact (клавиша E)
    }

    private void OnDisable()
    {
        if (_gameInput != null)
            _gameInput.Player.Interact.performed -= ctx => TryInteract();
    }

    private void Update()
    {
        // Пускаем луч из центра камеры
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    InteractionUI.Instance.ShowPrompt(_currentInteractable.PromptText); // Показываем UI
                }
                return;
            }
        }

        // Если луч никуда не попал
        if (_currentInteractable != null)
        {
            _currentInteractable = null;
            InteractionUI.Instance.HidePrompt();
        }
    }

    private void TryInteract()
    {
        // Сначала проверяем, не держим ли мы уже предмет (в GMod можно отпустить предмет, смотря в пустоту)
        if (Grabber.IsHoldingItem)
        {
            Grabber.DropItem();
            return;
        }

        // Если записка открыта — закрываем её (тут зависит от твоей архитектуры UI)
        if (NoteUIController.Instance.IsNoteOpen)
        {
            NoteUIController.Instance.CloseNote();
            return;
        }

        // Если смотрим на интерактивный предмет — взаимодействуем
        if (_currentInteractable != null && !NoteUIController.Instance.IsNoteOpen)
        {
            _currentInteractable.Interact(this);
        }
    }
}