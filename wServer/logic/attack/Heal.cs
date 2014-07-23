#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class Heal : Behavior
    {
        private static readonly Dictionary<Tuple<float, int, short?>, Heal> instances =
            new Dictionary<Tuple<float, int, short?>, Heal>();

        private readonly int amount;
        private readonly short? objType;
        private readonly float radius;

        private Heal(float radius, int amount, short? objType)
        {
            this.radius = radius;
            this.amount = amount;
            this.objType = objType;
        }

        public static Heal Instance(float radius, int amount, short? objType)
        {
            var key = new Tuple<float, int, short?>(radius, amount, objType);
            Heal ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Heal(radius, amount, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (objType == null)
            {
                var host = Host.Self as Enemy;
                var hp = host.HP;
                hp = Math.Min(hp + amount, host.ObjectDesc.MaxHp);
                if (hp != host.HP)
                {
                    var n = hp - host.HP;
                    host.HP = hp;
                    host.UpdateCount++;
                    host.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Potion,
                        TargetId = host.Id,
                        Color = new ARGB(0xffffffff)
                    }, null);
                    host.Owner.BroadcastPacket(new NotificationPacket
                    {
                        ObjectId = host.Id,
                        Text = "+" + n,
                        Color = new ARGB(0xff00ff00)
                    }, null);

                    return true;
                }
            }
            else
            {
                var dist = radius;
                var entity = GetNearestEntity(ref dist, objType) as Enemy;
                while (entity != null)
                {
                    var hp = entity.HP;
                    hp = Math.Min(hp + amount, entity.ObjectDesc.MaxHp);
                    if (hp != entity.HP)
                    {
                        var n = hp - entity.HP;
                        entity.HP = hp;
                        entity.UpdateCount++;
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Potion,
                            TargetId = entity.Id,
                            Color = new ARGB(0xffffffff)
                        }, null);
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Trail,
                            TargetId = Host.Self.Id,
                            PosA = new Position {X = entity.X, Y = entity.Y},
                            Color = new ARGB(0xffffffff)
                        }, null);
                        entity.Owner.BroadcastPacket(new NotificationPacket
                        {
                            ObjectId = entity.Id,
                            Text = "+" + n,
                            Color = new ARGB(0xff00ff00)
                        }, null);

                        return true;
                    }
                    entity = GetNearestEntity(ref dist, null) as Enemy;
                }
            }
            return false;
        }
    }
}