using System;
using System.Gameplay;
using System.Input;
using System.Sound;
using Graphene.Time;
using Models.Signals;
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
        public BgmSettings bgmSettings;
        public SfxSettings sfxSettings;

        public AudioSource bgm, sfx;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.Bind<BgmManager>().AsSingle().WithArguments(Instantiate(bgm)).NonLazy();
            
            Container.Bind<SfxManager>().AsSingle().WithArguments(Instantiate(sfx)).NonLazy();
            Container.Bind<HighScoreManager>().AsSingle().NonLazy();

            Container.DeclareSignal<Bgm.Play>();
            Container.DeclareSignal<Bgm.Stop>();
            
            Container.DeclareSignal<SFX.Play>();
            
            Container.DeclareSignal<InputSignal.Up>();
            Container.DeclareSignal<InputSignal.Down>();
            Container.DeclareSignal<InputSignal.Axes>();

            Container.BindInstance(inputSettings);
            Container.BindInstance(physicsSettings);
            Container.BindInstance(gameSettings);
            Container.BindInstance(fxSettings);
            Container.BindInstance(bgmSettings);
            Container.BindInstance(sfxSettings);

            Container.BindInterfacesAndSelfTo<PlayerInput>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<TimeManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Timer>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<System.Gameplay.Physics>().AsTransient();
        }
    }
}