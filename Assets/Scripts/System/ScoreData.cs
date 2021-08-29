using Models.Accessors;

namespace System
{
    public class ScoreData : IScore, ICombo
    {
        public Observer<int> Score { get; } = new Observer<int>();
        public Observer<int> Combo { get; } = new Observer<int>();
        public Observer<int> MaxCombo { get; } = new Observer<int>();
    }
}