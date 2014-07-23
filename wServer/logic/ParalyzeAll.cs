#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic
{
    internal class ParalyzeAll : Behavior
    {

        private readonly float angle;
        private readonly short objType;
        private readonly float range;
        private Random rand = new Random();


        protected override bool TickCore(RealmTime time)
        {
            foreach (var i in Host.Self.Owner.Players)
            {
                i.Value.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paralyzed,
                    DurationMS = -1
                });
            }

            return true;
        }
    }
}