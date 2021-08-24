using Presentation.Gameplay;
using Presentation.Gameplay.Bosses;
using UnityEngine;

namespace Presentation.Animation
{
    [RequireComponent(typeof(Boss))]
    public class BossAnimation : MonoBehaviour
    {
        public Animator animator;
        private Boss _actor;

        private void Awake()
        {
            _actor = GetComponent<Boss>();
        }

        private void FixedUpdate()
        {
            animator.SetBool("Spiting", _actor.bossStates.spiting);
            animator.SetBool("Stagged", _actor.bossStates.stagged);
        }
    }
}