using System;

namespace Models.ModelView
{
    [Serializable]
    public class AttackAnimation
    {
        public float duration;
        public float delay;
        public float damageDuration;
    }
    [Serializable]
    public class ActorStatistics
    {
        public float speed = 3;
        public float dodgeSpeed = 4;
        public float runSpeed = 5;
        public float turnSpeed = 2;
        public float dodgeDuration = 1.4f;
        public float jumpForce = 2;
        public float height;
        public float radius;
        public AttackAnimation[] attacks;
        public float attackInputDelay;
        public float maxJumpTime;
    }
}