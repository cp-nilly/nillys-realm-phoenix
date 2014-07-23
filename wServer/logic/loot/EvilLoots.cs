#region

using System;
using db.data;

#endregion

namespace wServer.logic.loot
{
    internal class EvilLoot : ILoot //Blood of evil hen! :D
    {
        public Item GetLoot(Random rand)
        {
            return XmlDatas.ItemDescs[0x0a22];
        }
    }
}