using System;

namespace Models.Accessors
{
    public interface ICombo
    {
        Observer<int> Combo { get; }
    }

    public interface IHighScore
    {
        Observer<int> HighScore { get; }
    }
}