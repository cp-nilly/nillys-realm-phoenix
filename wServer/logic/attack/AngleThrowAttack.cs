#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class AngleThrowAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, float, int>, AngleThrowAttack> instances =
            new Dictionary<Tuple<float, float, float, int>, AngleThrowAttack>();

        private readonly float angle;
        private readonly float bombRadius;
        private readonly int damage;
        private readonly float range;
        private Random rand = new Random();

        private AngleThrowAttack(float angle, float range, float bombRadius, int damage)
        {
            this.angle = angle;
            this.range = range;
            this.bombRadius = bombRadius;
            this.damage = damage;
        }

        public static AngleThrowAttack Instance(float angle, float range, float bombRadius, int damage)
        {
            var key = new Tuple<float, float, float, int>(angle, range, bombRadius, damage);
            AngleThrowAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AngleThrowAttack(angle, range, bombRadius, damage);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            var chr = Host as Character;
            var target = new Position
            {
                X = Host.Self.X,
                Y = Host.Self.Y
            };
            target.X += (float) Math.Cos(angle)*range;
            target.Y += (float) Math.Sin(angle)*range;
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
    }
}