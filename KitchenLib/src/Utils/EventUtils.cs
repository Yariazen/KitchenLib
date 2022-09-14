﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenLib.Utils
{
    public class EventUtils
    {
        public static void InvokeEvent(string name, IEnumerable<Delegate> handlers, object sender)
        {
            if (handlers == null)
                return;

            var args = new EventArgs();
            foreach (EventHandler handler in handlers.Cast<EventHandler>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch (Exception e)
                {
                    Mod.Error($"Exception while handling event {name}:\n{e}");
                }
            }
        }

        public static void InvokeEvent<T> (string name, IEnumerable<Delegate> handlers, object sender, T args) where T : EventArgs
        {
            if (handlers == null)
                return;

            foreach (EventHandler<T> handler in handlers.Cast<EventHandler<T>>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch(Exception e)
                {
                    Mod.Error($"Exception while handling event {name}:\n{e}");
                }
            }
        }
    }
}
