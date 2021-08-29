using System;

namespace Models.Accessors
{
    public interface ICombo
    {
        Observer<int> Combo { get; }
        Observer<int> MaxCombo { get; }
    }

    public interface IHighScore
    {
        Observer<int> HighScore { get; }
    }
}