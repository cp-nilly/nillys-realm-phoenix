#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class StrictCirclingGroup : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, string>, StrictCirclingGroup> instances =
            new Dictionary<Tuple<float, float, string>, StrictCirclingGroup>();

        private readonly float angularSpeed;
        private readonly string group;
        private readonly float radius;
        private readonly Random rand = new Random();
        private readonly float speed;

        private StrictCirclingGroup(float radius, float speed, string group)
        {
            this.radius = radius;
            angularSpeed = speed/radius;
            this.speed = speed;
            this.group = group;
        }

        public static StrictCirclingGroup Instance(float radius, float speed, string group)
        {
            var key = new Tuple<float, float, string>(radius, speed, group);
            StrictCirclingGroup ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new StrictCirclingGroup(radius, speed, group);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            CirclingState state;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
            {
                var dist = radius;
                Host.StateStorage[Key] = state = new CirclingState
                {
                    target = WeakReference<Entity>.Create(GetNearestEntityByGroup(ref dist, group)),
                    angle = (float) (2*Math.PI*rand.NextDouble())
                };
            }
            else
            {
                state = (CirclingState) o;

                state.angle += angularSpeed*(time.thisTickTimes/1000f);
                if (!state.target.IsAlive)
                {
                    Host.StateStorage.Remove(Key);
                    return false;
                }
                var target = state.target.Target;
                if (target == null || target.Owner == null)
                {
                    Host.StateStorage.Remove(Key);
                    return false;
                }
                var x = target.X + Math.Cos(state.angle)*radius;
                var y = target.Y + Math.Sin(state.angle)*radius;
                ValidateAndMove((float) x, (float) y);
                Host.Self.UpdateCount++;
            }

            if (state.angle >= Math.PI*2)
                state.angle -= (float) (Math.PI*2);
            return true;
        }

        private class CirclingState
        {
            public float angle;
            public WeakReference<Entity> target;
        }
    }
}