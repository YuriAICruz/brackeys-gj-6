using UnityEngine;

namespace System.Input
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "Graphene/Brackeys-GJ/InputSettings")]
    public class InputSettings : ScriptableObject
    {
        public InputMap inputMap;

        public void Save()
        {
            DataManager.Save(inputMap, "Inputs");
        }

        public void Load(string file)
        {
            var input = DataManager.Load<InputMap>(file);
            
            if(input == null)
                return;

            inputMap = input;
        }
    }
}