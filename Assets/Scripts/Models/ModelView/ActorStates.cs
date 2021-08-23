using System;
using UnityEngine;

namespace Presentation.Gameplay
{
    [Serializable]
    public class ActorStates
    {
        public float currentSpeed;
        public Vector3 direction;
        public float turnAngle;
        public bool grounded;
        public bool running;
        public bool attacking;
        public bool dodging;
        public bool jumping;
        public int attackStage;
        public float lastAttack;
    }
}