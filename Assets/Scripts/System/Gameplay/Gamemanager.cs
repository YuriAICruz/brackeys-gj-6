using Graphene.Time;
using Models.Accessors;
using Models.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace System.Gameplay
{
    public class GameManager : IGameData
    {
        public Observer<float> Time { get; } = new Observer<float>();
        public Observer<int> Grade { get; } = new Observer<int>();
        public Observer<int> Hits { get; } = new Observer<int>();
        public Observer<int> Damages { get; } = new Observer<int>();
        public Observer<int> Heals { get; } = new Observer<int>();
        public Observer<int> Deaths { get; } = new Observer<int>();

        private readonly SignalBus _signalBus;
        private readonly ITimeManager _timer;
        private readonly ICombo _combo;
        private readonly GameSettings _settings;
        private IActorData player;
        private IActorData boss;
        private bool _gamerunning;
        private float _time;
        private bool _interacted;

        public IActorData Player => (IActorData) player;
        public IActorData Boss => (IActorData) boss;

        public GameManager(SignalBus signalBus, ITimeManager timer, ICombo combo, GameSettings settings)
        {
            _signalBus = signalBus;
            _timer = timer;
            _combo = combo;
            _settings = settings;

            _signalBus.Subscribe<Models.Signals.Player.Death>(OnPlayerDeath);
            _signalBus.Subscribe<Models.Signals.Boss.Death>(OnBossDeath);
            _signalBus.Subscribe<InputSignal.Down>(OnInputEvent);

            _signalBus.Fire(new Bgm.Play(Bgm.Clips.MainBoss));
            _timer.Wait(_settings.startDelay, StartGame);
        }

        private void OnInputEvent(InputSignal.Down data)
        {
            _interacted = true;
        }

        private void OnBossDeath(Boss.Death data)
        {
            _signalBus.Fire(new Bgm.Stop());
            EndGame();
        }

        private void OnPlayerDeath(Player.Death data)
        {
            
            Deaths.Commit(Deaths.GetValue()+1);
            _signalBus.Fire(new Bgm.Stop());
            EndGame();
        }

        private void EndGame()
        {
            if(!_gamerunning) return;
            
            _gamerunning = false;
            _signalBus.Fire<Game.End>();
            
            Time.Commit(Timer.time - _time);

            CalculateRank();

            var t = Timer.time;
            _interacted = false;
            _timer.Wait(() =>
            {
                var e = Timer.time - t;
                if (_interacted && e > _settings.restartDelay)
                    return true;
                
                return false;
            }, ReloadGame);
        }

        private void CalculateRank()
        {
            var grade = _settings.grades.Length/2;

            grade -= Deaths.GetValue() * 2;

            grade -= Mathf.FloorToInt(Damages.GetValue() / (float)_settings.gradeDamageCap);
            
            grade += Hits.GetValue() > _settings.gradeHitsCap ? 1 : 0;
            
            grade += _combo.MaxCombo.GetValue() >= _settings.gradeHitsCap ? 1 : 0;

            grade = Mathf.Clamp(grade, 0, _settings.grades.Length-1);
            
            Grade.Commit(grade);
        }

        private void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void StartGame()
        {
            if(_gamerunning) return;

            _time = Timer.time;

            _gamerunning = true;
            _signalBus.Fire<Models.Signals.Game.Start>();
        }

        public void SetActors(IActorData player, IActorData boss)
        {
            this.player = player;
            this.boss = boss;
        }
    }
}