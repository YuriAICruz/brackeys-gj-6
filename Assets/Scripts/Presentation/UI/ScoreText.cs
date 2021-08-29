using Midiadub.EasyEase;
using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    public class ScoreText: TextViewer
    {
        [Inject] private IScore _score;
        private Coroutine _animation;
        private float _currentScore;

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
            if (_animation != null)
                EaseEasy.Stop(_animation);
            
            _animation = EaseEasy.Animate(t =>
            {
                _currentScore = t;
                SetText(_currentScore.ToString("000000"));
            }, _currentScore, score, duration, 0,EaseTypes.Linear);
        }
    }
}