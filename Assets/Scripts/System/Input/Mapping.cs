using UnityEngine;

namespace System.Gameplay
{
    [Serializable]
    public class Mapping
    {
        public KeyCode key;
        public int id;
    }
    
    [Serializable]
    public class Axis
    {
        public KeyCode positive;
        public KeyCode negative;
        public int id;
    }
}