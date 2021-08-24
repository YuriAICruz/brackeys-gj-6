using System;
using Models.ModelView;
using UnityEngine;

namespace Models.Accessors
{
    public interface IActorData
    {
        Transform Transform { get; }
        Vector3 Position { get; }
        
        Observer<int> Hp { get; }
    }
}