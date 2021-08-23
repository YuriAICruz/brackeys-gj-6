using System;
using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.UI
{
    public class ActorHp : MonoBehaviour
    {
        [Inject] private IActorData _actor;

        private void Awake()
        {
            _actor.Hp.AddListener(UpdateHealth);
        }

        private void UpdateHealth(int hp)
        {
            Debug.Log($"hp {hp}");
        }
    }
}