using System;
using System.Gameplay;
using Presentation.Gameplay;
using UnityEngine;
using Zenject;

public class StageInstaller : MonoInstaller
{
    public Actor player;
    public Actor boss;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneData>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().OnInstantiated<GameManager>(OnGameManagerInstantiated);
    }

    private void OnGameManagerInstantiated(InjectContext context, GameManager gm)
    {
        gm.SetActors(player, boss);
    }
}