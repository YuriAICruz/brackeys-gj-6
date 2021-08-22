using System;
using UnityEngine;

namespace Graphene.Time
{
    public interface ITimeManager
    {
        float DeltaTime { get; }
        float Time { get; }
        float FixedTime { get; }
        float FixedDeltaTime { get; }
        bool IsPaused { get; }
        
        event Action UnPaused;
        event Action Paused;
        
        void Pause();
        void Resume();
        void Tick();
        void LateTick();
        void SetFixedTime(float time);
        Coroutine Wait(float f, Action callback);
        Coroutine Wait(Timer.Feedback feedback, Action callback);
        Coroutine Wait(float f, Action<float> update, Action onEnd);
        void Stop(Coroutine waiter);
    }
}