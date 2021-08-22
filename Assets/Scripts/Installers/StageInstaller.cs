using System;
using UnityEngine;
using Zenject;

public class StageInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneData>().AsSingle();
    }
}