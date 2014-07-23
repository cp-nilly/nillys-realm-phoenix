#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class ThrowAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, int, float>, ThrowAttack> instances =
            new Dictionary<Tuple<float, float, int, float>, ThrowAttack>();

        private readonly float bombRadius;
        private readonly int damage;
        private readonly float offsetAngle;
        private readonly float sightRadius;
        private Random rand = new Random();

        private ThrowAttack(float bombRadius, float sightRadius, int damage, float offsetAngle)
        {
            this.bombRadius = bombRadius;
            this.sightRadius = sightRadius;
            this.damage = damage;
            this.offsetAngle = offsetAngle;
        }

        public static ThrowAttack Instance(float bombRadius, float sightRadius, int damage, float offsetAngle = 0)
        {
            var key = new Tuple<float, float, int, float>(bombRadius, sightRadius, damage, offsetAngle);
            ThrowAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new ThrowAttack(bombRadius, sightRadius, damage, offsetAngle);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            var dist = sightRadius;

            var player = GetNearestEntity(ref dist, null);
            if (player != null)
            {
                var distance = Vector2.Distance(new Vector2(Host.Self.X, Host.Self.Y), new Vector2(player.X, player.Y));
                var chr = Host as Character;
                var angle = Math.Atan2(player.Y - chr.Y, player.X - chr.X)
                               + offsetAngle;
                var target = new Position
                {
                    X = Host.Self.X,
                    Y = Host.Self.Y
                };
                target.X += (float) Math.Cos(angle)*distance;
                target.Y += (float) Math.Sin(angle)*distance;

                chr.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Throw,
                    Color = new ARGB(0xffff0000),
                    TargetId = Host.Self.Id,
                    PosA = target
                }, null);
                chr.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                {
                    world.BroadcastPacket(new AOEPacket
                    {
                        Position = target,
                        Radius = bombRadius,
                        Damage = (ushort) damage,
                        EffectDuration = 0,
                        Effects = 0,
                        OriginType = Host.Self.ObjectType
                    }, null);
                    AOE(world, target, bombRadius, true, p => { (p as IPlayer).Damage(damage, Host.Self as Character); });
                }));

                return true;
            }
            return false;
        }
    }
}