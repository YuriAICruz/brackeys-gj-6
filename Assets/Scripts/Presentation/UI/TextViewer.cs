using System;
using Graphene.Time;
using Midiadub.EasyEase;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    [RequireComponent(typeof(Text))]
    public class TextViewer : MonoBehaviour
    {
        [Inject] private ITimeManager _timeManager;

        public float duration = 0.4f;
        public float size;
        public AnimationCurve curve;
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
            EaseEasy.Animate(t => { transform.localScale = Vector3.one * t; }, size, 1, duration, 0,
                EaseTypes.AnimationCurve, curve);
        }
    }
}