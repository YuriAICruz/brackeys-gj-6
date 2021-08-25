using System.Gameplay;
using System.Input;
using Graphene.Time;
using UnityEngine;
using Zenject;

namespace Graphene.Installers
{
    [CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
    public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
    {
        public InputSettings inputSettings;
        public PhysicsSettings physicsSettings;
        public GameSettings gameSettings;
        public FxSettings fxSettings;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<InputSignal.Up>();
            Container.DeclareSignal<InputSignal.Down>();
            Container.DeclareSignal<InputSignal.Axes>();

            Container.BindInstance(inputSettings);
            Container.BindInstance(physicsSettings);
            Container.BindInstance(gameSettings);
            Container.BindInstance(fxSettings);

            Container.BindInterfacesAndSelfTo<PlayerInput>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<TimeManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Timer>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<System.Gameplay.Physics>().AsTransient();
        }
    }
}