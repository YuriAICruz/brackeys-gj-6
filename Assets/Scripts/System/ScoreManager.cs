using System.Gameplay;
using Graphene.Time;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace System
{
    public class ScoreManager
    {
        private readonly SignalBus _signalBus;
        private readonly HighScoreManager _highScoreManager;
        private readonly ITimeManager _timer;
        private readonly GameSettings _gameSettings;
        private int _currentCombo;
        private Coroutine _comboDelayAnimation;
        private int _score;

        public ScoreManager(SignalBus signalBus, HighScoreManager highScoreManager, ITimeManager timer,
            GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _highScoreManager = highScoreManager;
            _timer = timer;
            _gameSettings = gameSettings;

            _signalBus.Subscribe<Player.HitEnemy>(AddCombo);
        }

        private void AddCombo(Player.HitEnemy data)
        {
            _currentCombo++;
            
            _score += (int) (_gameSettings.scoreBase * (_currentCombo * _gameSettings.comboMultiplier));
            
            _signalBus.Fire(new Score.ComboChange(_currentCombo));
            _signalBus.Fire(new Score.ScoreChange(_score));

            if (_comboDelayAnimation != null)
                _timer.Stop(_comboDelayAnimation);

            _comboDelayAnimation = _timer.Wait(_gameSettings.comboDelay,
                t =>
                {
                    _signalBus.Fire(
                        new Score.ComboUpdate(t / _gameSettings.comboDelay, t, _gameSettings.comboDelay - t));
                }, () => { _currentCombo = 0; }
            );
        }
    }

    public class HighScoreManager
    {
        private readonly SignalBus _signalBus;
        private int _currentScore;
        private int _highScore;

        public HighScoreManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
            
            _signalBus.Subscribe<Score.ScoreChange>(SetScore);
        }

        private void SetScore(Score.ScoreChange data)
        {
            _currentScore = data.score;
            
            if (_currentScore > _highScore)
                _highScore = _currentScore;
        }
    }
}