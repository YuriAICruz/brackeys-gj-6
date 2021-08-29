using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class DamageText : TextViewer
    {
        [Inject] private IGameData _game;


        protected override void Awake()
        {
            base.Awake();

            _game.Damages.AddListener(UpdateData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _game.Damages.RemoveListener(UpdateData);
        }

        private void UpdateData(int damage)
        {
            SetText(damage.ToString("00"));
        }
    }
}