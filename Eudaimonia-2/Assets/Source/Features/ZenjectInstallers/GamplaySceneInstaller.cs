using Core;
using Features.Player;
using Features.UI;
using UnityEngine;
using Zenject;
using Features.Player.Interact.Smoking;
using Features.Player.Data;
using Features.Player.Move;
using Features.Player.Stress;

namespace Features.ZenjectInstallers
{
    public class GamplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private StressSettingsSO stressSettings;
        [SerializeField] private MovementSettingsSO movementSettings;
        [SerializeField] private HintUI hintUI;
        [SerializeField] private NoteUIController NoteUIController;
        [SerializeField] private DialogueUI dialogueUI;
        [SerializeField] private TextAnimator textAnimator;

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

            Container.DeclareSignal<CigarettesCountChangedSignal>();
            Container.DeclareSignal<StressChangedSignal>();
            Container.DeclareSignal<CoughFromSmokingSignal>();
            Container.DeclareSignal<StaminaChangedSignal>();
        }

        private void InstallClasses()
        {
            Container.Bind<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<StressManager>().AsSingle();
            Container.Bind<PlayerInventory>().AsSingle();
        }

        private void InstallMonoClasses()
        {
            Container.Bind<HintUI>().FromInstance(hintUI).AsSingle();
            Container.Bind<NoteUIController>().FromInstance(NoteUIController).AsSingle();
            Container.Bind<DialogueUI>().FromInstance(dialogueUI).AsSingle();
            Container.Bind<TextAnimator>().FromInstance(textAnimator).AsSingle();
        }

        private void InstallSO()
        {
            Container.BindInstance(stressSettings).AsSingle();
            Container.BindInstance(movementSettings).AsSingle();
        }
    }
}