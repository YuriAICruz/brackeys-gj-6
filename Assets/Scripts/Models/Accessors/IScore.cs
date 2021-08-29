using System;

namespace Models.Accessors
{
    public interface IScore
    {
        Observer<int> Score { get; }
    }
}