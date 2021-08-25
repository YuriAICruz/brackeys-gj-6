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
    }
}