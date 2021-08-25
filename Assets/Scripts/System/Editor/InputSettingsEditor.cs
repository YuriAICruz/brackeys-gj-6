using System.Gameplay;
using System.Input;
using UnityEditor;
using UnityEngine;

namespace System.Editor
{
    [CustomEditor(typeof(InputSettings))]
    public class InputSettingsEditor : UnityEditor.Editor
    {
        private InputSettings _self;

        private void Awake()
        {
            _self = target as InputSettings;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save"))
            {
                _self.Save();
            }

            if (GUILayout.Button("Load"))
            {
                Debug.Log("Loading \"Inputs_Override\" file");
                _self.Load("Inputs_Override");
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}