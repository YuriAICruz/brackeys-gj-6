using System.Gameplay;
using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class GradeText : TextViewer
    {
        [Inject] private IGameData _game;
        [Inject] private GameSettings _settings;


        protected override void Awake()
        {
            base.Awake();

            _game.Grade.AddListener(UpdateData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _game.Grade.RemoveListener(UpdateData);
        }

        private void UpdateData(int grade)
        {
            SetText(_settings.grades[grade]);
        }
    }
}