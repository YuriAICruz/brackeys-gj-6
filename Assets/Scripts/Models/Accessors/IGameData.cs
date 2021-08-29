using System;

namespace Models.Accessors
{
    public interface IGameData
    {
        Observer<float> Time { get; }
        Observer<int> Grade { get; }
        Observer<int> Hits { get; }
        Observer<int> Damages { get; }
        Observer<int> Heals { get; }
        Observer<int> Deaths { get; }
    }
}