#region

using System.Collections.Generic;
using db;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private const int LOCKED_LIST_ID = 0;
        private const int IGNORED_LIST_ID = 1;

        public void SendAccountList(List<int> list, int id)
        {
            psr.SendPacket(new AccountListPacket
            {
                AccountListId = id,
                AccountIds = list.ToArray()
            });
        }

        public void EditAccountList(RealmTime time, EditAccountListPacket pkt)
        {
            List<int> list;
            if (pkt.AccountListId == LOCKED_LIST_ID)
                list = Locked;
            else if (pkt.AccountListId == IGNORED_LIST_ID)
                list = Ignored;
            else return;
            if (list == null)
                list = new List<int>();
            var player = Owner.GetEntity(pkt.ObjectId) as Player;
            if (player == null) return;
            int accId = player.psr.Account.AccountId;
            var dbx = new Database();
            //if (pkt.Add && list.Count < 6)
            //    list.Add(accId);
            //else
            //    list.Remove(accId);

            if (pkt.Add)
            {
                list.Add(accId);
                if (pkt.AccountListId == LOCKED_LIST_ID)
                    dbx.AddLock(psr.Account.AccountId, accId);
                if (pkt.AccountListId == IGNORED_LIST_ID)
                    dbx.AddIgnore(psr.Account.AccountId, accId);
            }
            else
            {
                list.Remove(accId);
                if (pkt.AccountListId == LOCKED_LIST_ID)
                    dbx.RemoveLock(psr.Account.AccountId, accId);
                if (pkt.AccountListId == IGNORED_LIST_ID)
                    dbx.RemoveIgnore(psr.Account.AccountId, accId);
            }

            SendAccountList(list, pkt.AccountListId);
        }

        public bool IsUserInLegends()
        {
            //Week
            using (Database db = new Database())
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM death WHERE (time >= DATE_SUB(NOW(), INTERVAL 1 WEEK)) ORDER BY totalFame DESC LIMIT 10;";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        if (rdr.GetInt32("accId") == AccountId) return true;
            }

            //Month
            using (Database db = new Database())
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM death WHERE (time >= DATE_SUB(NOW(), INTERVAL 1 MONTH)) ORDER BY totalFame DESC LIMIT 10;";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        if (rdr.GetInt32("accId") == AccountId) return true;
            }
            //All Time
            using (Database db = new Database())
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT * FROM death WHERE TRUE ORDER BY totalFame DESC LIMIT 10;";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        if (rdr.GetInt32("accId") == AccountId) return true;
            }

            return false;
        }
    }
}