#region

using System;
using System.Collections.Generic;
using System.Linq;
using db;
using db.data;
using wServer.svrPackets;

#endregion

namespace wServer.realm.worlds
{
    public class ZombieMG : World
    {
        private readonly Dictionary<string, bool> Flags = new Dictionary<string, bool>();

        private readonly List<string> spawningZombies = new List<string>
        {
            "Regular Zombie",
            "Regular Zombie",
            "Speedy Zombie",
            "Tank Zombie",
        };

        private readonly List<IntPoint> zombieSpawns = new List<IntPoint>();
        private int famePot;
        private Entity tower;
        private int towerHp;
        private int wave;
        private int zombieAmount;

        public ZombieMG()
        {
            Name = "Zombies Minigame";
            Background = 0;
            AllowTeleport = true;
            famePot = 0;
            wave = 0;
            towerHp = 50;
            zombieAmount = 1;
            Flags.Add("started", false);
            Flags.Add("counting", false);
            Flags.Add("finished", false);
            SetMusic("Haunted Cemetary");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.zombies.wmap"));
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
            bool addedTower = false;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    WmapTile tile = Map[x, y];
                    if (tile.Region == TileRegion.Defender)
                        zombieSpawns.Add(new IntPoint(x, y));
                    if (tile.Region == TileRegion.Enemy && !addedTower)
                    {
                        tower = Entity.Resolve(0x7023);
                        tower.Move(x + 0.5f, y + 0.5f);
                        addedTower = true;
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
                            Text = (s == 5 ? "Wave " + wave + " - " : "") + s + " Second" + (s == 1 ? "" : "s")
                        });
                    Timers.Add(new WorldTimer(1000, (w, t) => Countdown(s - 1)));
                }
            }
            else
            {
                if (!Flags["started"])
                {
                    Flags["started"] = true;
                    Flags["counting"] = false;
                }
                else
                {
                    Flags["counting"] = false;
                    SpawnZombies();
                }
            }
        }


        public void SpawnZombies()
        {
            for (int i = 0; i < zombieAmount; i++)
            {
                zombieSpawns.Shuffle();
                spawningZombies.Shuffle();
                Entity e = Entity.Resolve(XmlDatas.IdToType[spawningZombies.First()]);
                e.Move(zombieSpawns[0].X, zombieSpawns[0].Y);
                EnterWorld(e);
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
                        wave++;
                        zombieAmount += Players.Count;
                        famePot += wave*10/2;
                        Flags["counting"] = true;
                        Countdown(5);
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
                            Text = "A zombie minigame has been started. Closing in 1 minute!"
                        });
                    EnterWorld(tower);
                    Flags["counting"] = true;
                    Countdown(60);
                }
            }

            base.Tick(time);
        }

        public override void BehaviorEvent(string type)
        {
            string[] typec = type.Split(':');
            if (typec.Length > 1)
                if (typec[0] == "dmg")
                {
                    try
                    {
                        towerHp -= Convert.ToInt32(typec[1]);
                        if (towerHp > 0)
                            BroadcastPacket(new TextPacket
                            {
                                Stars = -1,
                                BubbleTime = 0,
                                Name = "#Zombies",
                                Text = "Your tower's been damaged! " + towerHp + " HP left."
                            }, null);
                        else
                        {
                            Flags["finished"] = true;

                            BroadcastPacket(new TextPacket
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "#Zombies",
                                Text = "Your tower's been destroyed! You each earn " + famePot + " fame!"
                            }, null);
                            double golddivider = wave/15;
                            var tokens = (int) Math.Floor(golddivider);
                            foreach (var i in Players)
                            {
                                var db = new Database();
                                i.Value.CurrentFame =
                                    i.Value.Client.Account.Stats.Fame = db.UpdateFame(i.Value.Client.Account, famePot);
                                i.Value.UpdateCount++;
                                i.Value.Client.SendPacket(new NotificationPacket
                                {
                                    ObjectId = i.Value.Id,
                                    Color = new ARGB(0xFFFF6600),
                                    Text = "+" + famePot + " Fame"
                                });
                                i.Value.Credits =
                                    i.Value.Client.Account.Credits = db.UpdateCredit(i.Value.Client.Account, tokens);
                                i.Value.UpdateCount++;
                                Flags["counting"] = true;
                                db.Dispose();
                            }
                            foreach (var i in Enemies)
                            {
                                if (!i.Value.isPet)
                                {
                                    LeaveWorld(i.Value);
                                }
                            }
                            LeaveWorld(tower);
                        }
                    }
                    catch
                    {
                    }
                }
            base.BehaviorEvent(type);
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new ZombieMG());
        }
    }
}