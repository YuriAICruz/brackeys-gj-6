using System;
using Models.Accessors;
using Models.Interfaces;
using UnityEngine;

namespace Presentation.Gameplay.Bosses
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BossDamageTextures : MonoBehaviour
    {
        private Boss _boss;
        private SkinnedMeshRenderer _skin;
        private int _currentStage;

        public Texture[] stages;

        private void Awake()
        {
            _boss = transform.GetComponentInParent<Boss>();
            transform.GetComponentInParent<IActorData>().Hp.AddListener(HpChanged);
            _skin = GetComponent<SkinnedMeshRenderer>();

            SetTexture(0);
        }

        private void HpChanged(int hp)
        {
            var step = _boss.stats.maxHp/stages.Length;
            for (int i = _currentStage; i < _boss.bossStats.stageLife.Length; i++)
            {
                if (hp <  _boss.stats.maxHp - step*i)
                {
                    _currentStage = i;
                    SetTexture(_currentStage);
                }
            }
        }

        private void SetTexture(int index)
        {
            _skin.material.SetTexture("_SubTexture", stages[index]);
        }
    }
}