#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class SpawnMinion : Behavior
    {
        private static readonly Dictionary<Tuple<short, float, int, int, int>, SpawnMinion> instances =
            new Dictionary<Tuple<short, float, int, int, int>, SpawnMinion>();

        private readonly int maxCount;
        private readonly int maxTick;
        private readonly int minTick;
        private readonly short objType;
        private readonly float radius;
        private readonly Random rand = new Random();

        private SpawnMinion(short objType, float radius, int maxCount, int minTick, int maxTick)
        {
            this.objType = objType;
            this.radius = radius;
            this.maxCount = maxCount;
            this.minTick = minTick;
            this.maxTick = maxTick;
        }

        public static SpawnMinion Instance(short objType, float radius, int maxCount, int minTick, int maxTick)
        {
            var key = new Tuple<short, float, int, int, int>(objType, radius, maxCount, minTick, maxTick);
            SpawnMinion ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new SpawnMinion(objType, radius, maxCount, minTick, maxTick);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                remainingTick = 0;
            else
                remainingTick = (int) o;

            remainingTick -= time.thisTickTimes;
            bool ret;
            if (remainingTick <= 0)
            {
                if (CountEntity(radius, objType) < maxCount)
                {
                    var entity = Entity.Resolve(objType);
                    entity.Move(Host.Self.X, Host.Self.Y);
                    (entity as Enemy).Terrain = (Host as Enemy).Terrain;
                    Host.Self.Owner.EnterWorld(entity);
                }

                remainingTick = rand.Next(minTick, maxTick);
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }
}