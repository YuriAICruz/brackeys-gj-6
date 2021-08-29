namespace Models.Signals
{
    public class Player
    {
        public class Death
        {
        }

        public class SwitchWeapon
        {
            public int index;

            public SwitchWeapon(int index)
            {
                this.index = index;
            }
        }

        public class HitEnemy
        {
            public readonly int _damage;

            public HitEnemy(int damage)
            {
                _damage = damage;
            }
        }

        public class Hitted
        {
            public readonly int _damage;

            public Hitted(int damage)
            {
                _damage = damage;
            }
        }
    }
}