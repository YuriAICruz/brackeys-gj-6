using UnityEngine;

namespace System.Gameplay
{
    public class Physics : IPhysics
    {
        private readonly PhysicsSettings _settings;
        private bool _grounded;
        private Vector3 _groundNormal;
        private Vector3 _acceleration;

        public bool Grounded => _grounded;
        
        public Physics(PhysicsSettings settings)
        {
            _settings = settings;
        }

        public Vector3 Evaluate(Vector3 position, float delta)
        {
            _acceleration += _settings.gravity * delta;
            var pos = position;

            CheckGroundRay(pos, _acceleration);

            if (_grounded)
            {
                _acceleration.y = 0;
            }

            pos += _acceleration * delta;

            return pos;
        }

        public Vector3 Evaluate(Vector2 direction, Vector3 position, float delta)
        {
            _acceleration += _settings.gravity * delta;

            var pos = position;

            CheckGroundRay(pos, _acceleration);

            if (_grounded)
            {
                _acceleration.y = 0;
            }

            pos += (new Vector3(direction.x, 0, direction.y) + _acceleration) * delta;

            return pos;
        }

        private void CheckGroundRay(Vector3 position, Vector3 direction)
        {
            if (UnityEngine.Physics.Raycast(position, direction, out var hit, _settings.minimumDistance,
                _settings.colliders))
            {
                Debug.DrawLine(position, position + direction * _settings.minimumDistance, Color.red);
                _grounded = true;
                _groundNormal = hit.normal;
                return;
            }

            _grounded = false;
        }

        private void CheckGroundSphere(float radius, Vector3 position, Vector3 direction)
        {
            if (UnityEngine.Physics.SphereCast(new Ray(position, direction), radius, out var hit,
                _settings.minimumDistance, _settings.colliders))
            {
                Debug.DrawLine(position, position + direction * _settings.minimumDistance, Color.red);
                _grounded = true;
                _groundNormal = hit.normal;
                return;
            }

            _grounded = false;
        }
    }
}