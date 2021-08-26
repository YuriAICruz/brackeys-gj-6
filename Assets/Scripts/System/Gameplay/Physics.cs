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
        private float _slope;
        private RaycastHit[] _hitCollisions;

        public bool Grounded => _grounded;

        public Physics(PhysicsSettings settings)
        {
            _settings = settings;

            _colisions = new Collider[_settings.maxCollision];
            _hitCollisions = new RaycastHit[_settings.maxCollision];
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

            if (pos.y < _settings.yDeath)
                pos.y = _settings.yRespawn;

            return pos;
        }

        private Vector3 CalculateAcceleration(Vector3 position, float delta)
        {
            if (!_jumping)
                _acceleration += _settings.gravity * delta;

            var pos = position;

            if (CheckUpCollision(pos, _radius))
            {
                _acceleration.y = Mathf.Min(0, _acceleration.y);
            }

            if (_useSphere)
            {
                CheckGroundSphere(_radius, pos, _lastPosition, _settings.gravity.normalized, out var temp);
                if (!_jumping && (temp - pos).magnitude > 0.05f && Mathf.Abs(pos.x - temp.x) < 0.25f &&
                    Mathf.Abs(pos.z - temp.z) < 0.25f)
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

        private bool CheckUpCollision(Vector3 position, float radius)
        {
            Debug.DrawLine(position + Vector3.up * radius, Vector3.up * radius, Color.blue, 1);
            return UnityEngine.Physics.Raycast(new Ray(position + Vector3.up * radius, Vector3.up * radius));
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

        public Vector3 Drop(Vector3 position, float delta)
        {
            _jumping = false;
            _acceleration.y = Mathf.Min(_acceleration.y, 0);

            return Evaluate(position, delta);
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

        public bool CheckSphere(Vector3 position, Vector3 direction, float radius, LayerMask mask, out int hitsCount)
        {
            hitsCount = UnityEngine.Physics.SphereCastNonAlloc(new Ray(position, direction), radius, _hitCollisions,
                direction.magnitude,
                mask);

            for (int i = 0; i < hitsCount; i++)
            {
                var dir = _hitCollisions[i].collider.ClosestPoint(position) - position;
                if (UnityEngine.Physics.Raycast(new Ray(position, dir), out var hit, dir.magnitude * 5, mask))
                {
                    if (hit.collider == _hitCollisions[i].collider)
                        _hitCollisions[i] = hit;
                }
            }

            return hitsCount > 0;
        }

        private void CheckGroundSphere(float radius, Vector3 position, Vector3 lastPosition, Vector3 direction,
            out Vector3 hitPosition)
        {
            var offset = 0.25f;
            var pos = position + Vector3.up * (radius + offset * 0.5f);
            var lastPos = lastPosition + Vector3.up * (radius + offset * 0.5f);

            var dir = new Vector3(0, pos.y, 0) - new Vector3(0, lastPos.y, 0);

            hitPosition = position;

            if (dir.magnitude < 0.05f)
            {
                dir = direction * offset;
            }

            if (CheckSphere(lastPos, dir, radius, _settings.colliders, out int hitsCount))
            {
                // DrawCross(lastPos, radius, new Color(0.8f, 1f, 0));
                // DrawCross(lastPos + d
                // ir, radius, new Color(0.2f, 1f, 0));

                _grounded = false;
                _groundNormal = Vector3.zero;
                var c = 0;
                var height = Vector3.zero;

                for (int i = 0; i < hitsCount; i++)
                {
                    var hitDirection = _hitCollisions[i].point - lastPos;

                    if (_hitCollisions[i].point.magnitude == 0) continue;

                    // if (_hitCollisions[i].point.magnitude == 0)
                    //     hitDirection = _hitCollisions[i].collider.ClosestPoint(lastPos) - lastPos;

                    if (hitDirection.y > 0) continue;

                    c++;
                    var normal = _hitCollisions[i].normal;
                    _groundNormal += normal;

                    _slope = Vector3.Angle(Vector3.up, normal);

                    if (_slope > _settings.maxSlope)
                    {
                        continue;
                    }

                    if (_hitCollisions[i].point.y > height.y)
                        height = _hitCollisions[i].point;
                    _groundNormal = normal;

                    _grounded = true;
                    //break;
                }

                if (!_grounded)
                    _groundNormal /= c;


                hitPosition = height;

                //Debug.DrawRay(position, _groundNormal * 5, Color.blue);

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

        private Vector3 RepairHitSurfaceNormal(RaycastHit hit, int layerMask)
        {
            if (hit.collider is MeshCollider)
            {
                var collider = hit.collider as MeshCollider;
                var mesh = collider.sharedMesh;
                var tris = mesh.triangles;
                var verts = mesh.vertices;

                var v0 = verts[tris[hit.triangleIndex * 3]];
                var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
                var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

                var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;

                return hit.transform.TransformDirection(n);
            }
            else
            {
                var p = hit.point + hit.normal * 0.01f;
                UnityEngine.Physics.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
                return hit.normal;
            }
        }
    }
}