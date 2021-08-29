using System.Input;
using Midiadub.Navigation.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    public class FullscreenButton : ButtonView
    {
        [Inject] private InputSettings _inputSettings;
        public Text text;

        private void Setup()
        {
            text = transform.GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetText();
        }

        private void SetText()
        {
            text.text = "Fullscreen";//Screen.fullScreen ? "Fullscreen" : "Exit Fullscreen";
        }

        protected override void OnClick()
        {
            base.OnClick();
            
            Screen.fullScreen = !Screen.fullScreen;
            SetText();
        }
    }
}