using System;
using System.Gameplay;
using Midiadub.EasyEase;
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
        public float duration;
        private IActorData _actor;
        private Coroutine _animation;

        private void Awake()
        {
            _actor = isPlayer ? _gameManager.Player : _gameManager.Boss;
            _actor.Hp.AddListener(UpdateHealth);
        }

        private void UpdateHealth(int hp)
        {
            if (_animation != null)
                EaseEasy.Stop(_animation);
            
            _animation = EaseEasy.Animate(t =>
            {
                fill.fillAmount = t;
            }, fill.fillAmount, hp / (float) _actor.MaxHp, duration, 0,EaseTypes.Linear);
        }
    }
}