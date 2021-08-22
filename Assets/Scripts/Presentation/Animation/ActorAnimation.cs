using System;
using Presentation.Gameplay;
using UnityEngine;

namespace Presentation.Animation
{
    [RequireComponent(typeof(Actor))]
    public class ActorAnimation : MonoBehaviour
    {
        public Animator animator;
        private Actor _actor;

        private void Awake()
        {
            _actor = GetComponent<Actor>();
        }

        private void FixedUpdate()
        {
            animator.SetFloat("Speed", _actor.states.currentSpeed);
            animator.SetFloat("TurnAngle", _actor.states.turnAngle);
            animator.SetBool("Attacking", _actor.states.attacking);
            animator.SetBool("Dodging", _actor.states.dodging);
        }
    }
}