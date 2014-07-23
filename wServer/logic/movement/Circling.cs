#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class Circling : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, float, short?>, Circling> instances =
            new Dictionary<Tuple<float, float, float, short?>, Circling>();

        private readonly float angularSpeed;
        private readonly short? objType;

        private readonly float radius;
        private readonly Random rand = new Random();
        private readonly float sight;
        private readonly float speed;

        private Circling(float radius, float sight, float speed, short? objType)
        {
            this.radius = radius;
            this.sight = sight;
            angularSpeed = speed/radius;
            this.speed = speed;
            this.objType = objType;
        }

        public static Circling Instance(float radius, float sight, float speed, short? objType)
        {
            var key = new Tuple<float, float, float, short?>(radius, sight, speed, objType);
            Circling ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Circling(radius, sight, speed, objType);
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
                var dist = sight;
                Host.StateStorage[Key] = state = new CirclingState
                {
                    target = WeakReference<Entity>.Create(GetNearestEntity(ref dist, objType)),
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
                if (x != Host.Self.X || y != Host.Self.Y)
                {
                    var vect = new Vector2((float) x, (float) y) - new Vector2(Host.Self.X, Host.Self.Y);
                    vect.Normalize();
                    vect *= (speed/1.5f)*(time.thisTickTimes/1000f);
                    ValidateAndMove(Host.Self.X + vect.X, Host.Self.Y + vect.Y);
                    Host.Self.UpdateCount++;
                }
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