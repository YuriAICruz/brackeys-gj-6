namespace System.Input
{
    [Serializable]
    public class InputMap
    {
        public Mapping[] inputs;
        public Axis[] axes;

        public int rtId;
        
        public int ltId;
    }
}