using Graphene.Time;
using Models.Accessors;
using Models.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace System.Gameplay
{
    public class GameManager
    {
        private readonly SignalBus _signalBus;
        private readonly ITimeManager _timer;
        private readonly GameSettings _settings;
        private IActorData player;
        private IActorData boss;
        private bool _gamerunning;

        public IActorData Player => (IActorData) player;
        public IActorData Boss => (IActorData) boss;

        public GameManager(SignalBus signalBus, ITimeManager timer, GameSettings settings)
        {
            _signalBus = signalBus;
            _timer = timer;
            _settings = settings;

            _signalBus.Subscribe<Models.Signals.Player.Death>(OnPlayerDeath);
            _signalBus.Subscribe<Models.Signals.Boss.Death>(OnBossDeath);

            _signalBus.Fire(new Bgm.Play(Bgm.Clips.MainBoss));
            _timer.Wait(_settings.startDelay, StartGame);
        }

        private void OnBossDeath(Boss.Death data)
        {
            _signalBus.Fire(new Bgm.Stop());
            EndGame();
        }

        private void OnPlayerDeath(Player.Death data)
        {
            _signalBus.Fire(new Bgm.Stop());
            EndGame();
        }

        private void EndGame()
        {
            if(!_gamerunning) return;
            _gamerunning = false;
            _signalBus.Fire<Game.End>();

            _timer.Wait(_settings.restartDelay, ReloadGame);
        }

        private void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void StartGame()
        {
            if(_gamerunning) return;

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