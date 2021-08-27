using System;
using UnityEngine;

namespace Presentation.Gameplay
{
    public class Table : MonoBehaviour
    {
        public void Break(Action onBreak)
        {
            Destroy(gameObject);
            onBreak?.Invoke();
        }
    }
}