namespace Models.Interfaces
{
    public interface IDamageable
    {
        int Hp { get; }
        void Damage(int damage);
    }
}