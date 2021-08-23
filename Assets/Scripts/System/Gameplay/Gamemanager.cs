using Models.Accessors;
using Models.Signals;

namespace System.Gameplay
{
    public class GameManager
    {
        private IActorData player;
        private IActorData boss;
        
        public IActorData Player => (IActorData) player;
        public IActorData Boss => (IActorData) boss;

        public void SetActors(IActorData player, IActorData boss)
        {
            this.player = player;
            this.boss = boss;
        }
    }
}