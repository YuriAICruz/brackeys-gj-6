using UnityEngine;
using Zenject;

namespace System.Gameplay
{
    public class PlayerInput : IInput, ITickable
    {
        private SignalBus _signalBus;
        private InputSettings _settings;

        public InputSignal.Axes _axes;

        private int _axesCount;

        public PlayerInput(SignalBus signalBus, InputSettings settings)
        {
            _signalBus = signalBus;
            _settings = settings;
            _axes = new InputSignal.Axes();
        }

        public void Tick()
        {
            for (int i = 0, n = _settings.inputMap.inputs.Length; i < n; i++)
            {
                if (Input.GetKeyDown(_settings.inputMap.inputs[i].key))
                {
                    _signalBus.Fire(new InputSignal.Down(_settings.inputMap.inputs[i].id));
                }

                if (Input.GetKeyUp(_settings.inputMap.inputs[i].key))
                {
                    _signalBus.Fire(new InputSignal.Up(_settings.inputMap.inputs[i].id));
                }
            }
            
            _axes.Clear(_axesCount);
            for (int i = 0, n = _settings.inputMap.axes.Length; i < n; i++)
            {
                var index = _settings.inputMap.axes[i].id;

                int value = Input.GetKey(_settings.inputMap.axes[i].positive) ? 1 :
                    Input.GetKey(_settings.inputMap.axes[i].negative) ? -1 : 0;

                if (value != 0)
                    _axes.values[index] = value;
            }

            for (int i = 0; i < _axesCount; i++)
            {
                var value = Input.GetAxisRaw($"Axis_{i}");
                
                if (value != 0)
                    _axes.values[i] = value;
            }

            _signalBus.Fire(_axes);
        }
    }
}