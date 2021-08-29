using UnityEngine;

namespace Models.Signals
{
    public class FX
    {
        public class Slash
        {
            public Vector3 position;

            public Slash(Vector3 position)
            {
                this.position = position;
            }
        }

        public class Hit
        {
            public Vector3 position;

            public Hit(Vector3 position)
            {
                this.position = position;
            }
        }

        public class Smoke
        {
            public Vector3 position;

            public Smoke(Vector3 position)
            {
                this.position = position;
            }
        }

        public class Puff
        {
            public Vector3 position;

            public Puff(Vector3 position)
            {
                this.position = position;
            }
        }
    }
}