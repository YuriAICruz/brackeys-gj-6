using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public class Observer<T>
    {
        private T _value;
        private bool _committed;

        private event Action<T> _listeners;

        private event Action _eventListeners;

        public ref readonly T GetValue()
        {
            return ref _value;
        }

        public void Commit(T value)
        {
            _value = value;

            _committed = !IsNull();
            DispatchChangeEvent();
        }

        private void DispatchChangeEvent()
        {
            _eventListeners?.Invoke();
            _listeners?.Invoke(GetValue());
        }

        public void AddEventListener(Action listener)
        {
            _eventListeners += listener;

            if (_committed)
                listener?.Invoke();
        }

        public void AddListener(Action<T> listener)
        {
            _listeners += listener;

            if (_committed)
                listener?.Invoke(GetValue());
        }

        public void RemoveListener(Action<T> listener)
        {
            try
            {
                _listeners -= listener;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void RemoveEventListener(Action listener)
        {
            try
            {
                _eventListeners -= listener;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public bool IsNull()
        {
            return _value == null;
        }

        public bool TryAdd<TU>(TU item)
        {
            var list = _value as List<TU>;
            if (list == null) return false;
            
            list.Add(item);
            DispatchChangeEvent();
            return true;
        }

        public bool TryRemove<TU>(int index)
        {
            var list = _value as List<TU>;
            if (list == null) return false;
            
            list.RemoveAt(index);
            DispatchChangeEvent();
            return true;
        }
    }
}