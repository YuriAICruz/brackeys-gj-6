using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    public class HighScoreText : TextViewer
    {
        [Inject] private IHighScore _highScore;

        protected override void Awake()
        {
            base.Awake();
            
            _highScore.HighScore.AddListener(UpdateHighScore);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _highScore.HighScore.RemoveListener(UpdateHighScore);
        }

        private void UpdateHighScore(int highScore)
        {
            SetText(highScore.ToString("000000"));
        }
    }
}