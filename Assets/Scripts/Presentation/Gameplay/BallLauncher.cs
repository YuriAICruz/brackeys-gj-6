using System;
using System.Gameplay;
using Models.Interfaces;
using Presentation.Effects;
using Presentation.Gameplay.Projectiles;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class BallLauncher : MonoBehaviour, IDamageable
    {
        [Inject] private GameManager _gameManager;
        [Inject] private Bullet.Factory _factory;

        public Bullet bulletPrefab;
        public MeshDestroy stand;
        public int Hp => 0;
        
        public float speed;
        private Bullet _bullet;
        private bool _disable;

        private void Awake()
        {
            _bullet = _factory.Create(bulletPrefab);
        }

        public void Damage(int damage)
        {
            if (_disable)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _disable = true;
            var dir = _gameManager.Boss.Center - transform.position;

            var fwd = _gameManager.Player.Transform.forward;
            var angle = Vector3.Angle(fwd, dir);
            Debug.Log(angle);

            if (angle > 90)
                dir = fwd;
            
            _bullet.Shoot(transform.position, dir.normalized, speed, 0);
            
            gameObject.SetActive(false);
            
            stand.Break();
        }
    }
}