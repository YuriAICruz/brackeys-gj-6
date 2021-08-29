using System.Globalization;
using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class TimeText : TextViewer
    {
        [Inject] private IGameData _game;


        protected override void Awake()
        {
            base.Awake();

            _game.Time.AddListener(UpdateData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _game.Time.RemoveListener(UpdateData);
        }

        private void UpdateData(float time)
        {
            SetText(time.ToString("0.00", CultureInfo.InvariantCulture)+"s");
        }
    }
}