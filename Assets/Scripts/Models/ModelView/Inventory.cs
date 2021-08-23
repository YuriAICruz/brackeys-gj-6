using System;
using System.Collections.Generic;
using Models.Interfaces;

namespace Models.ModelView
{
    [Serializable]
    public class Inventory
    {
        public IWeapon weapon;
        public int consumablesLimit = 3;
        public List<IConsumable> consumables = new List<IConsumable>();
    }
}