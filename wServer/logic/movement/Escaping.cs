#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.movement
{
    internal class Escaping : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, int, short?>, Escaping> instances =
            new Dictionary<Tuple<float, float, int, short?>, Escaping>();

        private readonly short? objType;

        private readonly float radius;
        private readonly float speed;
        private readonly int threshold;
        private Random rand = new Random();

        private Escaping(float speed, float radius, int threshold, short? objType)
        {
            this.speed = speed;
            this.radius = radius;
            this.threshold = threshold;
            this.objType = objType;
        }

        public static Escaping Instance(float speed, float radius, int threshold, short? objType)
        {
            var key = new Tuple<float, float, int, short?>(speed, radius, threshold, objType);
            Escaping ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Escaping(speed, radius, threshold, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return true;
            var speed = this.speed*GetSpeedMultiplier(Host.Self);

            var dist = radius;
            var entity = GetNearestEntity(ref dist, objType);
            var chr = Host as Character;
            if (entity != null && chr.HP < threshold)
            {
                var x = Host.Self.X;
                var y = Host.Self.Y;
                var vect = new Vector2(entity.X, entity.Y) - new Vector2(Host.Self.X, Host.Self.Y);
                vect.Normalize();
                vect *= -1*(speed/1.5f)*(time.thisTickTimes/1000f);
                ValidateAndMove(Host.Self.X + vect.X, Host.Self.Y + vect.Y);
                Host.Self.UpdateCount++;

                if (!Host.StateStorage.ContainsKey(Key))
                {
                    chr.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Flashing,
                        PosA = new Position {X = 1, Y = 1000000},
                        TargetId = chr.Id,
                        Color = new ARGB(0xff303030)
                    }, null);
                    Host.StateStorage[Key] = true;
                }

                return true;
            }
            return false;
        }
    }
}