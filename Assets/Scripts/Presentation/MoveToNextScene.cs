using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation
{
    public class MoveToNextScene : MonoBehaviour
    {
        public int levelIndex;
        public float delay;

        private void Start()
        {
            Invoke(nameof(ChangeScene), delay);
        }

        void ChangeScene()
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}