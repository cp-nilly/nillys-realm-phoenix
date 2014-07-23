#region

using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.travoos
{
    internal class WorldEvent : Behavior
    {
        private static readonly Dictionary<string, WorldEvent> instances = new Dictionary<string, WorldEvent>();
        private readonly string type;

        private WorldEvent(string type)
        {
            this.type = type;
        }

        public static WorldEvent Instance(string type)
        {
            WorldEvent ret;
            if (!instances.TryGetValue(type, out ret))
                ret = instances[type] = new WorldEvent(type);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.Owner.BehaviorEvent(type);
            return true;
        }
    }
}