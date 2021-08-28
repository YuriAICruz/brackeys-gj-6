using Models.Accessors;
using Zenject;

namespace Presentation.UI
{
    public class ScoreText: TextViewer
    {
        [Inject] private IScore _score;

        protected override void Awake()
        {
            base.Awake();
            
            _score.Score.AddListener(UpdateScore);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _score.Score.RemoveListener(UpdateScore);
        }

        private void UpdateScore(int score)
        {
            SetText(score.ToString("000000"));
        }
    }
}