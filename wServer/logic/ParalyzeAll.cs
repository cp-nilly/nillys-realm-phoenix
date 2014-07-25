#region

using System;
using wServer.realm;

#endregion

namespace wServer.logic
{
    internal class ParalyzeAll : Behavior
    {
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