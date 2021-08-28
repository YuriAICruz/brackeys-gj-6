using Models.Accessors;
using Models.Signals;
using Zenject;

namespace System
{
    public class HighScoreManager : IHighScore
    {
        public Observer<int> HighScore { get; } = new Observer<int>();
        
        private readonly SignalBus _signalBus;
        private int _currentScore;

        public HighScoreManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
            
            _signalBus.Subscribe<Score.ScoreChange>(SetScore);
        }

        private void SetScore(Score.ScoreChange data)
        {
            _currentScore = data.score;
            
            if (_currentScore > HighScore.GetValue())
                HighScore.Commit(_currentScore);
        }
    }
}