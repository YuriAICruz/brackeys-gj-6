using System;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.UI
{
    [RequireComponent(typeof(Text))]
    public class TextViewer : MonoBehaviour
    {
        private Text _tx;

        protected virtual void Awake()
        {
            _tx = GetComponent<Text>();
        }

        protected virtual void OnDestroy()
        {
        }

        public void SetText(string text)
        {
            _tx.text = text;
        }
        public void AnimateTextChangeText(string text)
        {
            _tx.text = text;
        }
    }
}