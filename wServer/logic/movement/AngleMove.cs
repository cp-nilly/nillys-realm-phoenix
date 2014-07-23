#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class AngleMove : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, float>, AngleMove> instances =
            new Dictionary<Tuple<float, float, float>, AngleMove>();

        private readonly float angle;
        private readonly float dist;
        private readonly float speed;
        private Random rand = new Random();

        private AngleMove(float speed, float angle, float dist)
        {
            this.speed = speed;
            this.angle = angle;
            this.dist = dist;
        }

        public static AngleMove Instance(float speed, float angle, float dist)
        {
            var key = new Tuple<float, float, float>(speed, angle, dist);
            AngleMove ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AngleMove(speed, angle, dist);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            float d;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                Host.StateStorage[Key] = d = dist;
            else
            {
                d = (float) o;

                var dd = (speed/1.5f)*(time.thisTickTimes/1000f);
                d -= dd;
                ValidateAndMove(Host.Self.X + (float) Math.Cos(angle)*dd, Host.Self.Y + (float) Math.Sin(angle)*dd);
                Host.Self.UpdateCount++;
            }

            bool ret;
            if (d <= 0)
            {
                d = dist;
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = d;
            return ret;
        }
    }
}