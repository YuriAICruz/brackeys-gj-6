using Presentation.Gameplay;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    public Actor player;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Actor>().FromInstance(player);
    }
}