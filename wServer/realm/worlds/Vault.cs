#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using common;
using common.data;
using wServer.realm.entities;

#endregion

namespace wServer.realm.worlds
{
    public class Vault : World
    {
        private readonly ConcurrentDictionary<Tuple<Container, VaultChest>, int> _vaultChests =
            new ConcurrentDictionary<Tuple<Container, VaultChest>, int>();

        private Account acc;
        public ClientProcessor psr;

        public Vault(bool isLimbo, ClientProcessor psr = null)
        {
            Id = VAULT_ID;
            Name = "Vault";
            Background = 2;
            SetMusic("Vault");
            if (!(IsLimbo = isLimbo))
            {
                base.FromWorldMap(
                    typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.vault.wmap"));
                Init(psr);
            }
        }

        private void Init(ClientProcessor psr)
        {
            this.psr = psr;
            acc = psr.Account;
            var vaultChestPosition = new List<IntPoint>();
            var spawn = new IntPoint(0, 0);

            int w = Map.Width;
            int h = Map.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    WmapTile tile = Map[x, y];
                    if (tile.Region == TileRegion.Spawn)
                        spawn = new IntPoint(x, y);
                    else if (tile.Region == TileRegion.Vault)
                        vaultChestPosition.Add(new IntPoint(x, y));
                }
            vaultChestPosition.Sort((x, y) => Comparer<int>.Default.Compare(
                (x.X - spawn.X)*(x.X - spawn.X) + (x.Y - spawn.Y)*(x.Y - spawn.Y),
                (y.X - spawn.X)*(y.X - spawn.X) + (y.Y - spawn.Y)*(y.Y - spawn.Y)));

            List<VaultChest> chests = psr.Account.Vault.Chests;
            foreach (VaultChest t in chests)
            {
                var con = new Container(0x0504, null, false);
                Item[] inv =
                    t.Items.Select(
                        _ =>
                            _ == -1
                                ? null
                                : (XmlDatas.ItemDescs.ContainsKey((short) _) ? XmlDatas.ItemDescs[(short) _] : null))
                        .ToArray();
                for (int j = 0; j < 8; j++)
                    con.Inventory[j] = inv[j];
                con.Move(vaultChestPosition[0].X + 0.5f, vaultChestPosition[0].Y + 0.5f);
                EnterWorld(con);
                vaultChestPosition.RemoveAt(0);

                _vaultChests[new Tuple<Container, VaultChest>(con, t)] = con.UpdateCount;
            }
            foreach (IntPoint i in vaultChestPosition)
            {
                var x = new SellableObject(0x0505);
                x.Move(i.X + 0.5f, i.Y + 0.5f);
                EnterWorld(x);
            }
        }

        public void AddChest(VaultChest chest, Entity original)
        {
            var con = new Container(0x0504, null, false);
            Item[] inv =
                chest.Items.Select(
                    _ =>
                        _ == -1
                            ? null
                            : (XmlDatas.ItemDescs.ContainsKey((short) _) ? XmlDatas.ItemDescs[(short) _] : null))
                    .ToArray();
            for (int j = 0; j < 8; j++)
                con.Inventory[j] = inv[j];
            con.Move(original.X, original.Y);
            LeaveWorld(original);
            EnterWorld(con);

            _vaultChests[new Tuple<Container, VaultChest>(con, chest)] = con.UpdateCount;
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Vault(false, psr));
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (var i in _vaultChests)
            {
                if (i.Key.Item1.UpdateCount > i.Value)
                {
                    try
                    {
                        i.Key.Item2._Items =
                            Utils.GetCommaSepString(
                                i.Key.Item1.Inventory.Take(8).Select(_ => _ == null ? -1 : _.ObjectType).ToArray());
                        using (var db = new Database())
                            db.SaveChest(acc, i.Key.Item2);
                        _vaultChests[i.Key] = i.Key.Item1.UpdateCount;
                    }
                    catch
                    {
                        //Do NOT make anything happen here..
                        //This catch continues on endlessly
                    }
                }
            }
        }
    }
}