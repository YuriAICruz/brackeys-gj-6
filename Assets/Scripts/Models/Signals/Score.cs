namespace Models.Signals
{
    public class Score
    {
        public class ComboChange
        {
            public int combo;

            public ComboChange(int combo)
            {
                this.combo = combo;
            }
        }
        
        public class ComboUpdate
        {
            public float normalized;
            public float elapsed;
            public float remaining;

            public ComboUpdate(float normalized, float elapsed, float remaining)
            {
                this.normalized = normalized;
                this.elapsed = elapsed;
                this.remaining = remaining;
            }
        }
        
        public class ScoreChange
        {
            public int score;

            public ScoreChange(int score)
            {
                this.score = score;
            }
        }
    }
}