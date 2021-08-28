using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class ComboFeedback : TextViewer
    {
        [Inject] private ICombo _combo;
        
        protected override void Awake()
        {
            base.Awake();
            
            _combo.Combo.AddListener(CheckFeedback);
        }

        private void CheckFeedback(int combo)
        {
            if (combo > 5)
            {
                SetText("GREAT!");
            }

            SetText("");
        }
    }
}