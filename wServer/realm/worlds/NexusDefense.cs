#region

using System;
using System.Collections.Generic;
using System.Linq;
using db;
using db.data;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.realm.worlds
{
    public class NexusDefense : World
    {
        private readonly Dictionary<string, bool> Flags = new Dictionary<string, bool>();

        private readonly Dictionary<string, float> RandomBosses = new Dictionary<string, float>
        {
            {"Elder Tree", 25},
            {"Limon the Sprite God", 25},
            {"Stheno the Snake Queen", 25},
            {"Abyss Idol", 25},
            {"Archdemon Malphas", 50},
            {"Septavius the Ghost God", 50},
            {"Skull Shrine", 100},
            {"Cube God", 100},
            {"Grand Sphinx", 125},
            {"Lord of the Lost Lands", 125},
            {"Crystal Prisoner", 150},
            {"Phoenix God", 150},
            {"Thessal the Mermaid Goddess", 200},
            {"Tomb Support", 200},
            {"Tomb Defender", 200},
            {"Tomb Attacker", 200},
            {"Oryx the Mad God 2", 200}
        };

        private readonly Dictionary<string, float> UsedBuffs = new Dictionary<string, float>();
        private readonly Dictionary<string, float> UsedDebuffs = new Dictionary<string, float>();
        private readonly List<IntPoint> bossSpawns = new List<IntPoint>();

        private readonly Dictionary<string, float> buffs = new Dictionary<string, float>
        {
            {"Speedy", 2},
            {"Tiny", 4},
            {"Small", 2},
            {"Buff", 3},
            {"Armored", 3},
            {"Invisible", 5},
            {"Berserk", 5},
            {"Damaging", 5}
        };

        private readonly Dictionary<string, float> debuffs = new Dictionary<string, float>
        {
            {"Slow", 2},
            {"Giant", 4},
            {"Large", 2},
            {"Vulnerable", 3},
            {"Unarmored", 5},
            {"Weak", 5},
            {"Dazed", 5},
            {"Paralyzed", 5}
        };

        private int actualmonsters = 0;
        private Dictionary<string, float> availableBuffs = new Dictionary<string, float>();
        private Dictionary<string, float> availableDebuffs = new Dictionary<string, float>();
        private Dictionary<string, float> currentbuffs = new Dictionary<string, float>();
        private Dictionary<string, float> currentdebuffs = new Dictionary<string, float>();
        private int enemynumber;
        private KeyValuePair<string, float> monster;
        private float monstervalue;

        public NexusDefense()
        {
            Name = "Nexus Defense";
            Background = 2;
            AllowTeleport = true;
            Flags.Add("started", false);
            Flags.Add("counting", false);
            Flags.Add("finished", false);
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.nexusdefense.wmap"));
            InitVars();
        }


        public bool Joinable
        {
            get { return !Flags["started"]; }
        }

        private void InitVars()
        {
            int w = Map.Width;
            int h = Map.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    WmapTile tile = Map[x, y];
                    if (tile.Region == TileRegion.Enemy)
                    {
                        bossSpawns.Add(new IntPoint(x, y));
                    }
                }
        }

        public void Countdown(int s)
        {
            if (s != 0)
            {
                if (!Flags["started"])
                {
                    foreach (var i in Players)
                        i.Value.Client.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0xffff0000),
                            ObjectId = i.Value.Id,
                            Text = s + " Second" + (s == 1 ? "" : "s")
                        });
                    Timers.Add(new WorldTimer(1000, (w, t) => Countdown(s - 1)));
                }
                else
                {
                    foreach (var i in Players)
                        i.Value.Client.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0xffff0000),
                            ObjectId = i.Value.Id,
                            Text = (s == 5 ? "Next Boss in " : "") + s + " Second" + (s == 1 ? "" : "s")
                        });
                    Timers.Add(new WorldTimer(1000, (w, t) => Countdown(s - 1)));
                }
            }
            else
            {
                foreach (var i in Players)
                    i.Value.Client.SendPacket(new SwitchMusicPacket
                    {
                        Music = "Arena"
                    });
                if (!Flags["started"])
                {
                    Flags["started"] = true;
                    Flags["counting"] = false;
                }
                else
                {
                    SpawnBoss();
                    Flags["counting"] = false;
                }
            }
        }

        public void getBuffs(int amount)
        {
            availableBuffs = buffs;
            for (int i = 0; i < amount; i++)
            {
                var rand = new Random();
                int random = rand.Next(0, availableBuffs.Count);
                KeyValuePair<string, float> buff = availableBuffs.ElementAt(random);
                availableBuffs.Remove(buff.Key);
                UsedBuffs.Add(buff.Key, buff.Value);
                if (buff.Key == "Speedy")
                {
                    availableDebuffs.Remove("Slow");
                    availableDebuffs.Remove("Paralyzed");
                }
                if (buff.Key == "Tiny")
                {
                    availableDebuffs.Remove("Large");
                    availableDebuffs.Remove("Giant");
                    availableBuffs.Remove("Small");
                    availableBuffs.Remove("Invisible");
                }
                if (buff.Key == "Small")
                {
                    availableDebuffs.Remove("Large");
                    availableDebuffs.Remove("Giant");
                    availableBuffs.Remove("Tiny");
                    availableBuffs.Remove("Invisible");
                }
                if (buff.Key == "Buff")
                {
                    availableDebuffs.Remove("Vulnerable");
                }
                if (buff.Key == "Armored")
                {
                    availableDebuffs.Remove("Unarmored");
                }
                if (buff.Key == "Invisible")
                {
                    availableDebuffs.Remove("Large");
                    availableDebuffs.Remove("Giant");
                    availableBuffs.Remove("Tiny");
                    availableBuffs.Remove("Small");
                }
                if (buff.Key == "Berserk")
                {
                    availableDebuffs.Remove("Dazed");
                }
                if (buff.Key == "Damaging")
                {
                    availableDebuffs.Remove("Weak");
                }
            }
            currentbuffs = UsedBuffs;
        }

        public void getDebuffs(int amount)
        {
            availableDebuffs = debuffs;
            for (int i = 0; i < amount; i++)
            {
                var rand = new Random();
                int random = rand.Next(0, availableDebuffs.Count);
                KeyValuePair<string, float> debuff = availableDebuffs.ElementAt(random);
                availableDebuffs.Remove(debuff.Key);
                UsedDebuffs.Add(debuff.Key, debuff.Value);
                if (debuff.Key == "Slowed")
                {
                    availableDebuffs.Remove("Paralyzed");
                }
                if (debuff.Key == "Giant")
                {
                    availableDebuffs.Remove("Large");
                }
                if (debuff.Key == "Large")
                {
                    availableDebuffs.Remove("Giant");
                }
                if (debuff.Key == "Paralyzed")
                {
                    availableDebuffs.Remove("Slowed");
                }
            }
            currentdebuffs = UsedDebuffs;
        }

        public void SpawnBoss()
        {
            bossSpawns.Shuffle();
            var rand = new Random();
            monster = RandomBosses.ElementAt(rand.Next(0, RandomBosses.Count));
            Entity e = Entity.Resolve(XmlDatas.IdToType[monster.Key]);
            availableDebuffs = buffs;
            availableDebuffs = debuffs;
            if (enemynumber < 3)
            {
                getBuffs(1);
                getDebuffs(1);
            }
            else if (enemynumber < 6)
            {
                getBuffs(2);
                getDebuffs(1);
            }
            else
            {
                getBuffs(4);
                getDebuffs(2);
            }
            foreach (var i in UsedBuffs)
            {
                string name = i.Key;
                var Speedy = new ConditionEffect();
                Speedy.Effect = ConditionEffectIndex.Speedy;
                Speedy.DurationMS = -1;
                var Berserk = new ConditionEffect();
                Berserk.Effect = ConditionEffectIndex.Berserk;
                Berserk.DurationMS = -1;
                var Armored = new ConditionEffect();
                Armored.Effect = ConditionEffectIndex.Armored;
                Armored.DurationMS = -1;
                var Damaging = new ConditionEffect();
                Damaging.Effect = ConditionEffectIndex.Damaging;
                Damaging.DurationMS = -1;
                switch (name)
                {
                    case "Speedy":
                        e.ApplyConditionEffect(new[]
                        {
                            Speedy
                        });
                        break;
                    case "Berserk":
                        e.ApplyConditionEffect(new[]
                        {
                            Berserk
                        });
                        break;
                    case "Tiny":
                        e.Size = e.Size/4;
                        break;
                    case "Small":
                        e.Size = e.Size/2;
                        break;
                    case "Buff":
                        (e as Enemy).HP = (e as Enemy).HP*2;
                        break;
                    case "Armored":
                        e.ApplyConditionEffect(new[]
                        {
                            Armored
                        });
                        break;
                    case "Invisible":
                        e.Size = 0;
                        break;
                    case "Damaging":
                        e.ApplyConditionEffect(new[]
                        {
                            Damaging
                        });
                        break;
                }
            }
            foreach (var i in UsedDebuffs)
            {
                string name = i.Key;
                var Slowed = new ConditionEffect();
                Slowed.Effect = ConditionEffectIndex.Slowed;
                Slowed.DurationMS = -1;
                var ArmorBroken = new ConditionEffect();
                ArmorBroken.Effect = ConditionEffectIndex.ArmorBroken;
                ArmorBroken.DurationMS = -1;
                var Weak = new ConditionEffect();
                Weak.Effect = ConditionEffectIndex.Weak;
                Weak.DurationMS = -1;
                var Dazed = new ConditionEffect();
                Dazed.Effect = ConditionEffectIndex.Dazed;
                Dazed.DurationMS = -1;
                var Paralyzed = new ConditionEffect();
                Paralyzed.Effect = ConditionEffectIndex.Paralyzed;
                Paralyzed.DurationMS = -1;
                switch (name)
                {
                    case "Slow":
                        e.ApplyConditionEffect(new[]
                        {
                            Slowed
                        });
                        break;
                    case "Giant":
                        e.Size = e.Size*4;
                        break;
                    case "Large":
                        e.Size = e.Size*2;
                        break;
                    case "Vulnerable":
                        (e as Enemy).HP = (e as Enemy).HP/2;
                        break;
                    case "Unarmored":
                        e.ApplyConditionEffect(new[]
                        {
                            ArmorBroken
                        });
                        break;
                    case "Weak":
                        e.ApplyConditionEffect(new[]
                        {
                            Weak
                        });
                        break;
                    case "Dazed":
                        e.ApplyConditionEffect(new[]
                        {
                            Dazed
                        });
                        break;
                    case "Paralyzed":
                        e.ApplyConditionEffect(new[]
                        {
                            Paralyzed
                        });
                        break;
                }
                e.Name = buffs.Keys.ToArray().ToString() + debuffs.Keys.ToArray().ToString() + e.Name;
                float buffvalue = 0;
                float debuffvalue = 0;
                foreach (var x in buffs)
                {
                    buffvalue += x.Value;
                }
                foreach (var y in debuffs)
                {
                    debuffvalue += y.Value;
                }
                monstervalue = (int) Math.Ceiling(monster.Value*buffvalue/debuffvalue);
                BroadcastPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "#Nexus Defense",
                    Text = "New Monster: " + e.Name + " Bounty: " + monstervalue
                }, null);
                e.Move(bossSpawns[0].X, bossSpawns[0].Y);
                EnterWorld(e);
                UsedBuffs.Clear();
                UsedDebuffs.Clear();
            }
        }

        public override void Tick(RealmTime time)
        {
            if (Players.Count > 0 && !Flags["finished"])
            {
                if (Flags["started"] && !Flags["counting"])
                {
                    if (Enemies.Count < 1 + Pets.Count)
                    {
                        if (monstervalue != 0)
                        {
                            var db = new Database();
                            foreach (var i in Players)
                            {
                                i.Value.CurrentFame =
                                    i.Value.Client.Account.Stats.Fame =
                                        db.UpdateFame(i.Value.Client.Account, (int) monstervalue);
                                i.Value.UpdateCount++;
                                i.Value.Client.SendPacket(new NotificationPacket
                                {
                                    Color = new ARGB(0xFFFF6600),
                                    ObjectId = i.Value.Id,
                                    Text = "+" + (int) monstervalue + " Fame"
                                });
                                if (Math.IEEERemainder(monstervalue, 1000) == 0)
                                {
                                    i.Value.Credits =
                                        i.Value.Client.Account.Credits = db.UpdateCredit(i.Value.Client.Account, 1);
                                    i.Value.UpdateCount++;
                                }
                            }
                            db.Dispose();
                            Countdown(5);
                        }
                    }
                }

                else if (!Flags["started"] && !Flags["counting"])
                {
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                        i.SendPacket(new TextPacket
                        {
                            Stars = -1,
                            BubbleTime = 0,
                            Name = "#Announcement",
                            Text = "A Nexus Defense Game has been started. Closing in 1 minute!"
                        });
                    Flags["counting"] = true;
                    Countdown(60);
                }
            }
            base.Tick(time);
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new NexusDefense());
        }
    }
}