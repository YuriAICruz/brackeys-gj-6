namespace System
{
    public interface IWeapon
    {
        IActor Owner { get; }

        void Swing(float duration);
        void Discard();
    }
}