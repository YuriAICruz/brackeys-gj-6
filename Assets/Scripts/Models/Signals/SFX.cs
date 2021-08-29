using UnityEngine;

namespace Models.Signals
{
    public class SFX
    {
        public enum Clips
        {
            Slash = 0,
            Hit = 1,
            PlayerHit = 2,
            Bite = 3,
            Anticipation = 4,
            Spit = 5,
            EatPillow = 6,
            Swing = 7,
            StepL = 8,
            StepR = 9,
            Jump = 10,
            Dodge = 11
        }
        public class Play
        {
            public Clips Clip;
            public float delay;
            public Vector3 position;

            public Play(Clips clip, Vector3 position, float delay = 0)
            {
                this.Clip = clip;
                this.delay = delay;
                this.position = position;
            }
        }
    }
}