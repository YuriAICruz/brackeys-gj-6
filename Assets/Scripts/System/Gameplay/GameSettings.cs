using UnityEngine;

namespace System.Gameplay
{
    [Serializable]
    public class ComboFeedback
    {
        public string feedback;
        public int mark;
    }

    [CreateAssetMenu(fileName = "GameSettings", menuName = "Graphene/Brackeys-GJ/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public float restartDelay;
        public float startDelay;

        public float healDuration;
        public float comboDelay;
        public int scoreBase;
        public float comboMultiplier;
        public ComboFeedback[] comboFeedbacks;

        [Header("Grade")] public string[] grades;
        public int gradeDamageCap;
        public int gradeHitsCap;
        public int comboHitsCap;
    }
}