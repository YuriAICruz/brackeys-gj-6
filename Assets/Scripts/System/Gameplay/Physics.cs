using UnityEngine;

namespace System.Gameplay
{
    public class Physics : IPhysics
    {
        private readonly PhysicsSettings _settings;
        private bool _grounded;
        private bool _jumping;
        private int _stepsSinceGrounded;
        private Vector3 _groundNormal;
        private Vector3 _acceleration;
        private float _radius;
        private bool _useSphere;
        private float _height;

        public bool Grounded => _grounded;

        public Physics(PhysicsSettings settings)
        {
            _settings = settings;
        }

        public void SetSphereRadius(float radius, float height)
        {
            _useSphere = true;
            _radius = radius;
            _height = height;
        }

        public Vector3 Evaluate(Vector3 position, float delta)
        {
            var pos = CalculateAcceleration(position, delta);

            pos += _acceleration * delta;

            return pos;
        }

        public Vector3 Evaluate(Vector2 direction, Vector3 position, float delta)
        {
            var pos = CalculateAcceleration(position, delta);

            pos += (new Vector3(direction.x, 0, direction.y) + _acceleration) * delta;

            return pos;
        }

        private Vector3 CalculateAcceleration(Vector3 position, float delta)
        {
            _acceleration += _settings.gravity * delta;

            var pos = position;

            if (_useSphere)
            {
                CheckGroundSphere(_radius, _height + 0.1f, 0.1f, pos, _acceleration, out var temp);
                if ((temp - pos).magnitude > 0.05f)
                    pos = temp;
            }
            else
                CheckGroundRay(pos, _acceleration);

            if (_grounded)
            {
                _acceleration.y = 0;
                _stepsSinceGrounded = 0;
            }

            _stepsSinceGrounded++;
            return pos;
        }

        public void Jump(float statsJumpForce)
        {
            if (!_grounded && _stepsSinceGrounded > 4) return;

            _acceleration.y = statsJumpForce;
            _grounded = false;
            _jumping = true;
        }

        public void StopJump()
        {
            _jumping = false;
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

        private void CheckGroundSphere(float radius, float height, float distance, Vector3 position, Vector3 direction, out Vector3 hitPosition)
        {
            var pos = position + Vector3.up * height;
            hitPosition = position;
            if (UnityEngine.Physics.SphereCast(new Ray(pos, direction), radius, out var hit, distance, _settings.colliders))
            {
                //DrawCross(pos, radius, Color.green);
                //DrawCross(pos + direction, radius, Color.green);
                
                hitPosition = hit.point;
                
                _grounded = true;
                _groundNormal = hit.normal;
                return;
            }

            _grounded = false;
        }

        private void DrawCross(Vector3 pos, float radius, Color color)
        {
            Debug.DrawRay(pos, Vector3.right * radius, color);
            Debug.DrawRay(pos, -Vector3.right * radius, color);

            Debug.DrawRay(pos, Vector3.up * radius, color);
            Debug.DrawRay(pos, -Vector3.up * radius, color);

            Debug.DrawRay(pos, Vector3.forward * radius, color);
            Debug.DrawRay(pos, -Vector3.forward * radius, color);
        }
    }
}