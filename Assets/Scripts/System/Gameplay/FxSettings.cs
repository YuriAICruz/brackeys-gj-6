using UnityEngine;

namespace System.Gameplay
{
    [CreateAssetMenu(fileName = "FxSettings", menuName = "Graphene/Brackeys-GJ/FxSettings")]
    public class FxSettings : ScriptableObject
    {
        public GameObject puff;
        public GameObject slash;
        public GameObject hit;
        public GameObject smokeScreen;
    }
}