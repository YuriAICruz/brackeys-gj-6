using System;
using System.Gameplay;
using Models.Accessors;
using Presentation.Gameplay;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    public class ActorHp : MonoBehaviour
    {
        public bool isPlayer;

        [Inject] private GameManager _gameManager;

        private void Awake()
        {
            var actor = isPlayer ? _gameManager.Player : _gameManager.Boss;
            actor.Hp.AddListener(UpdateHealth);
        }

        private void UpdateHealth(int hp)
        {
            Debug.Log($"hp {hp}");
        }
    }
}