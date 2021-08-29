using Midiadub.EasyEase;
using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    public class MaxComboText : TextViewer
    {
        [Inject] private ICombo _combo;
        
        protected override void Awake()
        {
            base.Awake();
            
            _combo.MaxCombo.AddListener(UpdateCombo);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _combo.MaxCombo.RemoveListener(UpdateCombo);
        }

        private void UpdateCombo(int combo)
        {
            SetText(combo.ToString("00"));
        }
    }
}