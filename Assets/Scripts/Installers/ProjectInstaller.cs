using System.Gameplay;
using UnityEngine;
using Zenject;

namespace Graphene.Installers
{
    [CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
    public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
    {
        public InputSettings settings;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<InputSignal.Up>();
            Container.DeclareSignal<InputSignal.Down>();
            Container.DeclareSignal<InputSignal.Axes>();

            Container.BindInstance(settings);

            Container.BindInterfacesAndSelfTo<PlayerInput>().AsSingle().NonLazy();
        }
    }
}