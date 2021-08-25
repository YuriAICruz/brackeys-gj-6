using System;
using UnityEngine;

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
        public bool anticipating;
        public float anticipatingElapsed;
        public bool moving;
        public Vector3 playerDirection;
        public Vector3 destination;
        public float movingElapsed;
        public bool spited;
        public bool backingUp;
    }
}