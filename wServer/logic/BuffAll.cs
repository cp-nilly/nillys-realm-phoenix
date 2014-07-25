#region

using System;
using wServer.realm;

#endregion

namespace wServer.logic
{
    internal class BuffAll : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            foreach (var i in Host.Self.Owner.Players)
            {
                i.Value.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Armored,
                    DurationMS = -1
                });
                i.Value.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Healing,
                    DurationMS = -1
                });
                i.Value.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Berserk,
                    DurationMS = -1
                });
                i.Value.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Damaging,
                    DurationMS = -1
                });
            }

            return true;
        }
    }
}