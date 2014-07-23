#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class Planewalk : Behavior
    {
        private static readonly Dictionary<Tuple<float, short?>, Planewalk> instances =
            new Dictionary<Tuple<float, short?>, Planewalk>();

        private readonly short? objType;
        private readonly float radius;
        private Random rand = new Random();

        private Planewalk(float radius, short? objType)
        {
            this.radius = radius;
            this.objType = objType;
        }

        public static Planewalk Instance(float radius, short? objType)
        {
            var key = new Tuple<float, short?>(radius, objType);
            Planewalk ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Planewalk(radius, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;

            var dist = radius;
            var entity = GetNearestEntity(ref dist, objType);
            if (entity != null)
                ValidateAndMove(entity.X, entity.Y);
            return true;
        }
    }
}