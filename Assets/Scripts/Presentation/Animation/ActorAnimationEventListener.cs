using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.Animation
{
    public class ActorAnimationEventListener : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;

        public void FootR()
        {
            _signalBus.Fire(new SFX.Play(SFX.Clips.StepR, transform.position));
        }

        public void FootL()
        {
            _signalBus.Fire(new SFX.Play(SFX.Clips.StepL, transform.position));
        }

        public void Hit()
        {
            
        }

        public void Land()
        {
            
        }
    }
}