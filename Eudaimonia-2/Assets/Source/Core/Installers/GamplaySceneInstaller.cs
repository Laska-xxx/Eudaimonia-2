using Features.Player;
using Features.UI;
using UnityEngine;
using Zenject;
using Features.Player.Interact.Blowing;
using Features.Player.Data;
using Features.Player.Move;
using Features.Player.Stress;
using Features.Player.Interact;

namespace Core.Installers
{
    public class GamplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private StressSettingsSO stressSettings;
        [SerializeField] private MovementSettingsSO movementSettings;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private NoteUI NoteUIController;
        [SerializeField] private DialogueUI dialogueUI;
        [SerializeField] private TextAnimator textAnimator;
        [SerializeField] private TutorialManager tutorialManager;

        public override void InstallBindings()
        {
            InstallSignalBus();
            InstallClasses();
            InstallMonoClasses();
            InstallSO();
        }

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<SoapBubblesCountChangedSignal>();
            Container.DeclareSignal<StressChangedSignal>();
            Container.DeclareSignal<CoughFromBlowingSignal>();
            Container.DeclareSignal<StaminaChangedSignal>();
            Container.DeclareSignal<StressMaxReachedSignal>();
            Container.DeclareSignal<StressDroppedBelowMaxSignal>();
        }

        private void InstallClasses()
        {
            Container.Bind<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<StressManager>().AsSingle();
            Container.Bind<PlayerInventory>().AsSingle();
        }

        private void InstallMonoClasses()
        {
            Container.Bind<PlayerInteractor>().FromInstance(playerInteractor).AsSingle();
            Container.Bind<NoteUI>().FromInstance(NoteUIController).AsSingle();
            Container.Bind<DialogueUI>().FromInstance(dialogueUI).AsSingle();
            Container.Bind<TextAnimator>().FromInstance(textAnimator).AsSingle();
            Container.Bind<TutorialManager>().FromInstance(tutorialManager).AsSingle();
        }

        private void InstallSO()
        {
            Container.BindInstance(stressSettings).AsSingle();
            Container.BindInstance(movementSettings).AsSingle();
        }
    }
}