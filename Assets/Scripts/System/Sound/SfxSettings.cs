using System.Collections.Generic;
using Models.Signals;
using ModestTree;
using UnityEngine;
using Zenject;

namespace System.Sound
{
    [CreateAssetMenu(fileName = "SfxSettings", menuName = "Graphene/Brackeys-GJ/SfxSettings")]
    public class SfxSettings :ScriptableObject
    {
        public AudioClip[] clips;
        public SFX.Clips[] key;
        
        public AudioClip Clip(SFX.Clips clip)
        {
            return clips[key.IndexOf(clip)];
        }

        private void OnValidate()
        {
            if(key.Length > 0 && clips.Length > 0 && clips.Length == key.Length) return;

            var e = Enum.GetValues(typeof(SFX.Clips));
            
            var k = new List<AudioClip>(clips);
            
            key = new SFX.Clips[e.Length];

            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (SFX.Clips) e.GetValue(i);
            }
            
            var c = new List<AudioClip>(clips);
            
            clips = new AudioClip[key.Length];

            for (int i = 0; i < c.Count; i++)
            {
                if(i >= clips.Length) break;
                
                clips[i] = c[i];
            }
        }
    }
}