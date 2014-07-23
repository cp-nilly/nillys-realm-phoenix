#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;

#endregion

namespace wServer.logic.movement
{
    internal class Swirling : Behavior
    {
        private static readonly Dictionary<Tuple<float, float>, Swirling> instances =
            new Dictionary<Tuple<float, float>, Swirling>();

        private readonly float angularSpeed;
        private readonly float radius;
        private readonly Random rand = new Random();
        private readonly float speed;

        private Swirling(float radius, float speed)
        {
            this.radius = radius;
            angularSpeed = speed/radius;
            this.speed = speed;
        }

        public static Swirling Instance(float radius, float speed)
        {
            var key = new Tuple<float, float>(radius, speed);
            Swirling ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Swirling(radius, speed);
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
                    center = new Position {X = Host.Self.X, Y = Host.Self.Y},
                    angle = (float) (2*Math.PI*rand.NextDouble())
                };
            }
            else
            {
                state = (CirclingState) o;

                state.angle += angularSpeed*(time.thisTickTimes/1000f);
                var x = state.center.X + Math.Cos(state.angle)*radius;
                var y = state.center.Y + Math.Sin(state.angle)*radius;
                if (x != Host.Self.X && y != Host.Self.Y)
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
            public Position center;
        }
    }
}