using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation
{
    public class MoveToNextScene : MonoBehaviour
    {
        public int levelIndex;
        private void Start()
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}