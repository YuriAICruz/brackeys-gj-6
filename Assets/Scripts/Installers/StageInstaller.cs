using System;
using System.Gameplay;
using Presentation.Effects;
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
    public Bullet genericBullet;
    public Heart heart;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneData>().AsSingle();
        Container.BindInterfacesAndSelfTo<ParticlesManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle()
            .OnInstantiated<GameManager>(OnGameManagerInstantiated);

        Container.BindFactory<Spit, Spit.Factory>().FromComponentInNewPrefab(bossSpit);
        Container.BindFactory<Bullet, Bullet.Factory>().FromComponentInNewPrefab(genericBullet);
        Container.BindFactory<Heart, Heart.Factory>().FromComponentInNewPrefab(heart);

        Container.DeclareSignal<Models.Signals.Player.Death>();

        Container.DeclareSignal<Models.Signals.Game.Start>();
        Container.DeclareSignal<Models.Signals.Game.End>();

        Container.DeclareSignal<Models.Signals.FX.Hit>();
        Container.DeclareSignal<Models.Signals.FX.Slash>();
        Container.DeclareSignal<Models.Signals.FX.Smoke>();
        Container.DeclareSignal<Models.Signals.FX.Puff>();
    }

    private void OnGameManagerInstantiated(InjectContext context, GameManager gm)
    {
        gm.SetActors(player, boss);
    }
}