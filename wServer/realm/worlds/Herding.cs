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
    public class Herding : World
    {
        private readonly Dictionary<string, bool> Flags = new Dictionary<string, bool>();
        private readonly List<IntPoint> SheepSpawns = new List<IntPoint>();

        private readonly List<string> SpawningSheep = new List<string>
        {
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Herding Sheep",
            "Black Herding Sheep",
            "Giant Herding Sheep"
        };

        private int FamePot;
        private int HerdedSheep;

        private Random rand;

        public Herding()
        {
            Name = "Sheep Herding Minigame";
            Background = 0;
            AllowTeleport = true;
            FamePot = 0;
            HerdedSheep = 0;
            Flags.Add("started", false);
            Flags.Add("counting", false);
            Flags.Add("finished", false);
            rand = new Random();
            SetMusic("Island");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.herding.wmap"));
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
                    if (tile.Region == TileRegion.Defender)
                        SheepSpawns.Add(new IntPoint(x, y));
                }
        }

        public void Countdown(int s)
        {
            if (s != 0)
            {
                if (!Flags["started"])
                    foreach (var i in Players)
                        i.Value.Client.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0xffffff00),
                            ObjectId = i.Value.Id,
                            Text = s + " Second" + (s == 1 ? "" : "s")
                        });
                Timers.Add(new WorldTimer(1000, (w, t) => Countdown(s - 1)));
            }
            else
            {
                if (!Flags["started"])
                {
                    BroadcastPacket(new TextPacket
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "#Sheep Herding",
                        Text = "You have 2 minutes to herd sheep! Watch out for black sheep!"
                    }, null);
                    Flags["started"] = true;
                    Countdown(120);
                }
                else
                    Flags["counting"] = false;
            }
        }

        public override void Tick(RealmTime time)
        {
            if (Players.Count > 0 && !Flags["finished"])
            {
                if (Flags["started"] && Flags["counting"])
                {
                    if (Enemies.Count < 15 + Pets.Count)
                    {
                        SheepSpawns.Shuffle();
                        SpawningSheep.Shuffle();
                        Entity e = Entity.Resolve(XmlDatas.IdToType[SpawningSheep.First()]);
                        e.Move(SheepSpawns[0].X, SheepSpawns[0].Y);
                        EnterWorld(e);
                    }
                }
                else if (Flags["started"] && !Flags["counting"])
                {
                    var div = (int) Math.Ceiling((double) (FamePot/Players.Count));
                    double golddivider = HerdedSheep/20;
                    var tokens = (int) Math.Floor(golddivider);
                    BroadcastPacket(new TextPacket
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "#Sheep Herding",
                        Text = "Time's up! You each win " + div + " fame!"
                    }, null);
                    foreach (var i in Players)
                    {
                        i.Value.CurrentFame =
                            i.Value.Client.Account.Stats.Fame = i.Value.Client.Database.UpdateFame(i.Value.Client.Account, div);
                        i.Value.UpdateCount++;
                        i.Value.Client.SendPacket(new NotificationPacket
                        {
                            ObjectId = i.Value.Id,
                            Color = new ARGB(0xFFFF6600),
                            Text = "+" + div + " Fame"
                        });
                        i.Value.Credits =
                            i.Value.Client.Account.Credits = i.Value.Client.Database.UpdateCredit(i.Value.Client.Account, tokens);
                        i.Value.UpdateCount++;
                    }
                    foreach (var i in Enemies)
                        if (!i.Value.isPet)
                            LeaveWorld(i.Value);
                    Flags["finished"] = true;
                }
                else if (!Flags["started"] && !Flags["counting"])
                {
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                        i.SendPacket(new TextPacket
                        {
                            Stars = -1,
                            BubbleTime = 0,
                            Name = "#Announcement",
                            Text = "A sheep herding minigame has been started. Closing in 1 minute!"
                        });
                    Flags["counting"] = true;
                    Countdown(60);
                }
            }
            base.Tick(time);
        }

        public override void BehaviorEvent(string type)
        {
            if (type.StartsWith("sheep"))
            {
                int inc = (type.EndsWith("-g") ? 100 : 10)*Players.Count + 10*HerdedSheep;
                foreach (var i in Players)
                    i.Value.Client.SendPacket(new NotificationPacket
                    {
                        ObjectId = i.Value.Id,
                        Color = new ARGB(0xFFFFFF00),
                        Text = "+" + inc + " Fame in Pot"
                    });
                HerdedSheep += (type.EndsWith("-g") ? 2 : 1);
                FamePot += inc;
            }
            else if (type == "blackSheep")
            {
                foreach (var i in Players)
                    i.Value.Client.SendPacket(new NotificationPacket
                    {
                        ObjectId = i.Value.Id,
                        Color = new ARGB(0xFF777700),
                        Text = "Oh no! A black sheep stole all the fame!"
                    });
                FamePot = 0;
            }
            base.BehaviorEvent(type);
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Herding());
        }
    }
}