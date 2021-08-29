using UnityEngine;

namespace System.Gameplay
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "Graphene/Brackeys-GJ/PhysicsSettings")]
    public class PhysicsSettings : ScriptableObject
    {
        public Vector3 gravity;
        
        public LayerMask actors;
        public LayerMask enemies;
        public LayerMask colliders;
        public LayerMask hittable;
        public LayerMask player;
        public LayerMask movementBlockers;
        public float minimumDistance = 0.1f;

        public int rayPrecision = 4;
        public int maxCollision = 3;
        public float maxSlope = 30;
        public float yDeath;
        public float yRespawn;
    }
}