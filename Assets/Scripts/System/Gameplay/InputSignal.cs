namespace System.Gameplay
{
    public class InputSignal
    {
        public class Down
        {
            public int id;

            public Down(int id)
            {
                this.id = id;
            }
        }

        public class Up
        {
            public int id;
            public float duration;

            public Up(int id, float duration)
            {
                this.id = id;
                this.duration = duration;
            }
        }

        public class Axes
        {
            public float[] values;

            public Axes(int axesCount)
            {
                values = new float[axesCount];
            }

            public void Clear()
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = 0;
                }
            }
        }
    }
}