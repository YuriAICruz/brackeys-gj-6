using System;

namespace Models.Interfaces
{
    public enum InteractionType
    {
        Heal = 0
    }
    
    public interface IInteractable
    {
        bool CanActivate { get; }
        
        InteractionType EffectType { get; }
        
        void Activate(Action onEnd);
        void Cancel();
    }
}