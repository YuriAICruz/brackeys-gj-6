using UnityEngine;

namespace System.Gameplay
{
    public interface IPhysics
    {
        bool Grounded { get; }
        
        void SetSphereRadius(float radius, float height);
        
        Vector3 Evaluate(Vector3 position, float delta);
        Vector3 Evaluate(Vector2 direction, Vector3 position, float delta);
        
        void Jump(float statsJumpForce);
        void StopJump();
        Vector3 Drop(Vector3 position, float duration);
    }
}