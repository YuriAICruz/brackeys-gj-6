namespace Graphene.Time
{
    public class Timer
    {
        private static ITimeManager _time;

        public delegate bool Feedback();

        public Timer(ITimeManager time)
        {
            _time = time;
        }

        public static float deltaTime => _time.DeltaTime;
        public static float time => _time.Time;
        public static float fixedTime => _time.FixedTime;
        public static float fixedDeltaTime => _time.FixedDeltaTime;
        public static bool paused => _time.IsPaused;
    }
}