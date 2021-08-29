using System;
using System.Gameplay;
using Midiadub.Navigation.UI;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    [RequireComponent(typeof(CanvasGroupView))]
    public class Results : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        private CanvasGroupView _cv;

        private void Awake()
        {
            _cv = GetComponent<CanvasGroupView>();
            _cv.Hide();
            
            _signalBus.Subscribe<Game.End>(Show);
        }

        private void Show(Game.End obj)
        {
            _cv.Show(() => { });
        }
    }
}