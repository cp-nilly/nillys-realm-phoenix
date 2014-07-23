#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class DoMultiple : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, Behavior>, DoMultiple> instances =
            new Dictionary<Tuple<int, int, Behavior>, DoMultiple>();

        private readonly Behavior behavior;
        private readonly int cooldown;
        private readonly int doTimes;
        private int irrelevantnumber = 1;
        private Random rand = new Random();
        private int timesDone;

        public DoMultiple(int doTimes, int cooldown, Behavior behavior)
        {
            this.doTimes = doTimes;
            this.cooldown = cooldown;
            this.behavior = behavior;
        }

        public static DoMultiple Instance(int doTimes, int cooldown, Behavior behavior)
        {
            var key = new Tuple<int, int, Behavior>(doTimes, cooldown, behavior);
            DoMultiple ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new DoMultiple(doTimes, cooldown, behavior);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var chr = Host as Character;
            var w = RealmManager.GetWorld(Host.Self.Owner.Id);

            while (doTimes > timesDone)
            {
                w.Timers.Add(new WorldTimer(cooldown, (world, t) =>
                {
                    try
                    {
                        timesDone++;
                    }
                    catch
                    {
                        timesDone++;
                    }
                }));


                return behavior.Tick(Host, time);
            }

            return false;
        }
    }
}