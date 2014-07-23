#region

using System;
using System.Collections.Generic;
using wServer.logic;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    internal class Totem : StaticObject
    {
        private readonly int amount;
        private readonly float radius;
        string monster;
        short totem;
        private int t;
        private int p = 1;
        private Player player;
        int maximumtransforms = 0;

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
            if (t / 1500 == p)
            {
                p = 100;
                var pkts = new List<Packet>();
                var enemies = new List<Enemy>();
                short obj;
                var pt = monster;
                db.data.XmlDatas.IdToType.TryGetValue(pt, out obj);
                BehaviorBase.AOE(Owner, this, radius, false, enemy =>
                {
                    enemies.Add(enemy as Enemy);
                });
                Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.AreaBlast,
                    Color = new ARGB(0x4E6C00),
                    TargetId = this.Id,
                    PosA = new Position { X = radius }
                }, null);
                foreach (var i in enemies)
                {
                    try
                    {
                        if (i.HasConditionEffect(ConditionEffects.StasisImmune) || i.ObjectDesc.MaxHp > amount || i.Name == monster || i.isPet || i.isSummon || !i.ObjectDesc.Enemy || i.Name == "Pentaract Tower" || i.HasConditionEffect(ConditionEffects.Invincible) || i.HasConditionEffect(ConditionEffects.Invulnerable) || i.ObjectDesc.StasisImmune)
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
                            Position pos = new Position();
                            pos.X = i.X;
                            pos.Y = i.Y;
                            Owner.LeaveWorld(i);
                            var newenemy = Resolve(obj);
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