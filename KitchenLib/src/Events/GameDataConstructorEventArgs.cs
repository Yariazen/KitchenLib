using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenLib.Events
{
    public class GameDataConstructorEventArgs : EventArgs
    {
        public IReadOnlyDictionary<int, Appliance> appliances;
        public IReadOnlyDictionary<int, Item> items;
        public IReadOnlyDictionary<string, Process> processes;

        internal GameDataConstructorEventArgs(IReadOnlyDictionary<int, Appliance> appliances, IReadOnlyDictionary<int, Item> items, IReadOnlyDictionary<string, Process> processes)
        {
            this.appliances = appliances;
            this.items = items;
            this.processes = processes;
        }
    }
}
