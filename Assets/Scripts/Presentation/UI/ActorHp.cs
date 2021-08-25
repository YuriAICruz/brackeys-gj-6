using System;
using System.Gameplay;
using Models.Accessors;
using Presentation.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    public class ActorHp : MonoBehaviour
    {
        public bool isPlayer;

        [Inject] private GameManager _gameManager;

        public Image fill;
        private IActorData _actor;

        private void Awake()
        {
            _actor = isPlayer ? _gameManager.Player : _gameManager.Boss;
            _actor.Hp.AddListener(UpdateHealth);
        }

        private void UpdateHealth(int hp)
        {
            Debug.Log($"hp {hp}");

            fill.fillAmount = hp / (float) _actor.MaxHp;
        }
    }
}