namespace Models.Signals
{
    public class Bgm
    {
        public enum Clips
        {
            MainBoss = 0
        }
        public class Play
        {
            public Clips Clip;

            public Play(Clips clip)
            {
                this.Clip = clip;
            }
        }
        public class Stop
        {
            
        }
    }
}