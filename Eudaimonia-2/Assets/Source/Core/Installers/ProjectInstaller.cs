using Features.Audio.UI;
using Features.UI;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private UIAudioController uiAudioControllerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<UIAudioController>().FromComponentInNewPrefab(uiAudioControllerPrefab).AsSingle();
        }
    }
}