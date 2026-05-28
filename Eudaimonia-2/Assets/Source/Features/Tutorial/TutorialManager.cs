using Core;
using Cysharp.Threading.Tasks;
using Features.Interactable.NPS;
using Features.Player.Interact;
using Features.Player.Interact.Blowing;
using Features.Player.Move;
using Features.Player.Move.MoveStates;
using Features.Player.Stress;
using Features.UI;
using System;
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

    public event Action OnTaskCompleted;
    public event Action<string> OnHintShow;
    public event Action OnHintHide;

    private InputManager _inputManager;
    private GameInput _gameInput;
    private PlayerInventory _inventory;
    private StressManager _stressManager;
    private CancellationTokenSource _cts;

    [Inject]
    private void Init(InputManager inputManager, PlayerInventory inventory, StressManager stressManager)
    {
        _inputManager = inputManager;
        _gameInput = _inputManager.GameInput;
        _inventory = inventory;
        _stressManager = stressManager;
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

        #region Move
        _gameInput.Player.Move.Enable();
        OnHintShow?.Invoke("Я думаю стоит использовать [WASD] для передвижения");
        await UniTask.Delay(3000, cancellationToken: token);
        #endregion

        #region Jump
        _gameInput.Player.Jump.Enable();
        OnHintShow?.Invoke("Кажется на [SPACE] я смогу прыгнуть");

        bool hasJumped = false;
        void OnJump(InputAction.CallbackContext ctx) => hasJumped = true;
        _gameInput.Player.Jump.performed += OnJump;
        await UniTask.WaitUntil(() => hasJumped, cancellationToken: token);
        _gameInput.Player.Jump.performed -= OnJump;

        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Squat
        _gameInput.Player.Squat.Enable();
        OnHintShow?.Invoke("Я чувствую что то внутри... На [CTRL] я могу присесть!");
        await UniTask.WaitUntil(() => _playerMovement.CurrentState == MovementStateEnum.Squatting, cancellationToken: token);
        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Sprint
        _gameInput.Player.Sprint.Enable();
        OnHintShow?.Invoke("А бегать, похоже, на [SHIFT]");
        await UniTask.WaitUntil(() => _playerMovement.CurrentState == MovementStateEnum.Sprinting, cancellationToken: token);
        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Get Bubbles
        _gameInput.Player.Interact.Enable();

        OnHintShow?.Invoke("Я видел на полу пузырики, хочу подобрать их на [E]");
        int startBubbles = _inventory.SoapBubblesCount;
        await UniTask.WaitUntil(() => startBubbles < _inventory.SoapBubblesCount, cancellationToken: token);

        _gameInput.Player.Interact.Disable();
        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Reduse Stress
        _stressManager.AddStress(30f);
        OnHintShow?.Invoke("Мой уровень стресса постепенно повысился...");
        await UniTask.Delay(3000, cancellationToken: token);

        _gameInput.Player.EquipCigarette.Enable();
        _gameInput.Player.Smoke.Enable();

        OnHintShow?.Invoke("Надо взять пузырики в руку на [1], и подуть их, зажав [LKM], что бы успокоится");

        int initialBubbles = _inventory.SoapBubblesCount;
        await UniTask.WaitUntil(() => _inventory.SoapBubblesCount < initialBubbles || _stressManager.CurrentStress < 30f, cancellationToken: token);

        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        OnHintShow?.Invoke("Это и правда успокаивает. Хе хе");
        await UniTask.Delay(2000, cancellationToken: token);
        #endregion

        #region Speak With NPC
        _gameInput.Player.Interact.Enable();
        OnHintShow?.Invoke("Надо бы найти кого-нибудь и поговорить с ним на [E]");


        await UniTask.WaitUntil(() => _tutorialNPC.DialogueWasPlayed, cancellationToken: token);

        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Grab Item
        OnHintShow?.Invoke("Мне нужно расчистить поход к лестнице. Хорошо, что я умею брать предметы на [E]");

        await UniTask.WaitUntil(() => _interactor.Grabber.IsHoldingItem, cancellationToken: token);

        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Drop Item
        OnHintShow?.Invoke("Что бы бросить предмет нужно так же нажать [E]");
        await UniTask.WaitUntil(() => !_interactor.Grabber.IsHoldingItem, cancellationToken: token);

        OnHintHide?.Invoke();
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(1000, cancellationToken: token);
        #endregion

        #region Climbing
        OnHintShow?.Invoke("Пора выдираться из бункера, мне нужно подойти к лестнице и двигаться вперед, смотря вверх");

        await UniTask.WaitUntil(() => _playerMovement.CurrentState == MovementStateEnum.Climbing, cancellationToken: token);

        OnHintHide?.Invoke();

        await UniTask.WaitUntil(() => _playerMovement.CurrentState != MovementStateEnum.Climbing, cancellationToken: token);
        OnHintShow?.Invoke("Похоже я всему научился");
        OnTaskCompleted?.Invoke();
        await UniTask.Delay(3000, cancellationToken: token);
        OnHintHide?.Invoke();
        #endregion

        _gameInput.Player.Enable();
    }

    private void LockAllMechanics()
    {
        _gameInput.Player.Disable();
        _gameInput.Player.Look.Enable();
    }
}
