using System;
using System.Gameplay;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class Actor : MonoBehaviour
    {
        [Inject] protected IPhysics _physics;
        [Inject] protected SignalBus _signalBus;

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
        
        protected virtual void FixedUpdate()
        {
            transform.position = _physics.Evaluate(transform.position, Time.fixedDeltaTime);
        }
    }
}
