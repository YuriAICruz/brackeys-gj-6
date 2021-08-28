using System;
using Models.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    public class ComboRemaining : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        public Image image;
        private void Awake()
        {
            image.fillAmount = 0;
            _signalBus.Subscribe<Score.ComboUpdate>(UpdateFill);
        }

        private void UpdateFill(Score.ComboUpdate data)
        {
            image.fillAmount = 1-data.normalized;
        }
    }
}