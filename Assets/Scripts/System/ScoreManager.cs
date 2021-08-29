using System.Gameplay;
using Graphene.Time;
using Models.Accessors;
using Models.Interfaces;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace System
{
    public class ScoreManager  
    {
        private readonly SignalBus _signalBus;
        private readonly ITimeManager _timer;
        private readonly IGameData _gameData;
        private readonly IScore _score;
        private readonly ICombo _combo;
        private readonly GameSettings _gameSettings;
        private Coroutine _comboDelayAnimation;

        public ScoreManager(SignalBus signalBus, ITimeManager timer, IGameData gameData, IScore score, ICombo combo, GameSettings gameSettings)
        {
            _signalBus = signalBus;
            _timer = timer;
            _gameData = gameData;
            _score = score;
            _combo = combo;
            _gameSettings = gameSettings;

            _signalBus.Subscribe<Player.HitEnemy>(AddComboEvt);
            _signalBus.Subscribe<Player.Hitted>(ResetComboEvt);
            _signalBus.Subscribe<Models.Signals.Score.OnHit>(CheckTargetHit);
            _signalBus.Subscribe<Models.Signals.Score.OnHitObject>(OnObjectHit);
        }

        private void OnObjectHit(Score.OnHitObject data)
        {
            if (_combo.Combo.GetValue() > 0)
                ComboDelay();
        }

        private void ResetComboEvt(Player.Hitted data)
        {
            _gameData.Damages.Commit(_gameData.Damages.GetValue()+1);
            
            ResetCombo();
        }

        private void ResetCombo()
        {
            if (_comboDelayAnimation != null)
                _timer.Stop(_comboDelayAnimation);

            _combo.Combo.Commit(0);

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
            var currentCombo = _combo.Combo.GetValue();
            currentCombo++;
            
            _gameData.Hits.Commit(_gameData.Hits.GetValue()+1);

            _score.Score.Commit(_score.Score.GetValue() +
                                (int) (_gameSettings.scoreBase * (currentCombo * _gameSettings.comboMultiplier)));

            _signalBus.Fire(new Score.ComboChange(currentCombo));
            _signalBus.Fire(new Score.ScoreChange(_score.Score.GetValue()));

            if(currentCombo > _combo.MaxCombo.GetValue())
                _combo.MaxCombo.Commit(currentCombo);
            
            _combo.Combo.Commit(currentCombo);
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
                    _combo.Combo.Commit(0);
                }
            );
        }
    }
}