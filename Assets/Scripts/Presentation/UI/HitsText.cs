using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class HitsText : TextViewer
    {
        [Inject] private IGameData _game;


        protected override void Awake()
        {
            base.Awake();

            _game.Hits.AddListener(UpdateData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _game.Hits.RemoveListener(UpdateData);
        }

        private void UpdateData(int hits)
        {
            SetText(hits.ToString("00"));
        }
    }
}