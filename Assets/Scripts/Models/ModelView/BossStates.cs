using System;

namespace Models.ModelView
{
    [Serializable]
    public class BossStates
    {
        public int stage;
        public bool spiting;
        public float spitingElapsed;
        public bool stagged;
        public float stagElapsed;
    }
}