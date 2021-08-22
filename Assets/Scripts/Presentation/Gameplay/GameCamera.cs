using System;
using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        public bool main;

        [Inject] private ICamera _camera;
        private Camera _cameraComponent;

        private void Awake()
        {
            _cameraComponent = GetComponent<Camera>();
            if (main)
                _camera.ActiveCamera.Commit(_cameraComponent);
        }
    }
}