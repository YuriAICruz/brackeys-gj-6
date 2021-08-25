using System.Gameplay;
using Models.Accessors;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    public class ActorHpDots : MonoBehaviour
    {
        public bool isPlayer;

        [Inject] private GameManager _gameManager;
        [Inject] private Heart.Factory _factory;

        private IActorData _actor;
        private Heart[] _hearts;

        private void Awake()
        {
            _actor = isPlayer ? _gameManager.Player : _gameManager.Boss;
            _actor.Hp.AddListener(UpdateHealth);

            _hearts = new Heart[_actor.MaxHp];
            for (int i = 0; i < _actor.MaxHp; i++)
            {
                _hearts[i] = _factory.Create(transform);
            }
        }

        private void UpdateHealth(int hp)
        {
            for (int i = 0; i < _actor.MaxHp; i++)
            {
                _hearts[i].SetValid((i + 1) <= hp);
            }
        }
    }
}