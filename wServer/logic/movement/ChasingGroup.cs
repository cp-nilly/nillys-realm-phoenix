#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class ChasingGroup : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, float, string>, ChasingGroup> instances =
            new Dictionary<Tuple<float, float, float, string>, ChasingGroup>();

        private readonly string group;

        private readonly float radius;
        private readonly Random rand = new Random();
        private readonly float speed;
        private readonly float targetRadius;

        private ChasingGroup(float speed, float radius, float targetRadius, string group)
        {
            this.speed = speed;
            this.radius = radius;
            this.targetRadius = targetRadius;
            this.group = group;
        }

        public static ChasingGroup Instance(float speed, float radius, float targetRadius, string group)
        {
            var key = new Tuple<float, float, float, string>(speed, radius, targetRadius, group);
            ChasingGroup ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new ChasingGroup(speed, radius, targetRadius, group);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            var dist = radius;
            var entity = GetNearestEntityByGroup(ref dist, group);
            if (entity != null && dist > targetRadius)
            {
                var tx = entity.X + rand.Next(-2, 2)/2f;
                var ty = entity.Y + rand.Next(-2, 2)/2f;
                if (tx != Host.Self.X || ty != Host.Self.Y)
                {
                    var x = Host.Self.X;
                    var y = Host.Self.Y;
                    var vect = new Vector2(tx, ty) - new Vector2(Host.Self.X, Host.Self.Y);
                    vect.Normalize();
                    vect *= (speed/1.5f)*(time.thisTickTimes/1000f);
                    ValidateAndMove(Host.Self.X + vect.X, Host.Self.Y + vect.Y);
                    Host.Self.UpdateCount++;
                }
                return true;
            }
            return false;
        }
    }
}