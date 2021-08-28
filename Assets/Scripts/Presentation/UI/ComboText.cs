using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class ComboText: TextViewer
    {
        [Inject] private ICombo _combo;

        protected override void Awake()
        {
            base.Awake();
            _combo.Combo.AddListener(UpdateCombo);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _combo.Combo.RemoveListener(UpdateCombo);
        }

        private void UpdateCombo(int combo)
        {
            AnimateTextChangeText(combo.ToString("00"));
        }
    }
}