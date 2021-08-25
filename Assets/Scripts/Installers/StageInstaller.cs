using System;
using System.Gameplay;
using Presentation.Gameplay;
using Presentation.Gameplay.Projectiles;
using UnityEngine;
using Zenject;

public class StageInstaller : MonoInstaller
{
    public Actor player;
    public Actor boss;

    public Spit bossSpit;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneData>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().OnInstantiated<GameManager>(OnGameManagerInstantiated);

        Container.BindFactory<Spit, Spit.Factory>().FromComponentInNewPrefab(bossSpit);
    }

    private void OnGameManagerInstantiated(InjectContext context, GameManager gm)
    {
        gm.SetActors(player, boss);
    }
}