using System;
using UnityEngine;

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
        public int maxHp;
        public float damageStagDuration;
        public float damageInvincibilityDuration;
        [Header("Locomotion")]
        public float speed = 3;
        public float dodgeSpeed = 4;
        public float runSpeed = 5;
        public float turnSpeed = 2;
        [Header("Physics")]
        public float height;
        public float radius;
        [Header("Dodge")]
        public float dodgeDuration = 1.4f;
        [Header("Jump")]
        public float jumpForce = 2;
        public float maxJumpTime;
        [Header("Attack")]
        public float attackInputDelay;
        public AttackAnimation[] attacks;
        [Space]
        public AttackAnimation aerialAttack;
        public int aerialAttackStage;
    }
}