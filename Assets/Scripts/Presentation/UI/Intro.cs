using System;
using System.Gameplay;
using Midiadub.Navigation.UI;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    [RequireComponent(typeof(CanvasGroupView))]
    public class Intro : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        private CanvasGroupView _cv;

        private void Awake()
        {
            _cv = GetComponent<CanvasGroupView>();
            _cv.Show();
            
            _signalBus.Subscribe<Game.Start>(Hide);
        }

        private void Hide(Game.Start obj)
        {
            _cv.Hide(() => { });
        }
    }
}