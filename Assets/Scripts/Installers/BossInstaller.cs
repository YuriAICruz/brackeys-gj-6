using Presentation.Gameplay;
using Zenject;

public class BossInstaller : MonoInstaller
{
    public Actor boss;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Actor>().FromInstance(boss);
    }
}