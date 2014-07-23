#region

using System;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    public class RandomDo : Behavior
    {
        private readonly int percent;
        private readonly Behavior result;

        public RandomDo(int percent, Behavior result)
        {
            this.result = result;
            this.percent = percent;
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            if (new Random().Next(1, 100) <= percent)
                return result.Tick(Host, time);

            return false;
        }
    }
}