using Models.Signals;
using Zenject;

namespace Graphene.Installers
{
    public class ActorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<Actor.Attack>();
            Container.DeclareSignal<Actor.Damage>();

            Container.DeclareSignal<Bgm.Play>();
            Container.DeclareSignal<Bgm.Stop>();
            
            Container.DeclareSignal<SFX.Play>();
        }
    }
}