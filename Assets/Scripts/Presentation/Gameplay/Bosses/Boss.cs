using Models.Interfaces;
using Models.ModelView;
using UnityEngine;

namespace Presentation.Gameplay.Bosses
{
    public class Boss : Actor, IEnemy
    {
        [Space]
        public BossStatistics bossStats;
        public BossStates bossStates;
    }
}