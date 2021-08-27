using System;
using Presentation.Effects;
using UnityEngine;

namespace Presentation.Gameplay
{
    public class Table : MonoBehaviour
    {
        public MeshDestroy[] _meshDestroys;
        
        public void Break(Action onBreak)
        {
            Destroy(gameObject);
            onBreak?.Invoke();

            for (int i = 0; i < _meshDestroys.Length; i++)
            {
                _meshDestroys[i].Break();
            }
        }
    }
}