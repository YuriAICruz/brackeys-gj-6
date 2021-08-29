using Graphene.Time;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay.Projectiles
{
    public class Spit : Bullet
    {
        public class Factory : PlaceholderFactory<Spit>
        {
            public override Spit Create()
            {
                return base.Create();
            }
        }

        public TrailRenderer trail;
        public float waveSize;
        public float waveLength;

        protected override void Deactivate()
        {
            trail.emitting = false;
            base.Deactivate();
        }

        public override void Shoot(Vector3 position, Vector3 dir, float speed, float delay)
        {
            trail.emitting = true;
            
            base.Shoot(position, dir, speed, delay);
        }

        protected override Vector3 GetDirection(Vector3 dir)
        {
            dir.x += Mathf.Sin((Time.time+_delay)*waveLength) * waveSize; 
            dir.z += Mathf.Sin((Time.time+_delay)*waveLength) * waveSize; 
            
            return dir.normalized;
        }
    }
}