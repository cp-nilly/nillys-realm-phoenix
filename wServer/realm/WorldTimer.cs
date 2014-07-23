#region

using System;

#endregion

namespace wServer.realm
{
    public class WorldTimer
    {
        private readonly Action<World, RealmTime> cb;
        private readonly int total;
        private int remain;

        private int t;

        public WorldTimer(int tickMs, Action<World, RealmTime> callback)
        {
            remain = total = tickMs;
            cb = callback;
            t = Environment.TickCount;
        }

        public void Reset()
        {
            remain = total;
        }

        public bool Tick(World world, RealmTime time)
        {
            remain -= time.thisTickTimes;
            if (remain < 0)
            {
                cb(world, time);
                return true;
            }
            return false;
        }
    }
}