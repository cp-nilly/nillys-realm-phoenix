#region

using System.Collections.Generic;
using System.Linq;
using db;
using MySql.Data.MySqlClient;
using wServer.cliPackets;
using wServer.logic.loot;
using wServer.realm.worlds;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        public string[] TierItems1;

        public void TextBoxButton(TextBoxButtonPacket pkt)
        {
            string type = pkt.Type;

            if (type == "test")
            {
                psr.SendPacket(new TextBoxPacket
                {
                    Button1 = "Yes",
                    Button2 = "No",
                    Message = "Do you want to enter the testing arena?",
                    Title = "Testing Arena Confirmation",
                    Type = "EnterTestArena"
                });
            }
            if (type == "NewClient")
            {
                psr.Disconnect();
            }
            if (type == "DecideArena")
            {
                if (pkt.Button == 1)
                {
                    psr.SendPacket(new TextBoxPacket
                    {
                        Button1 = "Enter",
                        Button2 = "Cancel",
                        Message = "Host an arena at the price of x fame?",
                        Title = "Arena Host Confirmation",
                        Type = "EnterArena2"
                    });
                }
                else
                {
                    psr.SendPacket(new TextBoxPacket
                    {
                        Button1 = "Enter",
                        Button2 = "Cancel",
                        Message = "Enter the arena solo at the price of 150 fame?",
                        Title = "Solo Arena Confirmation",
                        Type = "EnterArena1"
                    });
                }
            }
            if (type.Split(':')[0] == "ConfirmBan")
            {
                string pName = type.Split(':')[1];
                if (pkt.Button == 2)
                    return;
                if (psr.Account.Rank < 2)
                {
                    SendError("You do not have permission to ban!");
                    return;
                }
                Player target;
                if ((target = RealmManager.FindPlayer(pName)) != null)
                {
                    if (target.Client.Account.Rank > psr.Account.Rank)
                    {
                        SendError("You cannot ban someone higher than you!");
                        return;
                    }
                    MySqlCommand cmd = Client.Database.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", target.Client.Account.Name);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        SendError("Ban failed for some reason");
                        return;
                    }
                    target.Client.Disconnect();
                    SendInfo(pName + " successfully banned!");
                }
            }
            if (type.Split(':')[0] == "ConfirmIPBan")
            {
                string pName = type.Split(':')[1];
                if (pkt.Button == 2)
                    return;
                if (psr.Account.Rank < 2)
                {
                    SendError("You do not have permission to ban!");
                    return;
                }
                Player target;
                if ((target = RealmManager.FindPlayer(pName)) != null)
                {
                    if (target.Client.Account.Rank > psr.Account.Rank)
                    {
                        SendError("You cannot ban someone higher than you!");
                        return;
                    }
                    
                    string address = target.Client.IP.Address;
                    MySqlCommand cmd = Client.Database.CreateQuery();
                    cmd.CommandText = "UPDATE ips SET banned=1 WHERE ip=@Adress";
                    cmd.Parameters.AddWithValue("Adress", target.Client.IP.Address);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        SendInfo("Could not ban");
                        return;
                    }

                    target.Client.Disconnect();
                    SendInfo("IP successfully Banned");
                }
            }


            if (type == "EnterTestArena")
            {
                if (pkt.Button == 1)
                {
                    if (Client.Account.Stats.Fame >= 150)
                    {
                        /*RealmManager.PlayerWorldMapping.TryAdd(this.AccountId, Owner);
                        psr.Reconnect(new ReconnectPacket()
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                        */
                    }
                    else
                    {
                        SendHelp("Not Enough Fame");
                    }
                }
                else
                {
                    SendInfo("Cancelled entering arena.");
                }
            }
            if (type == "EnterArena1")
            {
                if (pkt.Button == 1)
                {
                    if (Client.Account.Stats.Fame >= 150)
                    {
                        Client.Database.UpdateFame(psr.Account, -150);

                        World world = RealmManager.GetWorld(World.NEXUS_ID);
                        bool fworld = false;
                        foreach (var i in RealmManager.Worlds)
                            if (i.Value is BattleArenaMap)
                                if ((i.Value as BattleArenaMap).Joinable)
                                {
                                    world = i.Value;
                                    fworld = true;
                                    break;
                                }
                        if (!fworld)
                            world = RealmManager.AddWorld(new BattleArenaMap());

                        psr.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                    }
                    else
                    {
                        SendHelp("Not Enough Fame");
                    }
                }
                else
                {
                    SendInfo("Cancelled entering arena.");
                }
            }
            if (type == "EnterArena2")
            {
                if (pkt.Button == 1)
                {
                    World world = RealmManager.GetWorld(World.NEXUS_ID);
                    bool fworld = false;
                    foreach (var i in RealmManager.Worlds)
                        if (i.Value is BattleArenaMap2)
                            if ((i.Value as BattleArenaMap2).Joinable)
                            {
                                world = i.Value;
                                fworld = true;
                                break;
                            }
                    if (!fworld)
                        world = RealmManager.AddWorld(new BattleArenaMap2());

                    psr.Reconnect(new ReconnectPacket
                    {
                        Host = "",
                        Port = 2050,
                        GameId = world.Id,
                        Name = world.Name,
                        Key = Empty<byte>.Array,
                    });
                }
                else
                {
                    SendInfo("Cancelled entering arena.");
                }
            }
            if (type == "SheepHerding")
            {
                if (pkt.Button == 1)
                {
                    if (Client.Account.Stats.Fame >= 500)
                    {
                        Client.Database.UpdateFame(psr.Account, -500);

                        World world = RealmManager.GetWorld(World.NEXUS_ID);
                        bool fworld = false;
                        foreach (var i in RealmManager.Worlds)
                            if (i.Value is Herding)
                                if ((i.Value as Herding).Joinable)
                                {
                                    world = i.Value;
                                    fworld = true;
                                    break;
                                }
                        if (!fworld)
                            world = RealmManager.AddWorld(new Herding());

                        psr.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                    }
                    else
                    {
                        SendHelp("Not Enough Fame");
                    }
                }
                else
                {
                    SendInfo("Cancelled entering sheep herding.");
                }
            }
            if (type == "Zombies")
            {
                if (pkt.Button == 1)
                {
                    if (Client.Account.Stats.Fame >= 100)
                    {
                        Client.Database.UpdateFame(psr.Account, -100);

                        World world = RealmManager.GetWorld(World.NEXUS_ID);
                        bool fworld = false;
                        foreach (var i in RealmManager.Worlds)
                            if (i.Value is ZombieMG)
                                if ((i.Value as ZombieMG).Joinable)
                                {
                                    world = i.Value;
                                    fworld = true;
                                    break;
                                }
                        if (!fworld)
                            world = RealmManager.AddWorld(new ZombieMG());

                        psr.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                    }
                    else
                    {
                        SendHelp("Not Enough Fame");
                    }
                }
                else
                {
                    SendInfo("Cancelled entering zombies.");
                }
            }
            if (type == "Nexus Defense")
            {
                if (pkt.Button == 1)
                {
                    SendInfo("Cancelled entering Nexus Defense.");
                }
                else
                {
                    SendInfo("Cancelled entering Nexus Defense.");
                }
            }
            if (type == "SlotMachine1")
            {
                if (pkt.Button == 1)
                {
                    List<Item> weaponsT5 = TierLoot.WeaponItems[5].ToList();
                    List<Item> weaponsT6 = TierLoot.WeaponItems[6].ToList();
                    List<Item> weaponsT7 = TierLoot.WeaponItems[7].ToList();
                    List<Item> abilitiesT3 = TierLoot.AbilityItems[2].ToList();
                    List<Item> ringsT3 = TierLoot.RingItems[3].ToList();
                    List<Item> armorT6 = TierLoot.ArmorItems[6].ToList();
                    List<Item> armorT7 = TierLoot.ArmorItems[7].ToList();
                    List<Item> armorT8 = TierLoot.ArmorItems[8].ToList();

                    int calculator = Random.Next(1, 1000);
                    if (calculator <= 600)
                    {
                        SendHelp("Better luck next time!");
                    }
                    else if (calculator <= 700 && calculator > 600)
                    {
                        SendHelp("Congratulations! You won a T5 Weapon!");

                        weaponsT5.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = weaponsT5[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 750 && calculator > 700)
                    {
                        SendHelp("Congratulations! You won a T6 Weapon!");

                        weaponsT6.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = weaponsT6[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 787.5 && calculator > 775)
                    {
                        SendHelp("Congratulations! You won a T7 Weapon!");

                        weaponsT7.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = weaponsT7[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 800 && calculator > 787.5)
                    {
                        SendHelp("Congratulations! You won a T3 Ability!");

                        abilitiesT3.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = abilitiesT3[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 850 && calculator > 800)
                    {
                        SendHelp("Congratulations! You won a T6 Armor!");

                        armorT6.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = armorT6[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 875 && calculator > 850)
                    {
                        SendHelp("Congratulations! You won a T7 Armor!");

                        armorT7.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = armorT7[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 887.5 && calculator > 875)
                    {
                        SendHelp("Congratulations! You won a T8 Armor!");

                        armorT8.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = armorT8[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 900 && calculator > 887.5)
                    {
                        SendHelp("Congratulations! You won a T3 Ring!");

                        ringsT3.Shuffle();

                        var container = new Container(0x0507, 1000*60, true) {BagOwner = psr.Account.AccountId};
                        container.Inventory[0] = ringsT3[0];
                        container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                            Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
                        container.Size = 75;
                        Owner.EnterWorld(container);
                    }
                    else if (calculator <= 905 && calculator > 900)
                    {
                        SendHelp("Too bad! You only got 1 fame!");
                        Client.Database.UpdateFame(Client.Account, 1);
                        Fame += 1;
                        UpdateCount++;
                    }
                    else if (calculator <= 910 && calculator > 905)
                    {
                        SendHelp("Too bad! You only got 5 fame!");
                        Client.Database.UpdateFame(Client.Account, 5);
                        Fame += 5;
                        UpdateCount++;
                    }
                    else if (calculator <= 940 && calculator > 910)
                    {
                        SendHelp("You won back the fame you paid!");
                        Client.Database.UpdateFame(Client.Account, 10);
                        Fame += 10;
                        UpdateCount++;
                    }
                    else if (calculator <= 970 && calculator > 940)
                    {
                        SendHelp("Nice! You won 25 fame!");
                        Client.Database.UpdateFame(Client.Account, 25);
                        Fame += 25;
                        UpdateCount++;
                    }
                    else if (calculator <= 985 && calculator > 970)
                    {
                        SendHelp("Nice! You won 50 fame!");
                        Client.Database.UpdateFame(Client.Account, 50);
                        Fame += 50;
                        UpdateCount++;
                    }
                    else if (calculator <= 990 && calculator > 985)
                    {
                        SendHelp("Very Nice! You won 100 fame!");
                        Client.Database.UpdateFame(Client.Account, 100);
                        Fame += 100;
                        UpdateCount++;
                    }
                    else if (calculator <= 994 && calculator > 990)
                    {
                        SendHelp("Awesome! You won 500 fame!");
                        Client.Database.UpdateFame(Client.Account, 500);
                        Fame += 500;
                        UpdateCount++;
                    }
                    else if (calculator <= 997 && calculator > 994)
                    {
                        SendHelp("Amazing! You won 1000 fame!");
                        Client.Database.UpdateFame(Client.Account, 1000);
                        Fame += 1000;
                        UpdateCount++;
                    }
                    else if (calculator <= 999 && calculator > 997)
                    {
                        SendHelp("Amazing! You won 5000 fame!");
                        Client.Database.UpdateFame(Client.Account, 5000);
                        Fame += 5000;
                        UpdateCount++;
                    }
                    else if (calculator <= 1000 && calculator > 999)
                    {
                        SendHelp("Incredible! You won the 10000 fame jackpot!");
                        foreach (ClientProcessor i in RealmManager.Clients.Values)
                            i.SendPacket(new TextPacket
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "#Announcement",
                                Text = Name + " has won the 10000 Fame jackpot on the bronze slot machines!"
                            });
                        Client.Database.UpdateFame(Client.Account, 10000);
                        Fame += 10000;
                        UpdateCount++;
                    }
                    psr.SendPacket(new BuyResultPacket
                    {
                        Result = 0
                    });
                }
                else
                {
                    SendInfo("Canceled");
                    psr.SendPacket(new BuyResultPacket
                    {
                        Result = 0
                    });
                }
            }
        }
    }
}