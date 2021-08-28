using System.Gameplay;
using Graphene.Time;
using Models.Accessors;
using Models.Interfaces;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace System
{
    public class ScoreManager : IScore, ICombo
    {
        public Observer<int> Score { get; } = new Observer<int>();
        public Observer<int> Combo { get; } = new Observer<int>();

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

            _signalBus.Subscribe<Player.HitEnemy>(AddComboEvt);
            _signalBus.Subscribe<Player.Hitted>(ResetComboEvt);
            _signalBus.Subscribe<Models.Signals.Score.OnHit>(CheckTargetHit);
            _signalBus.Subscribe<Models.Signals.Score.OnHitObject>(OnObjectHit);
        }

        private void OnObjectHit(Score.OnHitObject data)
        {
            if (Combo.GetValue() > 0)
                ComboDelay();
        }

        private void ResetComboEvt(Player.Hitted data)
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            if (_comboDelayAnimation != null)
                _timer.Stop(_comboDelayAnimation);

            Combo.Commit(0);

            _signalBus.Fire(
                new Score.ComboUpdate(1, _gameSettings.comboDelay, 0));
        }

        private void CheckTargetHit(Score.OnHit data)
        {
            if (data.gameObject.GetComponent<IPlayer>() != null)
            {
                ResetCombo();
                return;
            }

            if (data.gameObject.GetComponent<IEnemy>() != null)
            {
                AddCombo();
                return;
            }
        }

        private void AddComboEvt(Player.HitEnemy data)
        {
            AddCombo();
        }

        private void AddCombo()
        {
            var currentCombo = Combo.GetValue();
            currentCombo++;

            Score.Commit(Score.GetValue() +
                         (int) (_gameSettings.scoreBase * (currentCombo * _gameSettings.comboMultiplier)));

            _signalBus.Fire(new Score.ComboChange(currentCombo));
            _signalBus.Fire(new Score.ScoreChange(Score.GetValue()));


            Combo.Commit(currentCombo);
            ComboDelay();
        }

        private void ComboDelay()
        {
            if (_comboDelayAnimation != null)
                _timer.Stop(_comboDelayAnimation);

            _comboDelayAnimation = _timer.Wait(_gameSettings.comboDelay,
                t =>
                {
                    var e = t * _gameSettings.comboDelay;
                    _signalBus.Fire(
                        new Score.ComboUpdate(t, e, _gameSettings.comboDelay - e));
                }, () =>
                {
                    _signalBus.Fire(
                        new Score.ComboUpdate(1, _gameSettings.comboDelay, 0));
                    Combo.Commit(0);
                }
            );
        }
    }
}