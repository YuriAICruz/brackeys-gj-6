using Models.Accessors;
using UnityEngine;
using Zenject;

namespace System
{
    public class SceneData : ICamera
    {
        public Observer<Camera> ActiveCamera { get; } = new Observer<Camera>();
    }
}