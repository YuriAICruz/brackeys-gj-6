using System;
using System.Collections.Generic;
using System.Linq;
using Midiadub.EasyEase;
using UnityEngine;
using Zenject;

namespace Graphene.Time
{
    public class TimeManager : ITimeManager, ITickable, ILateTickable, IInitializable
    {
        public event Action UnPaused;
        public event Action Paused;

        private static float _scale = 1;
        private static bool _paused;

        private static float _time = 0;
        private static float _fixedTime = 0;

        public float DeltaTime => UnityEngine.Time.deltaTime * _scale;
        public float Time => _time;
        public float FixedTime => _fixedTime;
        public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime * _scale;
        public bool IsPaused => _paused;

        private Queue<Action> _waiterQueue = new Queue<Action>();
        private TimeManagerBehaviour _timer;


        public void Initialize()
        {
            _timer = new GameObject("TimeManagerBehaviour", new[] {typeof(TimeManagerBehaviour)}).GetComponent<TimeManagerBehaviour>();
        }

        public void Pause()
        {
            if (_paused) return;

#if DEV_MODE
            Debug.Log("Paused");
#endif

            _paused = true;
            EaseEasy.Animate((t) => { _scale = t; }, 1, 0, 0.4f, 0, EaseTypes.Linear, null, () => { _scale = 0; });

            Paused?.Invoke();
        }

        public void Resume()
        {
            if (!_paused) return;

#if DEV_MODE
            Debug.Log("Resume");
#endif

            _paused = false;
            EaseEasy.Animate((t) => { _scale = t; }, 0, 1, 0.4f, 0, EaseTypes.Linear, null, () => { _scale = 1; });

            UnPaused?.Invoke();
        }

        public void Tick()
        {
            _time += Timer.deltaTime;

            if (_waiterQueue.Any())
                _waiterQueue.Dequeue()?.Invoke();
        }

        public void LateTick()
        {
            _fixedTime += Timer.fixedDeltaTime;
        }

        public Coroutine Wait(float f, Action callback)
        {
            return _timer.Wait(f, callback);
        }
        
        public Coroutine Wait(float f, Action<float> update, Action onEnd)
        {
            return _timer.Wait(f, update, onEnd);
        }

        public Coroutine Wait(Timer.Feedback feedback, Action callback)
        {
            return _timer.Wait(feedback,  callback);
        }

        public void Stop(Coroutine routine)
        {
            _timer.Stop(routine);
        }

        public void SetFixedTime(float time)
        {
            _fixedTime = time;
        }
    }
}