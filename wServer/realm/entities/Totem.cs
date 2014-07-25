#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.logic;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    internal class Totem : StaticObject
    {
        private readonly int amount;
        private readonly string monster;
        private readonly float radius;
        private int maximumtransforms;
        private int p = 1;
        private Player player;
        private int t;
        private short totem;

        public Totem(Player player, float radius, int amount, string monster, short totem)
            : base(totem, 3000, true, true, false)
        {
            this.player = player;
            this.radius = radius;
            this.amount = amount;
            this.monster = monster;
            this.totem = totem;
        }

        public override void Tick(RealmTime time)
        {
            if (t/1500 == p)
            {
                p = 100;
                var pkts = new List<Packet>();
                var enemies = new List<Enemy>();
                short obj;
                string pt = monster;
                XmlDatas.IdToType.TryGetValue(pt, out obj);
                BehaviorBase.AOE(Owner, this, radius, false, enemy => { enemies.Add(enemy as Enemy); });
                Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.AreaBlast,
                    Color = new ARGB(0x4E6C00),
                    TargetId = Id,
                    PosA = new Position {X = radius}
                }, null);
                foreach (Enemy i in enemies)
                {
                    try
                    {
                        if (i.HasConditionEffect(ConditionEffects.StasisImmune) || i.ObjectDesc.MaxHp > amount ||
                            i.Name == monster || i.isPet || i.isSummon || !i.ObjectDesc.Enemy ||
                            i.Name == "Pentaract Tower" || i.HasConditionEffect(ConditionEffects.Invincible) ||
                            i.HasConditionEffect(ConditionEffects.Invulnerable) || i.ObjectDesc.StasisImmune)
                        {
                            pkts.Add(new NotificationPacket
                            {
                                ObjectId = i.Id,
                                Color = new ARGB(0x4E6C00),
                                Text = "Immune"
                            });
                        }
                        else
                        {
                            var pos = new Position();
                            pos.X = i.X;
                            pos.Y = i.Y;
                            Owner.LeaveWorld(i);
                            Entity newenemy = Resolve(obj);
                            Owner.EnterWorld(newenemy);
                            newenemy.Move(pos.X, pos.Y);
                            maximumtransforms++;
                            if (maximumtransforms == 10)
                            {
                                break;
                            }
                        }
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Out.WriteLine("Crash halted - Totem error!");
                        Console.Out.WriteLine(i.Name);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Owner.BroadcastPackets(pkts, null);
                t += time.thisTickTimes;
            }
            t += time.thisTickTimes;
            base.Tick(time);
        }
    }
}