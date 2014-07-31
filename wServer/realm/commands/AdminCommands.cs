#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using db;
using db.data;
using MySql.Data.MySqlClient;
using wServer.cliPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.setpieces;
using wServer.realm.worlds;
using wServer.svrPackets;

#endregion

namespace wServer.realm.commands
{
    internal class SpawnCommand : ICommand
    {
        public string Command
        {
            get { return "spawn"; }
        }

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            string name;
            short objType;
            int amount = 1;
            int delay = 3; // in seconds
            
            // multiple spawn check
            if (args.Length > 0 && !int.TryParse(args[0], out amount)) {
                name = string.Join(" ", args);
                amount = 1;
            }
            else
            {
                name = string.Join(" ", args.Skip(1).ToArray());
            }

            
            Console.Write("<" + player.nName + "> Spawning " + ((amount > 1) ? amount + " " : "") + name + "...\n");

            // check map restrictions
            string mapName = player.Owner.Name;
            if (mapName.Equals("Nexus") ||
                mapName.Equals("Guild Hall") ||
                mapName.Equals("Vault"))
            {
                player.SendInfo("Spawning in " + mapName + " not allowed.");
                return;
            }
                
            // check to see if entity exists
            var icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                // ^^ creates a new case insensitive dictionary based on the XmlDatas
            if (!icdatas.TryGetValue(name, out objType) ||
                !XmlDatas.ObjectDescs.ContainsKey(objType))
            {
                player.SendInfo("Unknown entity!");
                return;
            }

            // check for banned objects
            Regex wall = new Regex(@"( |^)wall( |$)");
            string lname = name.ToLower();
            if (player.Client.Account.Rank < 5 && 
                lname.Equals("white fountain") ||
                lname.Equals("blood fountain") ||
                lname.Equals("scarab") ||
                lname.Equals("lair burst trap") ||
                lname.Equals("lair blast trap") ||
                lname.Equals("phoenix god final divination") ||
                
                // beach nightmare restriction
                lname.Equals("pirate") ||
                lname.Equals("piratess") ||
                lname.Equals("snake") ||
                lname.Equals("poison scorpion") ||
                lname.Equals("scorpion queen") ||
                lname.Equals("bandit leader") ||
                lname.Equals("bandit") ||
                lname.Equals("red gelatinous cube") ||
                lname.Equals("purple gelatinous cube") ||
                lname.Equals("green gelatinous cube") ||

                wall.IsMatch(lname))
            {
                player.SendInfo("Spawning " + name + " not allowed.");
                return;
            }

            // check spawn limit
            if (player.Client.Account.Rank < 5 && amount > 50)
            {
                player.SendError("Maximum spawn count is set to 50!");
                return;
            }

            // queue up mob spawn
            World w = RealmManager.GetWorld(player.Owner.Id);
            string announce = "Spawning " + ((amount > 1) ? amount + " " : "") + name + "...";
            w.BroadcastPacket(new NotificationPacket
            {
                Color = new ARGB(0xffff0000),
                ObjectId = player.Id,
                Text = announce
            }, null);
            w.BroadcastPacket(new TextPacket
            {
                Name = player.nName,
                Stars = player.Stars,
                BubbleTime = 0,
                Text = announce
            }, null);
            float x = player.X;
            float y = player.Y;
            w.Timers.Add(new WorldTimer(delay * 1000, (world, t) => // spawn mob in delay seconds
            {
                for (int i = 0; i < amount; i++)
                {
                    Entity entity = Entity.Resolve(objType);
                    entity.Move(x, y);
                    player.Owner.EnterWorld(entity);
                }
            }));
            
            // log event
            string dir = @"logs";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new StreamWriter(@"logs\SpawnLog.log", true))
            {
                writer.WriteLine(player.Name + " spawned " + amount + " " + name + "s");
            }

            player.SendInfo("Success!");
        }
    }

    internal class SSpawnCommand : ICommand
    {
        public string Command
        {
            get { return "sspawn"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            string name;
            short objType;
            int amount = 1;

            // multiple spawn check
            if (args.Length > 0 && !int.TryParse(args[0], out amount))
            {
                name = string.Join(" ", args);
                amount = 1;
            }
            else
            {
                name = string.Join(" ", args.Skip(1).ToArray());
            }

            // check to see if entity exists
            var icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
            // ^^ creates a new case insensitive dictionary based on the XmlDatas
            if (!icdatas.TryGetValue(name, out objType) ||
                !XmlDatas.ObjectDescs.ContainsKey(objType))
            {
                player.SendInfo("Unknown entity!");
                return;
            }

            // spawn mob
            for (int i = 0; i < amount; i++)
            {
                Entity entity = Entity.Resolve(objType);
                entity.Move(player.X, player.Y);
                player.Owner.EnterWorld(entity);
            }
            
            // log event
            string dir = @"logs";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new StreamWriter(@"logs\SSpawnLog.log", true))
            {
                writer.WriteLine(player.Name + " spawned " + amount + " " + name + "s");
            }

            player.SendInfo("Success!");
        }
    }

    internal class ArenaCommand : ICommand
    {
        public string Command
        {
            get { return "arena"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            Entity prtal = Entity.Resolve(0x1900);
            prtal.Move(player.X, player.Y);
            player.Owner.EnterWorld(prtal);
            World w = RealmManager.GetWorld(player.Owner.Id);
            w.Timers.Add(new WorldTimer(30*1000, (world, t) => //default portal close time * 1000

            {
                try
                {
                    w.LeaveWorld(prtal);
                }
                catch //couldn't remove portal, Owner became null. Should be fixed with RealmManager implementation
                {
                    Console.Out.WriteLine("Couldn't despawn portal.");
                }
            }));
            foreach (ClientProcessor i in RealmManager.Clients.Values)
                i.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Arena Opened by:" + " " + player.nName
                });
            foreach (ClientProcessor i in RealmManager.Clients.Values)
                i.SendPacket(new NotificationPacket
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "Arena Opened by " + player.nName
                });
        }
    }

    internal class GraveCommand : ICommand
    {
        public string Command
        {
            get { return "grave"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            string name = "";
            int maxed = 0;
            var rand = new Random();
            try
            {
                if (args.Length == 0)
                {
                    var list = new List<string>();
                    int num = 0;
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                    {
                        list.Add(i.Account.Name);
                        num++;
                    }
                    string[] array = list.ToArray();
                    var random = new Random();
                    int length = array.Length;
                    name = array[random.Next(0, length - 1)];
                    maxed = rand.Next(1, 11);
                }
                else if (args.Length == 1)
                {
                    name = args[0];
                    maxed = rand.Next(1, 11);
                }
                else if (args.Length == 2)
                {
                    name = args[0];
                    maxed = int.Parse(args[1]);
                }
                else
                {
                    var list = new List<string>();
                    int num = 0;
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                    {
                        list.Add(i.Account.Name);
                        num++;
                    }
                    string[] array = list.ToArray();
                    var random = new Random();
                    int length = array.Length;
                    name = array[random.Next(0, length - 1)];
                    maxed = rand.Next(1, 11);
                }
            }
            catch
            {
            }
            short objType;
            int? time;
            switch (maxed)
            {
                case 11:
                    objType = 0x0725;
                    time = 5*60*1000;
                    break;
                case 10:
                    objType = 0x0724;
                    time = 60*1000;
                    break;
                case 9:
                    objType = 0x0723;
                    time = 30*1000;
                    break;
                case 8:
                    objType = 0x0735;
                    time = null;
                    break;
                case 7:
                    objType = 0x0734;
                    time = null;
                    break;
                case 6:
                    objType = 0x072b;
                    time = null;
                    break;
                case 5:
                    objType = 0x072a;
                    time = null;
                    break;
                case 4:
                    objType = 0x0729;
                    time = null;
                    break;
                case 3:
                    objType = 0x0728;
                    time = null;
                    break;
                case 2:
                    objType = 0x0727;
                    time = null;
                    break;
                case 1:
                    objType = 0x0726;
                    time = null;
                    break;
                default:
                    objType = 0x0725;
                    time = 5*60*1000;
                    break;
            }
            var obj = new StaticObject(objType, time, true, time == null ? false : true, false);
            obj.Move(player.X, player.Y);
            obj.Name = name;
            player.Owner.EnterWorld(obj);
        }
    }

    internal class AddEffCommand : ICommand
    {
        public string Command
        {
            get { return "addeff"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /addeff <effect name or effect number>");
            }
            else
            {
                try
                {
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = (ConditionEffectIndex) Enum.Parse(typeof (ConditionEffectIndex), args[0].Trim(), true),
                        DurationMS = -1
                    });
                    {
                        player.SendInfo("Success!");
                    }
                }
                catch
                {
                    player.SendError("Invalid effect!");
                }
            }
        }
    }

    internal class RemoveEffCommand : ICommand
    {
        public string Command
        {
            get { return "remeff"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /remeff <effect name or effect number>");
            }
            else
            {
                try
                {
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = (ConditionEffectIndex) Enum.Parse(typeof (ConditionEffectIndex), args[0].Trim(), true),
                        DurationMS = 0
                    });
                    player.SendInfo("Success!");
                }
                catch
                {
                    player.SendError("Invalid effect!");
                }
            }
        }
    }

    internal class GiveCommand : ICommand
    {
        public string Command
        {
            get { return "give"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give [amount] <item name>");
            }
            else
            {
                string dir = @"logs";
                int num;
                int amount = 1;
                string name = "";
                if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
                {
                    name = string.Join(" ", args.Skip(1).ToArray()).Trim();
                    amount = int.Parse(args[0]);
                }
                else
                {
                    name = string.Join(" ", args.ToArray()).Trim();
                }

                short objType;
                var icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType))
                {
                    player.SendError("Unknown item!");
                    return;
                }
                if (!XmlDatas.ItemDescs[objType].Secret || player.Client.Account.Rank >= 5)
                {
                    for (int x = 0; x < amount; x++)
                    {
                        for (int i = 0; i < player.Inventory.Length; i++)
                        {
                            if (player.Inventory[i] == null)
                            {
                                player.Inventory[i] = XmlDatas.ItemDescs[objType];
                                player.UpdateCount++;
                                break;
                            }
                            if (i == 11 && x < amount)
                            {
                                player.SendError("Inventory full!");
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                using (var writer = new StreamWriter(@"logs\GiveLog.log", true))
                                {
                                    writer.WriteLine(player.Name + " gave themselves " + amount + " " + name);
                                }
                                return;
                            }
                        }
                    }
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    using (var writer = new StreamWriter(@"logs\GiveLog.log", true))
                    {
                        writer.WriteLine(player.Name + " gave themselves " + amount + " " + name);
                    }
                    player.SendInfo("Success!");
                }
                player.SendError("Item cannot be given!");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var writer = new StreamWriter(@"logs\GiveLog.log", true))
                {
                    writer.WriteLine(player.Name + " tried to give themselves " + amount + " " + name);
                }
            }
        }
    }

    internal class TpCommand : ICommand
    {
        public string Command
        {
            get { return "tp"; }
        }

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return;
                }
                player.Move(x + 0.5f, y + 0.5f);
                player.SetNewbiePeriod();
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
        }
    }

    internal class SetpieceCommand : ICommand
    {
        public string Command
        {
            get { return "setpiece"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /setpiece <setpiece ID>");
            }
            else
            {
                try
                {
                    var piece = (ISetPiece) Activator.CreateInstance(Type.GetType(
                        "wServer.realm.setpieces." + args[0]));
                    piece.RenderSetPiece(player.Owner, new IntPoint((int) player.X + 1, (int) player.Y + 1));

                    player.SendInfo("Success!");
                }
                catch
                {
                    player.SendError("Cannot apply setpiece!");
                }
            }
        }
    }

    internal class DebugCommand : ICommand
    {
        public string Command
        {
            get { return "debug"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            player.Client.SendPacket(new TextBoxPacket
            {
                Button1 = "I Understand",
                Button2 = "Cancel",
                Message = "This is a test to see if you can force a disconect using TextBoxPackets",
                Title = "Testing!",
                Type = "Test"
            });
        }
    }

    internal class KillAll : ICommand
    {
        public string Command
        {
            get { return "killall"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /killall <entity name>");
            }
            else
            {
                foreach (var i in player.Owner.Enemies)
                {
                    if ((i.Value.ObjectDesc != null) &&
                        (i.Value.ObjectDesc.ObjectId != null) &&
                        (i.Value.ObjectDesc.ObjectId.Contains(args[0])))
                    {
                        i.Value.Owner.LeaveWorld(i.Value);
                    }
                }
                player.SendInfo("Success!");
            }
        }
    }


    internal class Kick : ICommand
    {
        public string Command
        {
            get { return "kick"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <player name>");
            }
            else
            {
                try
                {
                    Player target = null;
                    if ((target = RealmManager.FindPlayer(string.Join(" ", args))) != null)
                    {
                        player.SendInfo("Player Disconnected");
                        target.Client.Disconnect();
                    }
                }
                catch
                {
                    player.SendError("Cannot kick!");
                }
            }
        }
    }

    internal class GetQuest : ICommand
    {
        public string Command
        {
            get { return "getquest"; }
        }

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            player.SendInfo("Loc: " + player.Quest.X + " " + player.Quest.Y);
        }
    }

    internal class OryxSay : ICommand
    {
        public string Command
        {
            get { return "osay"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <text>");
            }
            else
            {
                string saytext = string.Join(" ", args);
                player.SendEnemy("Oryx the Mad God", saytext);
            }
        }
    }

    internal class SWhoCommand : ICommand //get all players from all worlds (this may become too large!)
    {
        public string Command
        {
            get { return "swho"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            var sb = new StringBuilder("All conplayers: ");
            int players = 0;
            foreach (var w in RealmManager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    Player[] copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                            players++;
                        }
                    }
                }
            }
            sb.Append("players online: ");
            sb.Append(players.ToString());
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString);
        }
    }

    internal class Announcement : ICommand
    {
        public string Command
        {
            get { return "announce"; }
        } //msg all players in all worlds

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <text>");
            }
            else
            {
                string saytext = string.Join(" ", args);

                foreach (ClientProcessor i in RealmManager.Clients.Values)
                    i.SendPacket(new TextPacket
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "#Announcement",
                        Text = " " + saytext
                    });
            }
        }
    }

    internal class Summon : ICommand
    {
        public string Command
        {
            get { return "summon"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /summon <player name>");
            }
            else
            {
                Player plr = RealmManager.FindPlayer(args[0]);
                if (plr != null)
                {
                    plr.Client.Reconnect(new ReconnectPacket
                    {
                        Host = "",
                        Port = 2050,
                        GameId = player.Owner.Id,
                        Name = player.Owner.Name,
                        Key = Empty<byte>.Array,
                    });
                    return;
                }
                player.SendInfo(string.Format("Cannot summon, {0} not found!", args[0].Trim()));
            }
        }
    }

    internal class SummonAll : ICommand
    {
        public string Command
        {
            get { return "summonall"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            foreach (var w in RealmManager.Worlds)
            {
                foreach (var plr in w.Value.Players)
                {
                    if (plr.Value.Name != player.Name && plr.Value.Owner != player.Owner)
                    {
                        plr.Value.Client.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = player.Owner.Id,
                            Name = player.Owner.Name,
                            Key = Empty<byte>.Array,
                        });
                    }
                }
            }
        }
    }

    internal class TeleportAll : ICommand
    {
        public string Command
        {
            get { return "tpall"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
                foreach (var plr in player.Owner.Players)
                {
                    if(plr.Value.Name != player.Name)
                        plr.Value.Teleport(new RealmTime(), new TeleportPacket()
                        {
                            ObjectId = player.Id
                        });
                }
        }
    }

    internal class KillCommand : ICommand
    {
        public string Command
        {
            get { return "kill"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kill <player name>");
            }
            else
            {
                foreach (var w in RealmManager.Worlds)
                {
                    //string death = string.Join(" ", args);
                    World world = w.Value;
                    if (w.Key != 0) // 0 is limbo??
                    {
                        foreach (var i in world.Players)
                        {
                            //Unnamed becomes a problem: skip them
                            if (i.Value.nName.ToLower() == args[0].ToLower().Trim() && i.Value.NameChosen)
                            {
                                if (args.Length > 1)
                                {
                                    i.Value.Death(string.Join(" ", args, 1, args.Length - 1));
                                }
                                else
                                {
                                    i.Value.Death("Server Admin");
                                }

                                return;
                            }
                        }
                    }
                }
                player.SendInfo(string.Format("Cannot kill, {0} not found!", args[0].Trim()));
            }
        }
    }

    internal class RestartCommand : ICommand
    {
        public string Command
        {
            get { return "restart"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                foreach (var w in RealmManager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        world.BroadcastPacket(new TextPacket
                        {
                            Name = "#Announcement",
                            Stars = -1,
                            BubbleTime = 0,
                            Text =
                                "Server restarting soon. Please be ready to disconnect. Estimated server down time: 30 Seconds - 1 Minute"
                        }, null);
                    }
                }
            }
            catch
            {
                player.SendError("Cannot say that in announcement!");
            }
        }
    }

    internal class VitalityCommand : ICommand
    {
        public string Command
        {
            get { return "vit"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /vit <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[5] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class DefenseCommand : ICommand
    {
        public string Command
        {
            get { return "def"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /def <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[3] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class AttackCommand : ICommand
    {
        public string Command
        {
            get { return "att"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /att <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[2] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class DexterityCommand : ICommand
    {
        public string Command
        {
            get { return "dex"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /dex <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[7] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class LifeCommand : ICommand
    {
        public string Command
        {
            get { return "hp"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /hp <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[0] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class ManaCommand : ICommand
    {
        public string Command
        {
            get { return "mp"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /mp <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[1] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class SpeedCommand : ICommand
    {
        public string Command
        {
            get { return "spd"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /spd <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[4] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class WisdomCommand : ICommand
    {
        public string Command
        {
            get { return "wis"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /spd <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[6] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class Whitelist : ICommand
    {
        public string Command
        {
            get { return "whitelist"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /whitelist <username>");
            }
            try
            {
                using (var dbx = new Database())
                {
                    MySqlCommand cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET rank=1 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not whitelist!");
                    }
                    else
                    {
                        player.SendInfo("Account successfully whitelisted!");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(player.nName + " Has Whitelisted " + args[0]);
                        Console.ForegroundColor = ConsoleColor.White;

                        string dir = @"logs";
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        using (var writer = new StreamWriter(@"logs\WhitelistLog.log", true))
                        {
                            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " Has Whitelisted " + args[0]);
                        }
                    }
                    dbx.Dispose();
                }
            }
            catch
            {
                player.SendInfo("Could not whitelist!");
            }
        }
    }

    internal class BanC : ICommand
    {
        public string Command
        {
            get { return "banc"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
            if (plr != null)
            {
                string dir = @"logs";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var writer = new StreamWriter(@"logs\Bans.log", true))
                {
                    writer.WriteLine(player.Name + " Banned " + args[0] + ". ");
                }
            }
            string name = string.Join(" ", args);
            player.Client.SendPacket(new TextBoxPacket
            {
                Title = "Confirm Ban",
                Message = "Really ban " + name + "?",
                Button1 = "Confirm",
                Button2 = "Cancel",
                Type = "ConfirmBan:" + name
            });
        }
    }

    internal class Ban : ICommand
    {
        public string Command
        {
            get { return "ban"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /ban <username> <reason>");
            }
            else
            {
                try
                {
                    using (var dbx = new Database())
                    {
                        string dir = @"logs";
                        MySqlCommand cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not ban");
                        }
                        else
                        {
                            string reason = string.Join(" ", args.Skip(1).ToArray()).Trim();
                            Player target = null;
                            if ((target = RealmManager.FindPlayer(string.Join(" ", args[0]))) != null)
                            {
                                target.Client.Disconnect();
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Out.WriteLine(string.Join(" ", args) + " was Banned.");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            using (var writer = new StreamWriter(@"logs\Bans.log", true))
                            {
                                writer.WriteLine(player.Name + " Banned " + args[0] + ". " + "Reason: " + reason);
                            }
                            player.SendInfo("Account successfully Banned");
                        }
                        dbx.Dispose();
                    }
                }
                catch
                {
                    player.SendInfo("Could not ban");
                }
            }
        }
    }

    internal class UnBan : ICommand
    {
        public string Command
        {
            get { return "unban"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unban <username>");
            }
            try
            {
                using (var dbx = new Database())
                {
                    MySqlCommand cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not unban");
                    }
                    else
                    {
                        player.SendInfo("Account successfully Unbanned");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(args[1] + " was Unbanned.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    dbx.Dispose();
                }
            }
            catch
            {
                player.SendInfo("Could not unban, please unban in database");
            }
        }
    }

    internal class Rank : ICommand
    {
        public string Command
        {
            get { return "rank"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp(
                    "Usage: /rank <username> <number>\n0: Player\n1: Donator\n2: Game Master\n3: Developer\n4: Head Developer\n5: Admin");
            }
            else
            {
                if (int.Parse(args[1]) < player.Client.Account.Rank)
                {
                    try
                    {
                        using (var dbx = new Database())
                        {
                            MySqlCommand cmd = dbx.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
                            cmd.Parameters.AddWithValue("@rank", args[1]);
                            cmd.Parameters.AddWithValue("@name", args[0]);
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                player.SendInfo("Could not change rank");
                            }
                            else
                            {
                                player.SendInfo("Account rank successfully changed");
                            }
                            dbx.Dispose();
                        }
                    }
                    catch
                    {
                        player.SendInfo("Could not change rank, please change rank in database");
                    }
                }
                else
                {
                    player.SendError("You cannot set someone's rank higher than your own");
                }
            }
        }
    }

    internal class GuildRank : ICommand
    {
        public string Command
        {
            get { return "grank"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /grank <username> <number>");
            }
            else
            {
                try
                {
                    using (var dbx = new Database())
                    {
                        MySqlCommand cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
                        cmd.Parameters.AddWithValue("@guildRank", args[1]);
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not change guild rank. Use 10, 20, 30, 40, or 50 (invisible)");
                        }
                        else
                        {
                            player.SendInfo("Guild rank successfully changed");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Out.WriteLine(args[1] + "'s guild rank has been changed");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        dbx.Dispose();
                    }
                }
                catch
                {
                    player.SendInfo("Could not change rank, please change rank in database");
                }
            }
        }
    }

    internal class ChangeGuild : ICommand
    {
        public string Command
        {
            get { return "setguild"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /setguild <username> <guild id>");
            }
            else
            {
                try
                {
                    using (var dbx = new Database())
                    {
                        MySqlCommand cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET guild=@guild WHERE name=@name";
                        cmd.Parameters.AddWithValue("@guild", args[1]);
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not change guild.");
                        }
                        else
                        {
                            player.SendInfo("Guild successfully changed");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Out.WriteLine(args[1] + "'s guild has been changed");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        dbx.Dispose();
                    }
                }
                catch
                {
                    player.SendInfo(
                        "Could not change guild, please change in database.                                Use /setguild <username> <guild id>");
                }
            }
        }
    }

    internal class TqCommand : ICommand
    {
        public string Command
        {
            get { return "tq"; }
        }

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.Quest == null)
            {
                player.SendError("Player does not have a quest!");
            }
            else
                player.Move(player.X + 0.5f, player.Y + 0.5f);
            //player.SetNewbiePeriod();
            player.UpdateCount++;
            player.Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.Quest.X,
                    Y = player.Quest.Y
                }
            }, null);
            player.SendInfo("Success!");
        }
    }

    internal class GodMode : ICommand
    {
        public string Command
        {
            get { return "god"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0
                });
                player.SendInfo("Godmode Off");
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.SendInfo("Godmode On");
                foreach (ClientProcessor i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Godmode Activated"
                    });
            }
        }
    }

    internal class GetIPCommand : ICommand
    {
        public string Command
        {
            get { return "ip"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args));
            if (plr != null)
            {
                player.SendInfo(plr.Name + "'s IP: " + plr.Client.IP.Address);
                return;
            }
            foreach (var i in RealmManager.Worlds)
            {
                if (i.Key != 0 && i.Value.Id != player.Owner.Id)
                {
                    Player p = i.Value.GetUniqueNamedPlayerRough(string.Join(" ", args));
                    if (p != null)
                    {
                        player.SendInfo(plr.Name + "'s IP: " + plr.Client.IP.Address);
                        return;
                    }
                }
            }
            player.SendError("Could not find player.");
        }
    }

    internal class VaultVisit : ICommand
    {
        public string Command
        {
            get { return "vault"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                Player target = null;
                if ((target = RealmManager.FindPlayer(string.Join(" ", args))) != null)
                {
                    ClientProcessor player2 = target.Client;
                    Vault v = RealmManager.PlayerVault(player2);
                    int id = player2.Account.AccountId;
                    player.Client.Reconnect(new ReconnectPacket
                    {
                        Host = "",
                        Port = 2050,
                        GameId = v.Id,
                        Name = v.Name,
                        Key = Empty<byte>.Array,
                    });
                }
            }
            catch
            {
            }
        }
    }

    internal class StarCommand : ICommand
    {
        public string Command
        {
            get { return "stars"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /stars <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stars = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class LevelCommand : ICommand
    {
        public string Command
        {
            get { return "level"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /level <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Character.Level = int.Parse(args[0]);
                    player.Client.Player.Level = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class NameCommand : ICommand
    {
        public string Command
        {
            get { return "name"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Use /name <name>");
            }
            else if (args.Length == 1)
            {
                using (var db = new Database())
                {
                    MySqlCommand db1 = db.CreateQuery();
                    db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name;";
                    db1.Parameters.AddWithValue("@name", args[0]);
                    if ((int) (long) db1.ExecuteScalar() > 0)
                    {
                        player.SendError("Name Already In Use.");
                    }
                    else
                    {
                        db1 = db.CreateQuery();
                        db1.CommandText = "UPDATE accounts SET name=@name WHERE id=@accId";
                        db1.Parameters.AddWithValue("@name", args[0]);
                        db1.Parameters.AddWithValue("@accId", player.Client.Account.AccountId.ToString());
                        if (db1.ExecuteNonQuery() > 0)
                        {
                            player.Client.Player.Credits = db.UpdateCredit(player.Client.Account, -0);
                            player.Client.Player.Name = args[0];
                            player.Client.Player.NameChosen = true;
                            player.Client.Player.UpdateCount++;
                            player.SendInfo("Success!");
                        }
                        else
                        {
                            player.SendError("Internal Server Error Occurred.");
                        }
                    }
                    db1.Dispose();
                }
            }
        }
    }

    internal class RenameCommand : ICommand
    {
        public string Command
        {
            get { return "rename"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Use /rename <Old Player Name> <New Player Name>");
            }
            else if (args.Length == 2)
            {
                using (var db = new Database())
                {
                    MySqlCommand db1 = db.CreateQuery();
                    db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name;";
                    db1.Parameters.AddWithValue("@name", args[1]);
                    if ((int) (long) db1.ExecuteScalar() > 0)
                    {
                        player.SendError("Name Already In Use.");
                    }
                    else
                    {
                        db1 = db.CreateQuery();
                        db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name";
                        db1.Parameters.AddWithValue("@name", args[0]);
                        if ((int) (long) db1.ExecuteScalar() < 1)
                        {
                            player.SendError("Name Not Found.");
                        }
                        else
                        {
                            db1 = db.CreateQuery();
                            db1.CommandText = "UPDATE accounts SET name=@newName, namechosen=TRUE WHERE name=@oldName;";
                            db1.Parameters.AddWithValue("@newName", args[1]);
                            db1.Parameters.AddWithValue("@oldName", args[0]);
                            if (db1.ExecuteNonQuery() > 0)
                            {
                                foreach (var playerX in RealmManager.Worlds)
                                {
                                    if (playerX.Key != 0)
                                    {
                                        World world = playerX.Value;
                                        foreach (var p in world.Players)
                                        {
                                            Player Client = p.Value;
                                            if ((player.Name.ToLower() == args[0].ToLower()) && player.NameChosen)
                                            {
                                                player.Name = args[1];
                                                player.NameChosen = true;
                                                player.UpdateCount++;
                                                break;
                                            }
                                        }
                                    }
                                }
                                player.SendInfo("Success!");
                                //
                            }
                            else
                            {
                                player.SendError("Internal Server Error Occurred.");
                            }
                        }
                    }
                    db.Dispose();
                }
            }
        }
    }

    internal class messageCommand : ICommand
    {
        public string Command
        {
            get { return "message"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /message <title> <message>");
            }
            else
            {
                string title = string.Join(" ", args);
                string message = string.Join(" ", args.Skip(1).ToArray());
                foreach (ClientProcessor i in RealmManager.Clients.Values)
                    i.SendPacket(new TextBoxPacket
                    {
                        Title = args[0],
                        Message = message,
                        Button1 = "Ok",
                        Type = "GlobalMsg"
                    });
            }
        }
    }

    internal class GoldCommand : ICommand
    {
        public string Command
        {
            get { return "gold"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /gold <amount>");
                }
                else
                {
                    using (var db = new Database())
                    {
                        player.Credits = db.UpdateCredit(player.Client.Account, int.Parse(args[0]));
                        player.UpdateCount++;
                        db.Dispose();
                    }
                }
            }
            catch
            {
                player.SendError("Error");
            }
        }
    }

    internal class zTokenCommand : ICommand
    {
        public string Command
        {
            get { return "ztokens"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /ztokens <amount>");
                }
                else
                {
                    using (var db = new Database())
                    {
                        player.zTokens = db.UpdateCredit(player.Client.Account, int.Parse(args[0]));
                        player.UpdateCount++;
                        db.Dispose();
                    }
                }
            }
            catch
            {
                player.SendError("Error");
            }
        }
    }

    internal class FameCommand : ICommand
    {
        public string Command
        {
            get { return "fame"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /fame <amount>");
                }
                else
                {
                    using (var db = new Database())
                    {
                        player.CurrentFame = db.UpdateFame(player.Client.Account, int.Parse(args[0]));
                        player.UpdateCount++;
                        db.Dispose();
                    }
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class CurrencyCommand : ICommand
    {
        public string Command
        {
            get { return "currency"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    player.SendHelp("Use /currency <currency> <player> <amount>");
                }
                else if (args[0] == "ztokens")
                {
                    Player plr = RealmManager.FindPlayer(args[1]);
                    using (var db = new Database())
                    {
                        plr.zTokens = db.UpdateCredit(plr.Client.Account, int.Parse(args[2]));
                        plr.UpdateCount++;
                        db.Dispose();
                    }
                }
                else if (args[0] == "gold")
                {
                    using (var db = new Database())
                    {
                        Player plr = RealmManager.FindPlayer(args[1]);
                        plr.Credits = db.UpdateCredit(plr.Client.Account, int.Parse(args[2]));
                        plr.UpdateCount++;
                        db.Dispose();
                    }
                }
                else if (args[0] == "fame")
                {
                    Player plr = RealmManager.FindPlayer(args[1]);
                    using (var db = new Database())
                    {
                        plr.CurrentFame = db.UpdateFame(plr.Client.Account, int.Parse(args[2]));
                        plr.UpdateCount++;
                        db.Dispose();
                    }
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }

    internal class CloseRealmCommand : ICommand
    {
        public string Command
        {
            get { return "closerealm"; }
        }

        public int RequiredRank
        {
            get { return 2; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.Owner is GameWorld)
            {
                var gw = player.Owner as GameWorld;
                gw.Overseer.InitCloseRealm();
            }
        }
    }

    internal class AdminRoomCommand : ICommand
    {
        public string Command
        {
            get { return "adminroom"; }
        }

        public int RequiredRank
        {
            get { return 6; }
        }

        public void Execute(Player player, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = 2050,
                GameId = World.ADM_ID, // wtf? guess someone made the command but didn't make the map... TODO
                Name = "Admin Room",
                Key = Empty<byte>.Array,
            });
        }
    }

    internal class TakeCommand : ICommand
    {
        private RealmTime time;

        public string Command
        {
            get { return "take"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            Player plr = RealmManager.FindPlayer(args[0]);
            if (plr != null && plr.Owner == player.Owner)
            {
                player.RequestTake(time, new RequestTradePacket
                {
                    Name = plr.Name
                });
            }
        }
    }

    internal class BanIPCommand : ICommand
    {
        public string Command
        {
            get { return "ipban"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /ipban <username> <reason>");
            }
            else
            {
                foreach (var i in RealmManager.Worlds)
                {
                    if (i.Key != 0 && i.Value.Id != player.Owner.Id)
                    {
                        Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
                        if (plr != null)
                        {
                            using (var db = new Database())
                            {
                                string address = plr.Client.IP.Address;
                                MySqlCommand cmd = db.CreateQuery();
                                cmd.CommandText = "UPDATE ips SET banned=1 WHERE ip=@Adress";
                                cmd.Parameters.AddWithValue("Adress", plr.Client.IP.Address);
                                string reason = string.Join(" ", args.Skip(1).ToArray()).Trim();
                                if (cmd.ExecuteNonQuery() == 0)
                                {
                                    player.SendInfo("Could not ban");
                                }
                                else
                                {
                                    if (plr != null)
                                    {
                                        plr.Client.Disconnect();
                                    }
                                    string dir = @"logs";
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                    using (var writer = new StreamWriter(@"logs\Bans.log", true))
                                    {
                                        writer.WriteLine(player.Name + " IP-Banned " + args[0] + " (" + address + "). " +
                                                         "Reason: " + reason);
                                    }
                                    player.SendInfo("IP successfully Banned");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal class TradeCommand : ICommand
    {
        private RealmTime time;

        public string Command
        {
            get { return "trade"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /trade <username>");
            }
            else
            {
                Player plr = RealmManager.FindPlayer(args[0]);
                if (plr != null && plr.Owner == player.Owner)
                {
                    player.RequestTrade(time, new RequestTradePacket
                    {
                        Name = plr.Name
                    });
                }
            }
        }
    }

    internal class IPBanC : ICommand
    {
        public string Command
        {
            get { return "ipbanc"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
            if (plr != null)
            {
                string address = plr.Client.IP.Address;
                string dir = @"logs";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var writer = new StreamWriter(@"logs\Bans.log", true))
                {
                    writer.WriteLine(player.Name + " IP-Banned " + args[0] + " (" + address + "). ");
                }
            }
            string name = string.Join(" ", args);
            player.Client.SendPacket(new TextBoxPacket
            {
                Title = "Confirm IPBan",
                Message = "Really IPBan " + name + "?",
                Button1 = "Confirm",
                Button2 = "Cancel",
                Type = "ConfirmIPBan:" + name
            });
        }
    }

    internal class Mute : ICommand
    {
        public string Command
        {
            get { return "mute"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /mute <username>");
            }
            else
            {
                Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
                plr.muted = true;
            }
        }
    }

    internal class Unmute : ICommand
    {
        public string Command
        {
            get { return "unmute"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /unmute <username>");
            }
            else
            {
                Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
                plr.muted = false;
            }
        }
    }

    internal class SetTag : ICommand
    {
        public string Command
        {
            get { return "settag"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /settag [username] <tag>");
            }
            else
            {
                if (args.Length == 1)
                {
                    using (var db = new Database())
                    {
                        MySqlCommand cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET tag=@tag WHERE name=@name";
                        cmd.Parameters.AddWithValue("@name", player.nName);
                        cmd.Parameters.AddWithValue("@tag", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not set tag");
                        }
                        else
                        {
                            player.Name = "[" + args[0] + "] " + player.Client.Account.Name;
                            player.SendInfo("Tag succesfully changed");
                        }
                        db.Dispose();
                    }
                }
                else if (args.Length == 2)
                {
                    Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
                    if (plr != null)
                    {
                        using (var db = new Database())
                        {
                            MySqlCommand cmd = db.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET tag=@tag WHERE name=@name";
                            cmd.Parameters.AddWithValue("@name", args[0]);
                            cmd.Parameters.AddWithValue("@tag", args[1]);
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                player.SendInfo("Could not set tag");
                            }
                            else
                            {
                                plr.Name = "[" + args[1] + "] " + plr.Client.Account.Name;
                                player.SendInfo("Tag succesfully changed");
                            }
                            db.Dispose();
                        }
                    }
                    else
                    {
                        player.SendError("Could not find player");
                    }
                }
            }
        }
    }

    internal class RemoveTag : ICommand
    {
        public string Command
        {
            get { return "removetag"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                using (var db = new Database())
                {
                    MySqlCommand cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET tag=@tag WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", player.nName);
                    cmd.Parameters.AddWithValue("@tag", "");
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not remove tag");
                    }
                    else
                    {
                        player.Name = player.Client.Account.Name;
                        player.SendInfo("Tag succesfully removed");
                    }
                    db.Dispose();
                }
            }
            else if (args.Length == 1)
            {
                Player plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args[0]));
                if (plr != null)
                {
                    using (var db = new Database())
                    {
                        MySqlCommand cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET tag=@tag WHERE name=@name";
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        cmd.Parameters.AddWithValue("@tag", "");
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not remove tag");
                        }
                        else
                        {
                            plr.Name = plr.Client.Account.Name;
                            player.SendInfo("Tag succesfully removed");
                        }
                        db.Dispose();
                    }
                }
                else
                {
                    player.SendError("Could not find player");
                }
            }
        }
    }

    internal class Vanish : ICommand
    {
        public string Command
        {
            get { return "vanish"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            player.vanished = true;
            player.Owner.PlayersCollision.Remove(player);
            if (player.Pet != null)
            {
                player.Owner.LeaveWorld(player.Pet);
            }
        }
    }

    internal class Unvanish : ICommand
    {
        public string Command
        {
            get { return "unvanish"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            player.vanished = false;
        }
    }

    internal class Spectate : ICommand
    {
        public string Command
        {
            get { return "spectate"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible) &&
                (player.HasConditionEffect(ConditionEffects.Invisible)))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0
                });
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = 0
                });
                player.SendInfo("Spectate Off");
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = -1
                });
                player.SendInfo("Spectate On");
                foreach (ClientProcessor i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Spectate Activated"
                    });
            }
        }

        internal class AllOnlineCommand : ICommand
        {
            public string Command
            {
                get { return "online"; }
            }

            public int RequiredRank
            {
                get { return 3; }
            }

            public void Execute(Player player, string[] args)
            {
                try
                {
                    var sb = new StringBuilder("Users online: \r\n");
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                    {
                        if (i.Stage == ProtocalStage.Disconnected || i.Player == null || i.Player.Owner == null) continue;
                        sb.AppendFormat("{0}#{1}@{2}\r\n",
                            i.Account.Name,
                            i.Player.Owner.Name,
                            i.Socket.RemoteEndPoint);
                    }
                    player.SendInfo(sb.ToString());
                }
                catch
                {
                    player.SendError("Error!");
                }
            }
        }
    }
}