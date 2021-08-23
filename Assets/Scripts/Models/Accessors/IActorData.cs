using System;
using Models.ModelView;

namespace Models.Accessors
{
    public interface IActorData
    {
        Observer<int> Hp { get; }
    }
}