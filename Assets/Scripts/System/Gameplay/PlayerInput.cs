using System.Linq;
using UnityEngine;
using Zenject;

namespace System.Gameplay
{
    public class PlayerInput : IInput, ITickable
    {
        private SignalBus _signalBus;
        private InputSettings _settings;

        public InputSignal.Axes _axes;

        public class buttonInfo
        {
            public bool down;
            public float time;
        }

        public buttonInfo[] _buttons;

        private readonly int _axesCount = 4;

        public PlayerInput(SignalBus signalBus, InputSettings settings)
        {
            _signalBus = signalBus;
            _settings = settings;
            _axes = new InputSignal.Axes(_axesCount);
            _buttons = new buttonInfo[_settings.inputMap.inputs.Max(x => x.id) + 1];
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i] = new buttonInfo();
            }
        }

        public void Tick()
        {
            for (int i = 0, n = _settings.inputMap.inputs.Length; i < n; i++)
            {
                if (!_buttons[_settings.inputMap.inputs[i].id].down &&
                    Input.GetKeyDown(_settings.inputMap.inputs[i].key))
                {
                    _buttons[_settings.inputMap.inputs[i].id].down = true;
                    _buttons[_settings.inputMap.inputs[i].id].time = Time.time;
                    _signalBus.Fire(new InputSignal.Down(_settings.inputMap.inputs[i].id));
                }

                if (_buttons[_settings.inputMap.inputs[i].id].down && Input.GetKeyUp(_settings.inputMap.inputs[i].key))
                {
                    _buttons[_settings.inputMap.inputs[i].id].down = false;
                    _signalBus.Fire(new InputSignal.Up(_settings.inputMap.inputs[i].id,
                        Time.time - _buttons[_settings.inputMap.inputs[i].id].time));
                }
            }

            _axes.Clear();
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