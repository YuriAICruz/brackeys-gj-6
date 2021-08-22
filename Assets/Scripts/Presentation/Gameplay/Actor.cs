using System;
using System.Gameplay;
using Graphene.Time;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class Actor : MonoBehaviour
    {
        [Inject] protected IPhysics _physics;
        [Inject] protected SignalBus _signalBus;
        [Inject] protected ITimeManager _timer;

        public ActorStatistics stats;
        public ActorStates states;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void Update()
        {
        }

        protected void FixedUpdate()
        {
            if(states.attacking || states.dodging) return;
            
            Move();
        }
        
        protected virtual void Move()
        {
            transform.position = _physics.Evaluate(transform.position, Time.fixedDeltaTime);
        }


        protected virtual void Attack()
        {
            states.attacking = true;
            _timer.Wait(1, () => { states.attacking = false; });
        }

        protected virtual void Dodge()
        {
            states.dodging = true;
            _timer.Wait(stats.dodgeDuration, () => { states.dodging = false; });
        }

        protected virtual void Jump()
        {
        }
    }
}