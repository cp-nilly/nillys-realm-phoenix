#region

using wServer.cliPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        public void Buy(RealmTime time, BuyPacket pkt)
        {
            var obj = Owner.GetEntity(pkt.ObjectId) as SellableObject;
            if (obj != null)
                obj.Buy(this);
        }

        public void CheckCredits(RealmTime t, CheckCreditsPacket pkt)
        {
            psr.Database.ReadStats(psr.Account);
            Credits = psr.Account.Credits;
            UpdateCount++;
        }
    }
}