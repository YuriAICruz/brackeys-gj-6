using System;
using Midiadub.EasyEase;
using UnityEngine;

namespace Presentation.Gameplay.Bosses
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BossBlendShapes : MonoBehaviour
    {
        private Boss _boss;
        private SkinnedMeshRenderer _skin;
        private bool _anticipating;

        public EaseTypes ease;
        public float transition;

        private Coroutine _animation;

        private void Awake()
        {
            _boss = transform.GetComponentInParent<Boss>();
            _skin = GetComponent<SkinnedMeshRenderer>();
        }

        private void Update()
        {
            if (_anticipating != BossActive())
                Animate(_boss.bossStates.anticipating);
        }

        private bool BossActive()
        {
            return _boss.bossStates.anticipating | _boss.states.attacking | _boss.bossStates.spiting;
        }

        private void Animate(bool anticipating)
        {
            _anticipating = anticipating;

            if (_animation != null)
                EaseEasy.Stop(_animation);

            _animation = EaseEasy.Animate(t => { _skin.SetBlendShapeWeight(0, t); }, _anticipating ? 0 : 100,
                _anticipating ? 100 : 0, transition, 0, ease, null,
                () => { _skin.SetBlendShapeWeight(0, _anticipating ? 100 : 0); });
        }
    }
}