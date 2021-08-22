using System;
using UnityEngine;

namespace Presentation.Effects
{
    public class DestroyOnContact : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            var breakable = other.transform.GetComponent<IBreakable>();
            
            if(breakable == null) return;

            breakable.Break();
        }

        private void OnTriggerEnter(Collider other)
        {
            var breakable = other.transform.GetComponent<IBreakable>();
            
            if(breakable == null) return;

            breakable.Break();
        }
    }
}