using System;
using UnityEngine;

namespace Models.ModelView
{
    [Serializable]
    public class BossStatistics
    {
        public float chaseDistance;
        public float backupDistance;
        public float tableDistance;

        [Header("Dash")]
        public float dashSpeed;
        public float dashTurnSpeed;
        
        [Space]
        public float spitDuration;
        public float stagBaseDuration;
        public float[] anticipationBaseDuration;
        public float movingStepDuration;
        public float backupStepDuration;
        public int sprayCount;
        public float spitBaseSpeed = 3;
        public float spitDelay;
        public float stagBaseDelay;
        public float frontAttackAngle = 90;
        public float backAttackAngle = 90;
        public int tailBaseDamage;
        
        [Space]
        public float dashAttackRadius;
        public float attackRadius;
        
        [Range(0,1)]
        public float[] spitProbability;

        [Range(0,1)]
        public float[] dashBiteProbability;

        [Range(0,1)]
        public float[] stageLife;
    }
}