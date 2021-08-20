using System;
using System.Gameplay;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class InputDebug : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;

        private void Awake()
        {
            _signalBus.Subscribe<InputSignal.Up>(Log);
            _signalBus.Subscribe<InputSignal.Down>(Log);
            _signalBus.Subscribe<InputSignal.Axes>(Log);
        }

        private void Log(InputSignal.Up data)
        {
            Debug.Log($"{data.id} Up {data.duration}");
        }

        private void Log(InputSignal.Down data)
        {
            Debug.Log(data.id + " Down");
        }

        private void Log(InputSignal.Axes data)
        {
            var sum = 0f;
            for (int i = 0; i < data.values.Length; i++)
            {
                sum += Mathf.Abs(data.values[i]);
            }

            if (sum <= 0) return;

            for (int i = 0; i < data.values.Length; i++)
            {
                Debug.Log($"Axis[{i}]: {data.values[i]}");
            }
        }
    }
}