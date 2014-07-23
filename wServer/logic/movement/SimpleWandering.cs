#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class SimpleWandering : Behavior
    {
        private static readonly Dictionary<Tuple<float, float>, SimpleWandering> instances =
            new Dictionary<Tuple<float, float>, SimpleWandering>();

        private readonly float dist;
        private readonly Random rand = new Random();
        private readonly float speed;

        private SimpleWandering(float speed, float dist)
        {
            this.speed = speed;
            this.dist = dist;
        }

        public static SimpleWandering Instance(float speed, float dist = 1f)
        {
            var key = new Tuple<float, float>(speed, dist);
            SimpleWandering ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new SimpleWandering(speed, dist);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            WanderingState state;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                Host.StateStorage[Key] = state = new WanderingState();
            else
            {
                state = (WanderingState) o;

                var dist = (speed/1.5f)*(time.thisTickTimes/1000f);
                state.remainingDist -= dist;
                ValidateAndMove(Host.Self.X + state.x*dist, Host.Self.Y + state.y*dist);
                Host.Self.UpdateCount++;
            }

            bool ret;
            if (state.remainingDist <= 0)
            {
                state.x = rand.Next(-1, 2);
                state.y = rand.Next(-1, 2);
                state.remainingDist = dist + dist*(float) (rand.NextDouble()*0.1 - 0.05);
                ret = true;
            }
            else
                ret = false;
            return ret;
        }

        private class WanderingState
        {
            public float remainingDist;
            public int x;
            public int y;
        }
    }
}