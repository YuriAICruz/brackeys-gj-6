using System.Collections.Generic;
using Models.Signals;
using ModestTree;
using UnityEngine;

namespace System.Sound
{
    [CreateAssetMenu(fileName = "BgmSettings", menuName = "Graphene/Brackeys-GJ/BgmSettings")]
    public class BgmSettings :ScriptableObject
    {
        public AudioClip[] clips;
        public Bgm.Clips[] key;
        
        public AudioClip Clip(Bgm.Clips clip)
        {
            return clips[key.IndexOf(clip)];
        }

        private void OnValidate()
        {
            if(key.Length > 0 && clips.Length > 0 && clips.Length == key.Length) return;

            var e = Enum.GetValues(typeof(Bgm.Clips));
            
            var k = new List<AudioClip>(clips);
            
            key = new Bgm.Clips[e.Length];

            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (Bgm.Clips) e.GetValue(i);
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