using UnityEngine;

namespace System.Gameplay
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Graphene/Brackeys-GJ/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public float restartDelay;
        public float startDelay;

        public float healDuration;
        public float comboDelay;
    }
}