using System;
using Models.Interfaces;
using UnityEngine;

namespace Presentation.Gameplay.Bosses
{
    public class BossPieces : MonoBehaviour, IDamageable
    {
        public int Hp { get; }

        public float damageMultiplier = 1;

        public Action<int> damaged;
        
        public void Damage(int damage)
        {
            damaged?.Invoke((int)(damage*damageMultiplier));
        }
    }
}