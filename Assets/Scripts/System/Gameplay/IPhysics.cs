using UnityEngine;

namespace System.Gameplay
{
    public interface IPhysics
    {
        Vector3 Evaluate(Vector3 position, float delta);
        Vector3 Evaluate(Vector2 direction, Vector3 position, float delta);
    }
}