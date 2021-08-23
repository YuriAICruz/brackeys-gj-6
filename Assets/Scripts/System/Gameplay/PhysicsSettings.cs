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
        public float minimumDistance = 0.1f;
    }
}