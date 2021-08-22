using System;
using UnityEngine;

namespace Models.Accessors
{
    public interface ICamera
    {
        Observer<Camera> ActiveCamera { get; }
    }
}