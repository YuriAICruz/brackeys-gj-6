using System;
using System.Collections;
using UnityEngine;

namespace Graphene.Time
{
    public class TimeManagerBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public Coroutine Wait(float f, Action callback)
        {
            return StartCoroutine(WaitRoutine(f, callback));
        }

        public Coroutine Wait(float f, Action<float> update, Action onEnd)
        {
            return StartCoroutine(WaitRoutine(f, update, onEnd));
        }

        IEnumerator WaitRoutine(float f, Action callback)
        {
            yield return new WaitForSeconds(f);
            callback?.Invoke();
        }

        public Coroutine Wait(Timer.Feedback feedback, Action callback)
        {
            return StartCoroutine(WaitRoutine(feedback, callback));
        }

        IEnumerator WaitRoutine(float duration, Action<float> update, Action onEnd)
        {
            var t = 0f;
            while (t <= duration)
            {
                t += Timer.deltaTime;
                update?.Invoke(t/duration);
                yield return null;
            }
            onEnd?.Invoke();
        }
        
        IEnumerator WaitRoutine(Timer.Feedback feedback, Action callback)
        {
            yield return new WaitWhile(()=>!feedback());
            callback?.Invoke();
        }

        public void Stop(Coroutine routine)
        {
            StopCoroutine(routine);
        }
    }
}