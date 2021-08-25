using System;
using System.Gameplay;
using Presentation.Gameplay;
using Presentation.Gameplay.Projectiles;
using Presentation.UI;
using UnityEngine;
using Zenject;

public class StageInstaller : MonoInstaller
{
    public Actor player;
    public Actor boss;

    public Spit bossSpit;
    public Heart heart;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneData>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().OnInstantiated<GameManager>(OnGameManagerInstantiated);

        Container.BindFactory<Spit, Spit.Factory>().FromComponentInNewPrefab(bossSpit);
        Container.BindFactory<Heart, Heart.Factory>().FromComponentInNewPrefab(heart);

        Container.DeclareSignal<Models.Signals.Player.Death>();
        Container.DeclareSignal<Models.Signals.Game.Start>();
        Container.DeclareSignal<Models.Signals.Game.End>();
    }

    private void OnGameManagerInstantiated(InjectContext context, GameManager gm)
    {
        gm.SetActors(player, boss);
    }
}