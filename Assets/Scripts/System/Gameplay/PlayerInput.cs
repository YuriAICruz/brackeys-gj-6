using System.Input;
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

        private bool _rt;
        private float _rtTime;

        private bool _lt;
        private float _ltTime;


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
                    UnityEngine.Input.GetKeyDown(_settings.inputMap.inputs[i].key))
                {
                    _buttons[_settings.inputMap.inputs[i].id].down = true;
                    _buttons[_settings.inputMap.inputs[i].id].time = Time.time;
                    _signalBus.Fire(new InputSignal.Down(_settings.inputMap.inputs[i].id));
                }

                if (_buttons[_settings.inputMap.inputs[i].id].down &&
                    UnityEngine.Input.GetKeyUp(_settings.inputMap.inputs[i].key))
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

                int value = UnityEngine.Input.GetKey(_settings.inputMap.axes[i].positive) ? 1 :
                    UnityEngine.Input.GetKey(_settings.inputMap.axes[i].negative) ? -1 : 0;

                if (value != 0)
                    _axes.values[index] = value;
            }

            for (int i = 0; i < _axesCount; i++)
            {
                var value = UnityEngine.Input.GetAxisRaw($"Axis_{i}");

                if (value != 0)
                    _axes.values[i] = value;
            }

            var r = UnityEngine.Input.GetAxisRaw($"Axis_4");
            var l = UnityEngine.Input.GetAxisRaw($"Axis_5");
            
            Debug.Log($"l {l} - r {r}");

            if (r > 0.5f != _rt)
            {
                _rt = r > 0.5f;
                if (_rt)
                {
                    Debug.Log("RtDown");
                    _signalBus.Fire(new InputSignal.Down(_settings.inputMap.rtId));
                    _rtTime = Time.time;
                }
                else
                {
                    Debug.Log("RtUp");
                    _signalBus.Fire(new InputSignal.Up(_settings.inputMap.rtId, Time.time - _rtTime));
                }
            }

            if (l > 0.5f != _lt)
            {
                _lt = l > 0.5f;
                if (_lt)
                {
                    Debug.Log("LtDown");
                    _signalBus.Fire(new InputSignal.Down(_settings.inputMap.ltId));
                    _rtTime = Time.time;
                }
                else
                {
                    Debug.Log("LtUp");
                    _signalBus.Fire(new InputSignal.Up(_settings.inputMap.ltId, Time.time - _ltTime));
                }
            }

            _signalBus.Fire(_axes);
        }
    }
}