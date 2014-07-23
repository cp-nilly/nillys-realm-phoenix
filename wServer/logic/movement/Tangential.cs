#region

using System.Collections.Generic;
using Mono.Game;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class Tangential : Behavior
    {
        private static readonly Dictionary<float, Tangential> instances = new Dictionary<float, Tangential>();
        private readonly float speed;

        private Tangential(float speed)
        {
            this.speed = speed;
        }

        public static Tangential Instance(float speed)
        {
            Tangential ret;
            if (!instances.TryGetValue(speed, out ret))
                ret = instances[speed] = new Tangential(speed);
            return ret;
        }

        private Vector2 GetVelocity()
        {
            var ms = 100;
            var history = Host.Self.TryGetHistory(ms);
            if (history == null)
                return Vector2.Zero;
            return new Vector2(
                (Host.Self.X - history.Value.X)/(ms/1000f),
                (Host.Self.Y - history.Value.Y)/(ms/1000f)
                );
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            var velo = GetVelocity();
            if (velo != Vector2.Zero)
                velo.Normalize();
            var dist = (speed/1.5f)*(time.thisTickTimes/1000f);
            ValidateAndMove(Host.Self.X + velo.X*dist, Host.Self.Y + velo.Y*dist);
            Host.Self.UpdateCount++;
            return true;
        }
    }
}