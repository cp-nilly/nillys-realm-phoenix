#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using db;
using db.data;
using wServer.cliPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.worlds;
using wServer.svrPackets;
using Mono.Game;

#endregion

namespace wServer.realm.commands
{
    internal class TutorialCommand : ICommand
    {
        public string Command
        {
            get { return "tutorial"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = 2050,
                GameId = World.TUT_ID,
                Name = "Tutorial",
                Key = Empty<byte>.Array,
            });
        }
    }

    internal class WhoCommand : ICommand
    {
        public string Command
        {
            get { return "who"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            var sb = new StringBuilder("Players online: ");
            Player[] copy = player.Owner.Players.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(copy[i].Name);
            }

            player.SendInfo(sb.ToString());
        }
    }

    internal class ServerCommand : ICommand
    {
        public string Command
        {
            get { return "server"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            player.SendInfo(player.Owner.Name);
        }
    }

    internal class PauseCommand : ICommand
    {
        public string Command
        {
            get { return "pause"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.Owner.Name != "Battle Arena" && player.Owner.Name != "Free Battle Arena")
            {
                if (player.HasConditionEffect(ConditionEffects.Paused))
                {
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Paused,
                        DurationMS = 0
                    });
                    player.SendInfo("Game resumed.");
                }
                else
                {
                    foreach (Enemy i in player.Owner.EnemiesCollision.HitTest(player.X, player.Y, 6).OfType<Enemy>())
                    {
                        if (i.ObjectDesc.Enemy)
                        {
                            player.SendInfo("Not safe to pause.");
                            return;
                        }
                    }
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Paused,
                        DurationMS = -1
                    });
                    player.SendInfo("Game paused.");
                }
            }
            else
            {
                player.SendError("You cannot pause in the arena");
            }
        }
    }

    internal class TeleportCommand : ICommand
    {
        public string Command
        {
            get { return "teleport"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (player.nName.ToLower() == args[0].ToLower())
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return;
                }
                if (player.Owner.AllowTeleport == false)
                {
                    player.SendInfo("You are not allowed to teleport in this area!");
                    return;
                }
                foreach (var i in player.Owner.Players)
                {
                    if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                    {
                        if (i.Value.HasConditionEffect(ConditionEffects.Invisible))
                        {
                            player.SendInfo("Could not teleport to this player.");
                            return;
                        }
                        foreach (Entity e in i.Value._clientEntities)
                        {
                            if (e is Enemy && Vector2.Distance(new Vector2(e.X, e.Y), new Vector2(player.X, player.Y)) < 3)
                            {
                                player.SendHelp("There are enemies near that player and a teleport may not be safe. Use /ftp <playername> to teleport anyway.");
                                return;
                            }
                        }
                        player.Teleport(new RealmTime(), new TeleportPacket
                        {
                            ObjectId = i.Value.Id
                        });
                        return;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch
            {
                player.SendHelp("Usage: /teleport <player name>");
            }
        }
    }

    internal class ForceTeleportCommand : ICommand
    {
        public string Command
        {
            get { return "ftp"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (player.nName.ToLower() == args[0].ToLower())
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return;
                }
                if (player.Owner.AllowTeleport == false)
                {
                    player.SendInfo("You are not allowed to teleport in this area!");
                    return;
                }
                foreach (var i in player.Owner.Players)
                {
                    if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                    {
                        if (i.Value.HasConditionEffect(ConditionEffects.Invisible))
                        {
                            player.SendInfo("Could not teleport to this player.");
                            return;
                        }
                        player.Teleport(new RealmTime(), new TeleportPacket()
                        {
                            ObjectId = i.Value.Id
                        });
                        return;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch
            {
                player.SendHelp("Usage: /ftp <player name>");
            }
        }
    }

    internal class TellCommand : ICommand
    {
        public string Command
        {
            get { return "tell"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                int sindex = 1;

                if (!(player.NameChosen))
                {
                    player.SendInfo(string.Format("Choose a name!"));
                    return;
                }

                var tags = new List<string>();
                foreach (var x in RealmManager.Worlds)
                {
                    foreach (var y in x.Value.Players)
                    {
                        if (y.Value.Client.Account.Tag != "" && y.Value.Client.Account.Tag != null)
                        {
                            tags.Add("[" + y.Value.Client.Account.Tag + "]");
                        }
                    }
                }
                string playername = args[0].Trim();
                if (tags.Contains(playername))
                {
                    playername = args[1];
                    sindex = 2;
                }

                if (player.nName.ToLower() == playername.ToLower())
                {
                    player.SendInfo(string.Format("You canot tell yourself!"));
                    return;
                }

                string saytext = string.Join(" ", args, sindex, args.Length - sindex);

                foreach (var w in RealmManager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0) // 0 is limbo??
                    {
                        foreach (var i in world.Players)
                        {
                            if (i.Value.nName.ToLower() == playername.ToLower().Trim() && i.Value.NameChosen)
                            {
                                if (saytext == "" || saytext == null)
                                {
                                    player.SendHelp("Usage: /tell <player name> <text>");
                                    return;
                                }
                                player.Client.SendPacket(new TextPacket //echo to self
                                {
                                    BubbleTime = 10,
                                    Stars = player.Stars,
                                    Name = player.Name,
                                    Recipient = i.Value.Name,
                                    ObjectId = player.Id,
                                    Text = saytext.ToSafeText()
                                });

                                i.Value.Client.SendPacket(new TextPacket //echo to /tell player
                                {
                                    BubbleTime = 10,
                                    Stars = player.Stars,
                                    Recipient = i.Value.nName,
                                    Name = player.Name,
                                    ObjectId = (i.Value.Owner.Id == player.Owner.Id ? player.Id : 0),
                                    Text = saytext.ToSafeText()
                                });
                                return;
                            }
                        }
                    }
                }
                player.SendInfo(string.Format("Cannot tell, {0} not found!", args[sindex - 1].Trim()));
            }
            catch
            {
                player.SendInfo("Cannot tell!");
            }
        }
    }

    internal class TCommand : ICommand
    {
        public string Command
        {
            get { return "t"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                int sindex = 1;

                if (!(player.NameChosen))
                {
                    player.SendInfo(string.Format("Choose a name!"));
                    return;
                }

                var tags = new List<string>();
                foreach (var x in RealmManager.Worlds)
                {
                    foreach (var y in x.Value.Players)
                    {
                        if (y.Value.Client.Account.Tag != "" && y.Value.Client.Account.Tag != null)
                        {
                            tags.Add("[" + y.Value.Client.Account.Tag + "]");
                        }
                    }
                }
                string playername = args[0].Trim();
                if (tags.Contains(playername))
                {
                    playername = args[1];
                    sindex = 2;
                }

                if (player.nName.ToLower() == playername.ToLower())
                {
                    player.SendInfo(string.Format("You canot tell yourself!"));
                    return;
                }

                string saytext = string.Join(" ", args, sindex, args.Length - sindex);

                foreach (var w in RealmManager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0) // 0 is limbo??
                    {
                        foreach (var i in world.Players)
                        {
                            if (i.Value.nName.ToLower() == playername.ToLower().Trim() && i.Value.NameChosen)
                            {
                                if (saytext == "" || saytext == null)
                                {
                                    player.SendHelp("Usage: /tell <player name> <text>");
                                    return;
                                }
                                player.Client.SendPacket(new TextPacket //echo to self
                                {
                                    BubbleTime = 10,
                                    Stars = player.Stars,
                                    Name = player.Name,
                                    Recipient = i.Value.Name,
                                    ObjectId = player.Id,
                                    Text = saytext.ToSafeText()
                                });

                                i.Value.Client.SendPacket(new TextPacket //echo to /tell player
                                {
                                    BubbleTime = 10,
                                    Stars = player.Stars,
                                    Recipient = i.Value.nName,
                                    Name = player.Name,
                                    ObjectId = (i.Value.Owner.Id == player.Owner.Id ? player.Id : 0),
                                    Text = saytext.ToSafeText()
                                });
                                return;
                            }
                        }
                    }
                }
                player.SendInfo(string.Format("Cannot tell, {0} not found!", args[sindex - 1].Trim()));
            }
            catch
            {
                player.SendInfo("Cannot tell!");
            }
        }
    }

    internal class DyeCommand : ICommand
    {
        public string Command
        {
            get { return "dye"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            string name = string.Join(" ", args.ToArray()).Trim();
            short objType;
            if (!XmlDatas.IdToType.TryGetValue(name, out objType))
            {
                player.SendInfo("Unknown dye!");
                return;
            }
            try
            {
                if (XmlDatas.TypeToElement[objType].Element("Class").Value == "Dye")
                {
                    for (int i = 0; i < player.Inventory.Length; i++)
                        if (player.Inventory[i] == null)
                        {
                            player.Inventory[i] = XmlDatas.ItemDescs[objType];
                            player.UpdateCount++;
                            return;
                        }
                }
                else
                {
                    player.SendInfo("Unknown dye!");
                }
            }
            catch
            {
            }
        }
    }

    internal class VisitCommand : ICommand
    {
        public string Command
        {
            get { return "visit"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            string name = string.Join(" ", args.ToArray()).Trim();
            try
            {
                GlobalPlayerData PlayerData = PlayerDataList.GetData(player.Client.Account.Name);
                foreach (var w in RealmManager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        foreach (var i in world.Players)
                        {
                            if (i.Value.Client.Account.Name.ToLower() == name.ToLower())
                            {
                                GlobalPlayerData iPlayerData = PlayerDataList.GetData(i.Value.Client.Account.Name);
                                if (!(player.Client.Account.Rank > 2))
                                {
                                    if (world.Name != "Vault")
                                    {
                                        if (world.Name != "Guild Hall")
                                        {
                                            TryJoin(player, iPlayerData, world, i.Value);
                                            return;
                                        }
                                        if ((world as GuildHall).Guild == player.Guild)
                                        {
                                            TryJoin(player, iPlayerData, world, i.Value);
                                            return;
                                        }
                                        player.SendInfo("Player is in " + i.Value.Guild + "'s guild hall!");
                                        return;
                                    }
                                    if (world.Name == "Vault")
                                    {
                                        player.SendInfo("Player is in Vault!");
                                        return;
                                    }
                                    if (world.Name == "Guild Hall")
                                    {
                                        player.SendInfo("Player is in Guild Hall!");
                                        return;
                                    }
                                    if (!iPlayerData.UsingGroup)
                                    {
                                        player.Client.Reconnect(new ReconnectPacket
                                        {
                                            Host = "",
                                            Port = 2050,
                                            GameId = world.Id,
                                            Name = i.Value.Name + "'s Vault",
                                            Key = Empty<byte>.Array,
                                        });
                                        return;
                                    }
                                    foreach (var o in iPlayerData.JGroup)
                                    {
                                        if (o.Value == player.Client.Account.Name.ToLower())
                                        {
                                            player.Client.Reconnect(new ReconnectPacket
                                            {
                                                Host = "",
                                                Port = 2050,
                                                GameId = world.Id,
                                                Name = i.Value.Name + "'s Vault",
                                                Key = Empty<byte>.Array,
                                            });
                                            return;
                                        }
                                    }
                                    player.SendInfo("Not in " + i.Value.Client.Account.Name + "'s group!");
                                    return;
                                }
                                player.Client.Reconnect(new ReconnectPacket
                                {
                                    Host = "",
                                    Port = 2050,
                                    GameId = world.Id,
                                    Name = i.Value.Owner.Name,
                                    Key = Empty<byte>.Array,
                                });
                                return;
                            }
                        }
                    }
                }
                player.SendHelp("Use /visit <playername>");
            }
            catch
            {
                player.SendInfo("Unexpected error in command!");
            }
        }

        public bool TryJoin(Player player, GlobalPlayerData iPlayerData, World world, Player i)
        {
            if (world.Id == -60)
            {
                player.SendInfo("Sorry but that user is in a restricted area!");
                return true;
            }
            if (world.Name == "Admin Room")
            {
                if (!iPlayerData.Solo || player.Client.Account.Rank > 3)
                {
                    if (!iPlayerData.UsingGroup)
                    {
                        player.Client.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                        return true;
                    }
                    foreach (var o in iPlayerData.JGroup)
                    {
                        if (o.Value == player.Client.Account.Name.ToLower())
                        {
                            player.Client.Reconnect(new ReconnectPacket
                            {
                                Host = "",
                                Port = 2050,
                                GameId = world.Id,
                                Name = world.Name,
                                Key = Empty<byte>.Array,
                            });
                            return true;
                        }
                    }
                    player.SendInfo("Not in " + i.Client.Account.Name + "'s group!");
                    return true;
                }
                player.SendInfo("Player is going solo!");
                return true;
            }
            player.SendInfo("Sorry but that user is in a restricted area!");
            return true;
        }
    }

    internal class GroupCommand : ICommand
    {
        public string Command
        {
            get { return "group"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                GlobalPlayerData PlayerData = PlayerDataList.GetData(player.Client.Account.Name);
                if (args.Length > 0)
                {
                    string subcommand = args[0];
                    if (subcommand == "list")
                    {
                        string glist = "Players in your group: ";
                        foreach (var i in PlayerData.JGroup)
                        {
                            if (glist != "Players in your group: ")
                            {
                                glist = glist + ", " + i;
                            }
                            else
                            {
                                glist = glist + i;
                            }
                        }
                        player.SendInfo(glist);
                    }
                    else if (subcommand == "add" && args.Length > 1)
                    {
                        foreach (var i in PlayerData.JGroup)
                        {
                            if (i.Value == args[1].ToLower())
                            {
                                player.SendInfo("Player already added!");
                                return;
                            }
                        }
                        PlayerData.JGroup.TryAdd(PlayerData.JGroup.Count, args[1].ToLower());
                        player.SendInfo("Added " + args[1] + "!");
                    }
                    else if (subcommand == "del" && args.Length > 1)
                    {
                        int remc = 0;
                        foreach (var i in PlayerData.JGroup)
                        {
                            if (i.Value == args[1].ToLower())
                            {
                                string absolutelynothingdisregardthis;
                                player.SendInfo("Removed player " + i.Value + "!");
                                remc++;
                                PlayerData.JGroup.TryRemove(i.Key, out absolutelynothingdisregardthis);
                            }
                        }
                        if (remc < 1)
                        {
                            player.SendInfo("Player not found!");
                        }
                    }
                }
                else
                {
                    if (PlayerData.UsingGroup)
                    {
                        PlayerData.UsingGroup = false;
                        player.SendInfo("Group-only join disabled!");
                    }
                    else
                    {
                        PlayerData.UsingGroup = true;
                        player.SendInfo("Group-only join enabled!");
                    }
                }
            }
            catch
            {
                player.SendInfo("Unexpected error in command!");
            }
        }
    }

    internal class SoloCommand : ICommand
    {
        public string Command
        {
            get { return "solo"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            GlobalPlayerData PlayerData = PlayerDataList.GetData(player.Client.Account.Name);
            if (PlayerData.Solo)
            {
                PlayerData.Solo = false;
                player.SendInfo("Solo disabled! People can now join you!");
            }
            else
            {
                PlayerData.Solo = true;
                player.SendInfo("Solo enabled! People can no longer join you!");
            }
        }
    }

    internal class ShopCommand : ICommand
    {
        public string Command
        {
            get { return "shop"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            string evar = string.Join(" ", args.ToArray()).Trim();
            if (args.Length > 0)
            {
                World shop = RealmManager.AddWorld(new ShopMap(""));
                RealmManager.ShopWorlds.TryGetValue(evar, out shop);
                player.Client.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = 2050,
                    GameId = shop.Id,
                    Name = "Shop",
                    Key = Empty<byte>.Array
                });
            }
            else
            {
                string shopnames = "";
                string tname = "";
                foreach (var i in MerchantLists.shopLists)
                {
                    if (shopnames == "")
                    {
                        tname = i.Key;
                        tname.Insert(0, tname[0].ToString().ToUpper());
                        shopnames = i.Key;
                    }
                    else
                    {
                        tname = i.Key;
                        tname.Insert(0, tname[0].ToString().ToUpper());
                        shopnames += ", " + i.Key;
                    }
                }
                player.SendInfo("Shops: " + shopnames);
            }
        }
    }

    internal class ListCommands : ICommand
    {
        public string Command
        {
            get { return "commands"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            var cmds = new Dictionary<string, ICommand>();
            Type t = typeof (ICommand);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    var instance = (ICommand) Activator.CreateInstance(i);
                    cmds.Add(instance.Command, instance);
                }
            var sb = new StringBuilder("");
            ICommand[] copy = cmds.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append("  |  ");
                sb.Append(copy[i].Command);
            }

            player.Client.SendPacket(new TextBoxPacket
            {
                Title = "Commands:",
                Message = (sb.ToString()),
                Button1 = "Ok"
            });
        }
    }

    //class ListCommands : ICommand
    //{
    //    public string Command { get { return "commands"; } }
    //    public int RequiredRank { get { return 0; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        Dictionary<string, ICommand> cmds = new Dictionary<string, ICommand>();
    //        var t = typeof(ICommand);
    //        foreach (var i in t.Assembly.GetTypes())
    //            if (t.IsAssignableFrom(i) && i != t)
    //            {
    //                var instance = (ICommand)Activator.CreateInstance(i);
    //                cmds.Add(instance.Command, instance);
    //            }
    //        StringBuilder sb = new StringBuilder("Commands: ");
    //        var copy = cmds.Values.ToArray();
    //        for (int i = 0; i < copy.Length; i++)
    //        {
    //            if (i != 0) sb.Append(", ");
    //            sb.Append(copy[i].Command);
    //        }

    //        player.SendInfo(sb.ToString());
    //    }
    //}


    internal class statsCommand : ICommand
    {
        public string Command
        {
            get { return "stats"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            foreach (ClientProcessor i in RealmManager.Clients.Values)
                i.SendPacket(new NotificationPacket
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "HP:" + player.HP + " " + "MP:" + player.MP + " " + "Fame:" + " " + player.Fame
                });
            player.SendInfo("HP:" + player.HP + " " + "MP:" + player.MP + " " + "Att:" + " " + player.Stats[2] + " " +
                            "Def:" + " " + player.Stats[3] + " " + "Spd:" + " " + player.Stats[4] + " " + "Vit:" + " " +
                            player.Stats[5] + " " + "Wis:" + " " + player.Stats[6] + " " + "Dex:" + " " +
                            player.Stats[7]);
        }
    }

    internal class sayCommand : ICommand
    {
        public string Command
        {
            get { return "say"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /say <saytext>");
            }
            else
            {
                string saytext = string.Join(" ", args);
                foreach (ClientProcessor i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = saytext
                    });
            }
        }
    }

    internal class AFKCommand : ICommand
    {
        public string Command
        {
            get { return "afk"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.Owner.Name != "Battle Arena" && player.Owner.Name != "Free Battle Arena")
            {
                if (player.HasConditionEffect(ConditionEffects.Paused))
                {
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                        i.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0xff00ff00),
                            ObjectId = player.Id,
                            Text = "Active"
                        });
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Paused,
                        DurationMS = 0
                    });
                    player.SendInfo("Active!");
                }
                else
                {
                    foreach (Enemy i in player.Owner.EnemiesCollision.HitTest(player.X, player.Y, 8).OfType<Enemy>())
                    {
                        if (i.ObjectDesc.Enemy)
                        {
                            player.SendInfo("Not safe to go AFK.");
                            return;
                        }
                    }
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                        i.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0xff00ff00),
                            ObjectId = player.Id,
                            Text = "AFK"
                        });
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Paused,
                        DurationMS = -1
                    });
                    player.SendInfo("AFK!");
                }
            }
            else
            {
                player.SendInfo("You cannot pause in the arena");
            }
        }
    }

    internal class ArenasCommand : ICommand
    {
        public string Command
        {
            get { return "arenas"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            var Arenas = new List<BattleArenaMap>();
            var ArenaTexts = new List<string>();

            ArenaTexts.Add("");

            foreach (var w in RealmManager.Worlds)
            {
                World world = w.Value;
                if (w.Value.Name == "Battle Arena" && w.Value.Players.Count > 0 ||
                    w.Value.Name == "Free Battle Arena" && w.Value.Players.Count > 0)
                {
                    Arenas.Add(w.Value as BattleArenaMap);
                }
            }
            if (Arenas.Count > 0)
            {
                foreach (BattleArenaMap w in Arenas)
                {
                    string ctext = "Wave " + w.Wave + " - {0} {1}";
                    var players = new List<string>();
                    int solo = 0;
                    foreach (var p in w.Players)
                    {
                        players.Add(p.Value.Name);
                        if (PlayerDataList.GetData(p.Value.Client.Account.Name).Solo)
                            solo++;
                    }
                    if (players.Count > 0)
                    {
                        ArenaTexts.Add(string.Format(ctext, string.Join(", ", players.ToArray()),
                            solo == players.Count ? " (SOLO)" : ""));
                    }
                }
            }

            if (ArenaTexts.Count == 1)
                ArenaTexts.Add("None");

            //player.SendInfo(string.Join("\n", ArenaTexts.ToArray()));

            player.Client.SendPacket(new TextBoxPacket
            {
                Title = "Current Arenas:",
                Message = string.Join("\n", ArenaTexts.ToArray()),
                Button1 = "Ok"
            });
        }
    }

    //class ArenasCommand : ICommand 
    //{
    //    public string Command { get { return "arenas"; } }
    //    public int RequiredRank { get { return 0; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        List<BattleArenaMap> Arenas = new List<BattleArenaMap>();
    //        List<string> ArenaTexts = new List<string>();

    //        ArenaTexts.Add("Current arenas:");

    //        foreach (var w in RealmManager.Worlds)
    //        {
    //            World world = w.Value;
    //            if (w.Value.Name == "Battle Arena" && w.Value.Players.Count > 0)
    //            {
    //                Arenas.Add(w.Value as BattleArenaMap);
    //            }
    //        }
    //        if (Arenas.Count > 0)
    //        {
    //            foreach (var w in Arenas)
    //            {
    //                string ctext = "Wave "+ w.Wave +" - {0} {1}";
    //                List<string> players = new List<string>();
    //                int solo = 0;
    //                foreach (var p in w.Players)
    //                {
    //                    players.Add(p.Value.Name);
    //                    if (PlayerDataList.GetData(p.Value.Client.Account.Name).Solo)
    //                        solo++;
    //                }
    //                if (players.Count > 0)
    //                {
    //                    ArenaTexts.Add(string.Format(ctext, string.Join(", ", players.ToArray()), solo == players.Count ? " (SOLO)" : ""));
    //                }
    //            }
    //        }

    //        if(ArenaTexts.Count == 1)
    //            ArenaTexts.Add("None");

    //        player.SendInfo(string.Join("\n", ArenaTexts.ToArray()));
    //    }
    //}

    internal class LeaderboardCommand : ICommand
    {
        public string Command
        {
            get { return "leaderboard"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            using (var db = new Database())
            {
                string[] leaderboardInfo = db.GetArenaLeaderboards();
                player.Client.SendPacket(new TextBoxPacket
                {
                    Title = "Arena Leaderboard",
                    Message = string.Join("\n", leaderboardInfo),
                    Button1 = "Ok"
                });
            }
        }
    }

    internal class GuildLeaderboardCommand : ICommand
    {
        public string Command
        {
            get { return "gleaderboard"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            using (var db = new Database())
            {
                string[] leaderboardInfo = db.GetGuildLeaderboards();
                player.Client.SendPacket(new TextBoxPacket
                {
                    Title = "Guilds",
                    Message = string.Join("\n", leaderboardInfo),
                    Button1 = "Ok"
                });
            }
        }
    }

    internal class SellCommand : ICommand
    {
        public string Command
        {
            get { return "sell"; }
        }

        public int RequiredRank
        {
            get { return 5; }
        }

        public void Execute(Player player, string[] args)
        {
            try
            {
                player.Decision = 0;
                player.price = new Prices();
                var slotList = new List<int>();
                var slotList2 = new List<int>();
                for (int i = 0; i < args.Length; i++)
                    if (!slotList.Contains(Convert.ToInt32(args[i])))
                        slotList.Add(Convert.ToInt32(args[i]));
                if (slotList.Count < 1)
                    throw new Exception();
                foreach (int i in slotList)
                    if (!(i < 0) && !(i > 8))
                    {
                        int realslot = i + 3;
                        if (player.Inventory[realslot] != null)
                        {
                            slotList2.Add(realslot);
                        }
                    }
                player.price.SellSlots = slotList2;
                if (!player.price.HasPrices(player))
                {
                    player.SendInfo("No prices for specified items!");
                }
                else
                {
                    var msgSlots = new List<int>();
                    foreach (int i in player.price.SellSlots)
                        try
                        {
                            msgSlots.Add(i - 3);
                        }
                        catch
                        {
                        }
                    player.SendInfo("Slots being sold: [" + string.Join(", ", msgSlots) + "]");
                    player.SendInfo("You gain " + player.price.GetPrices(player) +
                                    " fame from these items. Sell them?\nType /yes or /no");
                    player.Decision = 2;
                }
            }
            catch
            {
                player.SendHelp("Usage: /sell <slot #1> <slot #2> etc.");
            }
        }
    }


    //class ForgeListCommand : ICommand
    //{
    //    public string Command { get { return "forgelist"; } }
    //    public int RequiredRank { get { return 0; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        List<string> itemNames = new List<string>();
    //        Combinations c = new Combinations();
    //        foreach (var i in c.combos)
    //        {
    //            itemNames.Add(i.Value.Item1);
    //        }
    //        player.SendInfo("These are the current items that can be forged:\n" + string.Join(", ", itemNames.ToArray()));
    //    }
    //}

    internal class PremChat : ICommand
    {
        public string Command
        {
            get { return "d"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Use /d <text>");
            }

            else if (player.Client.Account.Rank >= 2)
            {
                try
                {
                    string saytext = string.Join(" ", args);

                    foreach (var w in RealmManager.Worlds)
                    {
                        World world = w.Value;
                        if (w.Key != 0)
                        {
                            foreach (var i in world.Players)
                            {
                                if (i.Value.Client.Account.Rank >= 2)
                                {
                                    i.Value.Client.SendPacket(new TextPacket
                                    {
                                        BubbleTime = 10,
                                        ObjectId = player.Id,
                                        Stars = player.Stars,
                                        Name = "#[Premium] " + player.nName,
                                        Text = " " + saytext
                                    });
                                }
                            }
                        }
                    }
                }
                catch
                {
                    player.SendInfo("Cannot premium chat!");
                }
            }
            else
                player.SendInfo("You need to be a rank " + RequiredRank + " or higher to use this command.");
        }
    }

    internal class PackCommand : ICommand
    {
        public string Command
        {
            get { return "b"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.tradeTarget == null)
            {
                if (player.SwapBackpack(Convert.ToInt32(args[0])))
                {
                    player.SendInfo("Switched to backpack #" + args[0]);
                }
                else
                {
                    player.SendInfo("Backpack #" + args[0] + " does not exist!");
                }
            }
            else
            {
                player.SendError("Cannot switch backpack while trading!");
            }
        }
    }

    internal class Pack2Command : ICommand
    {
        public string Command
        {
            get { return "bp"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.tradeTarget == null)
            {
                if (player.SwapBackpack(Convert.ToInt32(args[0])))
                {
                    player.SendInfo("Switched to backpack #" + args[0]);
                }
                else
                {
                    player.SendInfo("Backpack #" + args[0] + " does not exist!");
                }
            }
            else
            {
                player.SendError("Cannot switch backpack while trading!");
            }
        }
    }

    internal class NothingCommand : ICommand
    {
        public string Command
        {
            get { return "nothing"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            player.Client.SendPacket(new NotificationPacket
            {
                Color = new ARGB(0xFFFFFF),
                ObjectId = player.Id,
                Text = "Nothing"
            });
        }
    }
}