using UnityEngine;

namespace System.Gameplay
{
    public interface IPhysics
    {
        bool Grounded { get; }
        
        Vector3 Evaluate(Vector3 position, float delta);
        Vector3 Evaluate(Vector2 direction, Vector3 position, float delta);
    }
}