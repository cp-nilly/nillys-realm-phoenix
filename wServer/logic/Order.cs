#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic
{
    internal class OrderGroup : Behavior
    {
        private static readonly Dictionary<Tuple<float, string, Behavior>, OrderGroup> instances =
            new Dictionary<Tuple<float, string, Behavior>, OrderGroup>();

        private readonly Behavior behav;
        private readonly string group;
        private readonly float radius;
        private Random rand = new Random();

        private OrderGroup(float radius, string group, Behavior behav)
        {
            this.radius = radius;
            this.group = group;
            this.behav = behav;
        }

        public static OrderGroup Instance(float radius, string group, Behavior behav)
        {
            var key = new Tuple<float, string, Behavior>(radius, group, behav);
            OrderGroup ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new OrderGroup(radius, group, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var ret = false;
            foreach (var i in GetNearestEntitiesByGroup(radius, group))
                ret |= behav.Tick(i, time);
            return ret;
        }
    }

    internal class OrderEntity : Behavior
    {
        private static readonly Dictionary<Tuple<float, short, Behavior>, OrderEntity> instances =
            new Dictionary<Tuple<float, short, Behavior>, OrderEntity>();

        private readonly Behavior behav;
        private readonly short objType;
        private readonly float radius;
        private Random rand = new Random();

        private OrderEntity(float radius, short objType, Behavior behav)
        {
            this.radius = radius;
            this.objType = objType;
            this.behav = behav;
        }

        public static OrderEntity Instance(float radius, short objType, Behavior behav)
        {
            var key = new Tuple<float, short, Behavior>(radius, objType, behav);
            OrderEntity ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new OrderEntity(radius, objType, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var d = radius;
            var x = GetNearestEntity(ref d, objType);
            if (x != null)
                return behav.Tick(x, time);
            return false;
        }
    }

    internal class OrderAllEntity : Behavior
    {
        private static readonly Dictionary<Tuple<float, short, Behavior>, OrderAllEntity> instances =
            new Dictionary<Tuple<float, short, Behavior>, OrderAllEntity>();

        private readonly Behavior behav;
        private readonly short objType;
        private readonly float radius;
        private Random rand = new Random();

        private OrderAllEntity(float radius, short objType, Behavior behav)
        {
            this.radius = radius;
            this.objType = objType;
            this.behav = behav;
        }

        public static OrderAllEntity Instance(float radius, short objType, Behavior behav)
        {
            var key = new Tuple<float, short, Behavior>(radius, objType, behav);
            OrderAllEntity ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new OrderAllEntity(radius, objType, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var x = GetNearestEntities(radius, objType);
            var ret = false;
            foreach (var i in x)
            {
                ret = true;
                behav.Tick(i, time);
            }
            return ret;
        }
    }
}