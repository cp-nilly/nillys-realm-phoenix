using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.svrPackets;
using wServer.cliPackets;
using System.Collections.Concurrent;
using wServer.realm.worlds;
using wServer.logic;
using wServer.realm.entities.player;
using db;

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        public void CreateGuild(RealmTime t, CreateGuildPacket pkt)
        {
            var GuildsActive = true;
            if (GuildsActive == false)
            {
                psr.SendPacket(new CreateGuildResultPacket()
                {
                    Success = false,
                    ResultMessage = "Guilds currently disabled!"
                });
                return;
            }
            else
            {
                try
                {
                    var name = pkt.Name.ToString();
                    if (psr.Account.Stats.Fame >= 1000)
                    {
                        if (name != "")
                        {
                            if (new Database().GetGuild(name) != null)
                            {
                                psr.SendPacket(new CreateGuildResultPacket()
                                {
                                    Success = false,
                                    ResultMessage = "Guild already exists!"
                                });
                                return;
                            }
                            using (var db1 = new Database())
                            {
                                try
                                {
                                    if (psr.Account.Guild.Name == "")
                                    {
                                        if (pkt.Name != "")
                                        {
                                            var g = db1.CreateGuild(psr.Account, pkt.Name);
                                            psr.Account.Guild.Name = g.Name;
                                            psr.Account.Guild.Rank = g.Rank;
                                            Guild = g.Name;
                                            GuildRank = g.Rank;
                                            psr.SendPacket(new NotificationPacket()
                                            {
                                                Text = "Created guild " + g.Name,
                                                Color = new ARGB(0xFF008800),
                                                ObjectId = Id
                                            });
                                            psr.SendPacket(new CreateGuildResultPacket()
                                            {
                                                Success = true,
                                                ResultMessage = "Success!"
                                            });
                                            CurrentFame = psr.Account.Stats.Fame = psr.Database.UpdateFame(psr.Account, -1000);
                                            UpdateCount++;
                                            return;
                                        }
                                        else
                                        {
                                            psr.SendPacket(new CreateGuildResultPacket()
                                            {
                                                Success = false,
                                                ResultMessage = "Guild name cannot be blank!"
                                            });
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        psr.SendPacket(new CreateGuildResultPacket()
                                        {
                                            Success = false,
                                            ResultMessage = "You cannot create a guild as a guild member!"
                                        });
                                        return;
                                    }
                                }
                                catch (Exception e)
                                {
                                    psr.SendPacket(new CreateGuildResultPacket()
                                    {
                                        Success = false,
                                        ResultMessage = e.Message
                                    });
                                    return;
                                }
                            }
                        }
                        else
                        {
                            psr.SendPacket(new CreateGuildResultPacket()
                            {
                                Success = false,
                                ResultMessage = "Name cannot be empty!"
                            });
                        }
                    }
                    else
                    {
                        psr.SendPacket(new CreateGuildResultPacket()
                        {
                            Success = false,
                            ResultMessage = "Not enough fame!"
                        });
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error at line 755 of Player.cs");
                    psr.SendPacket(new TextPacket()
                    {
                        Name = "",
                        Stars = -1,
                        BubbleTime = 0,
                        Text = "Error creating guild!"
                    });
                }
            }
        }
        public void JoinGuild(RealmTime t, JoinGuildPacket pkt)
        {
            var db = new Database();
            var gStruct = db.GetGuild(pkt.GuildName);
            if (psr.Player.Invited == false)
            {
                SendInfo("You need to be invited to join a guild!");
            }
            if (gStruct != null)
            {
                var g = db.ChangeGuild(psr.Account, gStruct.Id, 0, 0, false);
                if (g != null)
                {
                    psr.Account.Guild = g;
                    Guild = g.Name;
                    GuildRank = g.Rank;
                    UpdateCount++;
                    foreach (var p in RealmManager.GuildMembersOf(g.Name))
                    {
                        p.Client.SendPacket(new TextPacket()
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            //Name = "@" + psr.Account.Name + " has joined the guild!"
                            Name = "",
                            Recipient = "*Guild*",
                            Text = psr.Account.Name + " has joined the guild!"
                        });
                    }
                }
            }
        }
        public void InviteToGuild(RealmTime t, GuildInvitePacket pkt)
        {
            if (GuildRank >= 20)
            {
                foreach (var i in RealmManager.Clients.Values)
                {
                    foreach (var l in RealmManager.Worlds)
                    {
                        if (l.Key != 0)
                        {
                            foreach (var e in l.Value.Players)
                            {
                                if (e.Value.Name == pkt.Name)
                                {
                                    if (e.Value.Guild == "")
                                    {
                                        e.Value.Client.SendPacket(new InvitedToGuildPacket()
                                        {
                                            Name = psr.Account.Name,
                                            Guild = psr.Account.Guild.Name
                                        });
                                        i.Player.Invited = true;
                                        return;
                                    }
                                    else
                                    {
                                        SendError("Player is already in a guild!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                psr.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Members and initiates cannot invite!"
                });
            }

        }
        public string ResolveGuildChatName()
        {
            return Name;
        }
        public string ResolveRankName(int rank)
        {
            string name;
            switch (rank)
            {
                case 0:
                    name = "Initiate"; break;
                case 10:
                    name = "Member"; break;
                case 20:
                    name = "Officer"; break;
                case 30:
                    name = "Leader"; break;
                case 40:
                    name = "Founder"; break;
                default:
                    name = ""; break;
            }
            return name;
        }

        public void ChangeGuildRank(RealmTime t, ChangeGuildRankPacket pkt)
        {
            var pname = pkt.Name;
            var rank = pkt.GuildRank;
            if (GuildRank >= 20)
            {
                var other = RealmManager.FindPlayer(pname);
                if (other != null && other.Guild == Guild)
                {
                    var rankname = ResolveRankName(other.GuildRank);
                    var rankname2 = ResolveRankName(rank);
                    other.GuildRank = rank;
                    other.Client.Account.Guild.Rank = rank;
                    new Database().ChangeGuild(other.Client.Account, other.Client.Account.Guild.Id, other.GuildRank, other.Client.Account.Guild.Fame, false);
                    other.UpdateCount++;
                    foreach (var p in RealmManager.GuildMembersOf(Guild))
                    {
                        p.Client.SendPacket(new TextPacket()
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Recipient = "*Guild*",
                            Text = other.Client.Account.Name + " has been demoted to " + rankname2 + "."
                        });
                    }
                }
                else
                {
                    try
                    {
                        var db = new Database();
                        var acc = db.GetAccount(pname);
                        if (acc.Guild.Name == Guild)
                        {
                            var rankname = ResolveRankName(acc.Guild.Rank);
                            var rankname2 = ResolveRankName(rank);
                            db.ChangeGuild(acc, acc.Guild.Id, rank, acc.Guild.Fame, false);
                            foreach (var p in RealmManager.GuildMembersOf(Guild))
                            {
                                p.Client.SendPacket(new TextPacket()
                                {
                                    BubbleTime = 0,
                                    Stars = -1,
                                    Name = "",
                                    Recipient = "*Guild*",
                                    Text = acc.Name + " has been promoted to " + rankname2 + "."
                                });
                            }
                        }
                        else
                        {
                            psr.SendPacket(new TextPacket()
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "*Error*",
                                Text = "You can only change a player in your guild."
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        psr.SendPacket(new TextPacket()
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "*Error*",
                            Text = e.Message
                        });
                    }
                }
            }
            else
            {
                psr.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Members and initiates cannot promote!"
                });
            }
        }

        public void GuildRemove(RealmTime t, GuildRemovePacket pkt)
        {
            var pname = pkt.Name;
            try
            {
                var p = RealmManager.FindPlayer(pname);
                if (p != null && p.Guild == Guild)
                {
                    var db = new Database();
                    var g = db.ChangeGuild(p.Client.Account, p.Client.Account.Guild.Id, p.GuildRank, p.Client.Account.Guild.Fame, true);
                    p.Guild = "";
                    p.GuildRank = 0;
                    p.Client.Account.Guild = g;
                    p.UpdateCount++;
                    if (p != this)
                    {
                        p.SendGuild("You have been kicked from the guild.");
                        foreach (var pl in RealmManager.GuildMembersOf(Guild))
                            pl.SendGuild(p.nName + " has been kicked from the guild by " + nName + ".");
                    }
                    else
                    {
                        p.SendGuild("You have left the guild.");
                        foreach (var pl in RealmManager.GuildMembersOf(Guild))
                            pl.SendGuild(nName + " has left the guild.");
                    }
                }
                else
                {
                    try
                    {
                        var db = new Database();
                        var other = db.GetAccount(pname);
                        if (other.Guild.Name == Guild)
                        {
                            db.ChangeGuild(other, other.Guild.Id, other.Guild.Rank, other.Guild.Fame, true);
                            foreach (var pl in RealmManager.GuildMembersOf(Guild))
                                pl.SendGuild(pname + " has been kicked from the guild by " + nName + ".");
                        }
                    }
                    catch (Exception e)
                    {
                        psr.SendPacket(new TextPacket()
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "*Error*",
                            Text = e.Message
                        });
                    }
                }

            }
            catch (Exception e)
            {
                psr.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "*Error*",
                    Text = e.Message
                });
            }
        }
    }
}
