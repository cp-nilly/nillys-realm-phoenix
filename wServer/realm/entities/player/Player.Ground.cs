#region

using System.Linq;
using db.data;
using wServer.cliPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private bool OxygenRegen;
        private long b;
        private long l;

        private void HandleGround(RealmTime time)
        {
            if (time.tickTimes - l > 500)
            {
                if (HasConditionEffect(ConditionEffects.Paused) ||
                    HasConditionEffect(ConditionEffects.Invincible))
                    return;

                try
                {
                    var tile = Owner.Map[(int) X, (int) Y];
                    var objDesc = tile.ObjType == 0 ? null : XmlDatas.ObjectDescs[tile.ObjType];
                    var tileDesc = XmlDatas.TileDescs[tile.TileId];
                    if (tileDesc.Damaging && (objDesc == null || !objDesc.ProtectFromGroundDamage))
                    {
                        var dmg = Random.Next(tileDesc.MinDamage, tileDesc.MaxDamage);
                        dmg = (int) statsMgr.GetDefenseDamage(dmg, true);

                        HP -= dmg;
                        UpdateCount++;
                        if (HP <= 0)
                            Death("lava");

                        l = time.tickTimes;
                    }
                }
                catch
                {
                }
            }
            if (time.tickTimes - b > 60)
            {
                try
                {
                    if (Owner.Name == "Ocean Trench")
                    {
                        var fObject = false;
                        foreach (var i in Owner.StaticObjects.Where(i => i.Value.ObjectType == 0x0731).Where(i => (X - i.Value.X)*(X - i.Value.X) + (Y - i.Value.Y)*(Y - i.Value.Y) < 2))
                            fObject = true;

                        
                        OxygenRegen = fObject;

                        if (!OxygenRegen)
                        {
                            if (OxygenBar == 0)
                                HP -= 5;
                            else
                                OxygenBar -= 1;

                            UpdateCount++;

                            if (HP <= 0)
                                Death("lack of oxygen");
                        }
                        else
                        {
                            if (OxygenBar < 100)
                                OxygenBar += 8;
                            if (OxygenBar > 100)
                                OxygenBar = 100;

                            UpdateCount++;
                        }
                    }

                    b = time.tickTimes;
                }
                catch
                {
                }
            }
        }

        public void GroundDamage(RealmTime t, GroundDamagePacket pkt)
        {
        }
    }
}