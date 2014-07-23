#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class ThrowAttackPet : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, int, int>, ThrowAttackPet> instances =
            new Dictionary<Tuple<float, float, int, int>, ThrowAttackPet>();

        private readonly float bombRadius;
        private readonly int highest;
        private readonly int lowest;
        private readonly float sightRadius;
        private Random rand = new Random();

        private ThrowAttackPet(float bombRadius, float sightRadius, int lowest, int highest)
        {
            this.bombRadius = bombRadius;
            this.sightRadius = sightRadius;
            this.lowest = lowest;
            this.highest = highest;
        }

        public static ThrowAttackPet Instance(float bombRadius, float sightRadius, int lowest, int highest)
        {
            var key = new Tuple<float, float, int, int>(bombRadius, sightRadius, lowest, highest);
            ThrowAttackPet ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new ThrowAttackPet(bombRadius, sightRadius, lowest, highest);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            var dist = sightRadius;
            var entity = GetNearestEntityPet(ref dist) as Enemy;
            var dmg = new Random().Next(lowest, highest);
            if (entity != null)
            {
                var chr = Host as Character;
                var target = new Position

                {
                    X = entity.X,
                    Y = entity.Y
                };
                chr.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Throw,
                    Color = new ARGB(0x1C86EE),
                    TargetId = Host.Self.Id,
                    PosA = target
                }, null);

                var x = new Placeholder(1500);
                x.Move(target.X, target.Y);
                Host.Self.Owner.EnterWorld(x);
                chr.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                {
                    if (Host.Self != null && Host.Self.PlayerOwner != null)
                    {
                        chr.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.AreaBlast,
                            Color = new ARGB(0x1C86EE),
                            TargetId = x.Id,
                            PosA = new Position {X = bombRadius}
                        }, null);
                        AOEPet(Host.Self.Owner, target, bombRadius,
                            p =>
                            {
                                (p as Enemy).Damage(Host.Self.PlayerOwner, time, dmg, false, new ConditionEffect[] {});
                            });
                    }
                }));

                return true;
            }
            return false;
        }
    }
}