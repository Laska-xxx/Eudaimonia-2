using Core;
using Cysharp.Threading.Tasks;
using Features.Interactable.NPS;
using Features.Player.Interact;
using Features.Player.Interact.Blowing;
using Features.Player.Move;
using Features.Player.Move.MoveStates;
using Features.Player.Stress;
using Features.UI;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [SerializeField] private SpecialNPC _tutorialNPC;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerInteractor _interactor;

    private InputManager _inputManager;
    private GameInput _gameInput;

    private PlayerInventory _inventory;
    private StressManager _stressManager;
    private TutorialUI _tutorialUI;

    private CancellationTokenSource _cts;

    [Inject]
    private void Init(InputManager inputManager, PlayerInventory inventory, StressManager stressManager, TutorialUI tutorialUI)
    {
        _inputManager = inputManager;
        _gameInput = _inputManager.GameInput;
        _inventory = inventory;
        _stressManager = stressManager;
        _tutorialUI = tutorialUI;
    }

    private void Start()
    {
        _cts = new CancellationTokenSource();
        RunTutorialSequence(_cts.Token).Forget();
    }

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    private async UniTaskVoid RunTutorialSequence(CancellationToken token)
    {
        LockAllMechanics();

        _gameInput.Player.Move.Enable();
        _tutorialUI.ShowHint("Используйте [WASD] для перемещения");
        await UniTask.Delay(3000, cancellationToken: token);

        _gameInput.Player.Jump.Enable();
        _tutorialUI.ShowHint("Нажмите [SPACE] для прыжка");

        bool hasJumped = false;
        void OnJump(InputAction.CallbackContext ctx) => hasJumped = true;

        _gameInput.Player.Jump.performed += OnJump;
        await UniTask.WaitUntil(() => hasJumped, cancellationToken: token);
        _gameInput.Player.Jump.performed -= OnJump;

        _gameInput.Player.Squat.Enable();
        _tutorialUI.ShowHint("Нажмите [CTRL] для приседа");
        await UniTask.WaitUntil(() => _playerMovement.CurrentState == MovementStateEnum.Squatting, cancellationToken: token);
        _tutorialUI.HideHint();
        await UniTask.Delay(1000, cancellationToken: token);

        _gameInput.Player.Interact.Enable();

        _stressManager.AddStress(50f);
        _tutorialUI.ShowHint("Уровень стресса повышен...");
        await UniTask.Delay(3000, cancellationToken: token);

        _gameInput.Player.EquipCigarette.Enable();
        _gameInput.Player.Smoke.Enable();

        _tutorialUI.ShowHint("Достаньте мыльные пузыри [1] и зажмите [LKM], чтобы успокоиться");

        int initialBubbles = _inventory.SoapBubblesCount;
        await UniTask.WaitUntil(() => _inventory.SoapBubblesCount < initialBubbles || _stressManager.CurrentStress < 50f, cancellationToken: token);

        _tutorialUI.HideHint();
        _tutorialUI.ShowHint("У тебя хорошо получается");
        await UniTask.Delay(2000, cancellationToken: token);

        _tutorialUI.ShowHint("Подойдите к персонажу и поговорите с ним на [E]");

        await UniTask.WaitUntil(() => _tutorialNPC.DialogueWasPlayed, cancellationToken: token);

        _tutorialUI.HideHint();
        await UniTask.Delay(1000, cancellationToken: token);

        _tutorialUI.ShowHint("Наведитесь на ящик и нажмите [E], чтобы взять его");

        await UniTask.WaitUntil(() => _interactor.Grabber.IsHoldingItem, cancellationToken: token);

        _tutorialUI.ShowHint("Теперь отпустите предмет так же на [E]");
        await UniTask.WaitUntil(() => !_interactor.Grabber.IsHoldingItem, cancellationToken: token);

        _tutorialUI.HideHint();
        await UniTask.Delay(1000, cancellationToken: token);

        _tutorialUI.ShowHint("Подойдите к лестнице, посмотрите вверх и двигайтесь вперед, чтобы залезть на нее");

        await UniTask.WaitUntil(() => _playerMovement.CurrentState == MovementStateEnum.Climbing, cancellationToken: token);

        _tutorialUI.HideHint();

        _tutorialUI.ShowHint("Туториал успешно пройден!");
        await UniTask.Delay(3000, cancellationToken: token);
        _tutorialUI.HideHint();

        _gameInput.Player.Sprint.Enable();
        _gameInput.Player.Enable();
    }

    private void LockAllMechanics()
    {
        _gameInput.Player.Disable();
        _gameInput.Player.Look.Enable();
    }
}
