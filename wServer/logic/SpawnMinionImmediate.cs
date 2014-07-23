#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class SpawnMinionImmediate : Behavior
    {
        private static readonly Dictionary<Tuple<short, float, int, int>, SpawnMinionImmediate> instances
            = new Dictionary<Tuple<short, float, int, int>, SpawnMinionImmediate>();

        private readonly int maxCount;
        private readonly int minCount;

        private readonly short objType;
        private readonly float radius;
        private readonly Random rand = new Random();

        private SpawnMinionImmediate(short objType, float radius, int minCount, int maxCount)
        {
            this.objType = objType;
            this.radius = radius;
            this.minCount = minCount;
            this.maxCount = maxCount;
        }

        public static SpawnMinionImmediate Instance(short objType, float radius, int minCount, int maxCount)
        {
            var key = new Tuple<short, float, int, int>(objType, radius, minCount, maxCount);
            SpawnMinionImmediate ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new SpawnMinionImmediate(objType, radius, minCount, maxCount);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var count = rand.Next(minCount, maxCount + 1);
            for (var i = 0; i < count; i++)
            {
                var entity = Entity.Resolve(objType);
                entity.Move(Host.Self.X + (float) (rand.NextDouble()*2 - 1)*radius,
                    Host.Self.Y + (float) (rand.NextDouble()*2 - 1)*radius);
                (entity as Enemy).Terrain = (Host as Enemy).Terrain;
                Host.Self.Owner.EnterWorld(entity);
            }
            return true;
        }
    }

    internal class SpawnRoot : Behavior
    {
        private static readonly Dictionary<Tuple<short, float, float>, SpawnRoot> instances
            = new Dictionary<Tuple<short, float, float>, SpawnRoot>();

        private readonly short objType;
        private readonly float radius;
        private readonly Random rand = new Random();
        private readonly float targetRadius;

        private SpawnRoot(short objType, float radius, float targetRadius)
        {
            this.objType = objType;
            this.radius = radius;
            this.targetRadius = targetRadius;
        }

        public static SpawnRoot Instance(short objType, float radius, float targetRadius)
        {
            var key = new Tuple<short, float, float>(objType, radius, targetRadius);
            SpawnRoot ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new SpawnRoot(objType, radius, targetRadius);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            foreach (var i in GetNearestEntities(radius, null))
            {
                if (!i.HasConditionEffect(ConditionEffects.Paralyzed))
                {
                    var entity = Entity.Resolve(objType);
                    entity.Move(i.X + (float) (rand.NextDouble()*2 - 1)*targetRadius,
                        i.Y + (float) (rand.NextDouble()*2 - 1)*targetRadius);
                    (entity as Enemy).Terrain = (Host as Enemy).Terrain;
                    Host.Self.Owner.EnterWorld(entity);
                }
            }
            return true;
        }
    }
}