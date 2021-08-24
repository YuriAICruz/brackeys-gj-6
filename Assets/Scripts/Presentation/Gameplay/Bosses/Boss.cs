using Models.ModelView;
using UnityEngine;

namespace Presentation.Gameplay.Bosses
{
    public class Boss : Actor
    {
        [Space]
        public BossStatistics bossStats;
        public BossStates bossStates;
    }
}