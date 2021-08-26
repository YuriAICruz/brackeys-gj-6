namespace Models.Signals
{
    public class SFX
    {
        public enum Clips
        {
        }
        public class Play
        {
            public Clips Clip;

            public Play(Clips clip)
            {
                this.Clip = clip;
            }
        }
    }
}