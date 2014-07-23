#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic.movement
{
    internal class Retracting : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, short?>, Retracting> instances =
            new Dictionary<Tuple<float, float, short?>, Retracting>();

        private readonly short? objType;
        private readonly float radius;
        private readonly float speed;
        private Random rand = new Random();

        private Retracting(float speed, float radius, short? objType)
        {
            this.speed = speed;
            this.radius = radius;
            this.objType = objType;
        }

        public static Retracting Instance(float speed, float radius, short? objType)
        {
            var key = new Tuple<float, float, short?>(speed, radius, objType);
            Retracting ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Retracting(speed, radius, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            var dist = radius;
            var entity = GetNearestEntity(ref dist, objType);
            var chr = Host as Character;
            if (entity != null && (entity.X != Host.Self.X || entity.Y != Host.Self.Y))
            {
                var x = Host.Self.X;
                var y = Host.Self.Y;
                var vect = new Vector2(entity.X, entity.Y) - new Vector2(Host.Self.X, Host.Self.Y);
                vect.Normalize();
                vect *= -1*(speed/1.5f)*(time.thisTickTimes/1000f);
                ValidateAndMove(Host.Self.X + vect.X, Host.Self.Y + vect.Y);
                Host.Self.UpdateCount++;

                return true;
            }
            return false;
        }
    }
}