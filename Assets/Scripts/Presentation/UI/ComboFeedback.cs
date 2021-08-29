using System.Gameplay;
using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class ComboFeedback : TextViewer
    {
        [Inject] private ICombo _combo;
        [Inject] private GameSettings _gameSettings;
        
        protected override void Awake()
        {
            base.Awake();
            
            _combo.Combo.AddListener(CheckFeedback);
            SetText("");
        }

        private void CheckFeedback(int combo)
        {
            for (int i = _gameSettings.comboFeedbacks.Length-1; i >= 0 ; i--)
            {
                if (combo > _gameSettings.comboFeedbacks[i].mark)
                {
                    AnimateTextChangeText(_gameSettings.comboFeedbacks[i].feedback);
                    return;
                }
            }

            SetText("");
        }
    }
}