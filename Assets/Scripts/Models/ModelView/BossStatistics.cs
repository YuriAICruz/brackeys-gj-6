using System;

namespace Models.ModelView
{
    [Serializable]
    public class BossStatistics
    {
        public float chaseDistance;
        public float spitDuration;
        public float stagBaseDuration;
        public float[] anticipationBaseDuration;
        public float movingStepDuration;
        public int sprayCount;
        public float spitBaseSpeed = 3;
        public float spitDelay;
        public float stagBaseDelay;
        public float backAttackAngle = 90;
        public int tailBaseDamage;
        public float attackRadius;
    }
}