using Models.ModelView;

namespace Models.Signals
{
    public class Actor
    {
        public class Attack
        {
            public int id;
            public AttackAnimation data;

            public Attack(int id, AttackAnimation data)
            {
                this.id = id;
                this.data = data;
            }
        }

        public class Damage
        {
            public int damage;
            public int hp;

            public Damage(int damage, int hp)
            {
                this.damage = damage;
                this.hp = hp;
            }
        }
    }
}