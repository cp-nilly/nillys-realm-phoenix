#region

using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.travoos
{
    internal class CheckRegion : Behavior
    {
        private static readonly Dictionary<TileRegion, CheckRegion> instances =
            new Dictionary<TileRegion, CheckRegion>();

        private readonly TileRegion region;

        private CheckRegion(TileRegion region)
        {
            this.region = region;
        }

        public static CheckRegion Instance(TileRegion region)
        {
            CheckRegion ret;
            if (!instances.TryGetValue(region, out ret))
                ret = instances[region] = new CheckRegion(region);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var wt = Host.Self.Owner.Map[(int) Host.Self.X, (int) Host.Self.Y];
            if (wt.Region == region)
                return true;
            return false;
        }
    }
}