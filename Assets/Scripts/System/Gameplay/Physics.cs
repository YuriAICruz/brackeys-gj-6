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
        private float _radius = 0.5f;
        private bool _useSphere;
        private float _height;
        private Vector3 _lastPosition;
        private Collider[] _colisions;

        public bool Grounded => _grounded;

        public Physics(PhysicsSettings settings)
        {
            _settings = settings;

            _colisions = new Collider[_settings.maxCollision];
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

            CheckCollision(_radius, ref pos, ref _lastPosition);

            return pos;
        }

        private Vector3 CalculateAcceleration(Vector3 position, float delta)
        {
            if (!_jumping)
                _acceleration += _settings.gravity * delta;

            var pos = position;

            if (_useSphere)
            {
                CheckGroundSphere(_radius, pos, _lastPosition, _settings.gravity.normalized, out var temp);
                if ((temp - pos).magnitude > 0.05f && Mathf.Abs(pos.x - temp.x) < 0.05f &&
                    Mathf.Abs(pos.z - temp.z) < 0.05f)
                    pos.y = temp.y;
            }
            else
                CheckGroundRay(pos, _settings.gravity.normalized);

            if (!_jumping && _grounded)
            {
                _acceleration.y = 0;
                _stepsSinceGrounded = 0;
            }

            _stepsSinceGrounded++;
            _lastPosition = pos;

            return pos;
        }

        public void Jump(float statsJumpForce)
        {
            if (!_grounded && _stepsSinceGrounded > 4 || _jumping) return;

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

        public bool CheckSphere(Vector3 position, Vector3 direction, float radius, LayerMask mask, out RaycastHit hit)
        {
            return UnityEngine.Physics.SphereCast(new Ray(position, direction), radius, out hit, direction.magnitude,
                mask);
        }

        private void CheckGroundSphere(float radius, Vector3 position, Vector3 lastPosition, Vector3 direction,
            out Vector3 hitPosition)
        {
            var offset = 0.5f;
            var pos = position + Vector3.up * (radius + offset * 0.5f);
            var lastPos = lastPosition + Vector3.up * (radius + offset * 0.5f);

            var dir = new Vector3(0, pos.y, 0) - new Vector3(0, lastPos.y, 0);

            hitPosition = position;

            if (dir.magnitude < 0.05f)
            {
                dir = direction * offset;
            }

            if (CheckSphere(lastPos, dir, radius, _settings.colliders, out var hit))
            {
                // DrawCross(lastPos, radius, new Color(0.8f, 1f, 0));
                // DrawCross(lastPos + dir, radius, new Color(0.2f, 1f, 0));

                hitPosition = hit.point;
                _groundNormal = hit.normal;
                _grounded = true;
                return;
            }

            // DrawCross(lastPos, radius, new Color(1, 0.4f, 0));
            // DrawCross(lastPos + dir, radius, new Color(1, 0.8f, 0));

            _grounded = false;
        }

        private void CheckCollision(float radius, ref Vector3 position, ref Vector3 lastPosition)
        {
            var pos = position + Vector3.up * radius;

            var offset = 0.0f;

            var colliders = UnityEngine.Physics.OverlapSphereNonAlloc(pos, radius, _colisions, _settings.colliders);

            var pi = Mathf.PI * 2;
            var newPos = position;
            for (int i = 0; i < colliders; i++)
            {
                pos = newPos + Vector3.up * radius;
                var point = _colisions[i].ClosestPoint(pos);

                if (Mathf.Abs(point.y - pos.y) > 0.1f)
                {
                    Debug.DrawLine(pos, point, Color.green);
                    continue;
                }

                var dir = pos - point;
                dir.y = 0;

                Debug.DrawLine(position, new Vector3(point.x, newPos.y, point.z) + dir.normalized * (radius + offset),
                    Color.red);

                newPos = new Vector3(point.x, position.y, point.z) + dir.normalized * (radius + offset);
            }

            Debug.DrawLine(position, newPos, Color.magenta);

            position = newPos;
            lastPosition = position;
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