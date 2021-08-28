using System.Gameplay;
using Graphene.Time;
using Models.Accessors;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace System
{
    public class ScoreManager : IScore, ICombo
    {
        public Observer<int> Score { get; }= new Observer<int>();
        public Observer<int> Combo { get; }= new Observer<int>();
        
        private readonly SignalBus _signalBus;
        private readonly ITimeManager _timer;
        private readonly GameSettings _gameSettings;
        private Coroutine _comboDelayAnimation;

        public ScoreManager(SignalBus signalBus, ITimeManager timer,
            GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _timer = timer;
            _gameSettings = gameSettings;

            _signalBus.Subscribe<Player.HitEnemy>(AddCombo);
        }

        private void AddCombo(Player.HitEnemy data)
        {
            var currentCombo = Combo.GetValue();
            currentCombo++;
            
            Score.Commit(Score.GetValue() +  (int) (_gameSettings.scoreBase * (currentCombo * _gameSettings.comboMultiplier)));
            
            _signalBus.Fire(new Score.ComboChange(currentCombo));
            _signalBus.Fire(new Score.ScoreChange(Score.GetValue()));

            if (_comboDelayAnimation != null)
                _timer.Stop(_comboDelayAnimation);

            Combo.Commit(currentCombo);
            _comboDelayAnimation = _timer.Wait(_gameSettings.comboDelay,
                t =>
                {
                    _signalBus.Fire(
                        new Score.ComboUpdate(t / _gameSettings.comboDelay, t, _gameSettings.comboDelay - t));
                }, () =>
                {
                    Combo.Commit(0);
                }
            );
        }
    }
}