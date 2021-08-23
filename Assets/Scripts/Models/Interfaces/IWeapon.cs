namespace Models.Interfaces
{
    public interface IWeapon
    {
        IActor Owner { get; }

        void Discard();
    }
}