#region

using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic
{
    internal class SetConditionEffectTimed : Behavior
    {
        private readonly ConditionEffectIndex eff;
        private readonly int time;

        public SetConditionEffectTimed(ConditionEffectIndex eff, int time)
        {
            this.eff = eff;
            this.time = time;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.ApplyConditionEffect(new ConditionEffect
            {
                Effect = eff,
                DurationMS = this.time
            });
            return true;
        }
    }

    internal class SetConditionEffect : Behavior
    {
        private static readonly Dictionary<ConditionEffectIndex, SetConditionEffect> instances =
            new Dictionary<ConditionEffectIndex, SetConditionEffect>();

        private readonly ConditionEffectIndex eff;

        private SetConditionEffect(ConditionEffectIndex eff)
        {
            this.eff = eff;
        }

        public static SetConditionEffect Instance(ConditionEffectIndex eff)
        {
            SetConditionEffect ret;
            if (!instances.TryGetValue(eff, out ret))
                ret = instances[eff] = new SetConditionEffect(eff);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.ApplyConditionEffect(new ConditionEffect
            {
                Effect = eff,
                DurationMS = -1
            });
            return true;
        }
    }

    internal class UnsetConditionEffect : Behavior
    {
        private static readonly Dictionary<ConditionEffectIndex, UnsetConditionEffect> instances =
            new Dictionary<ConditionEffectIndex, UnsetConditionEffect>();

        private readonly ConditionEffectIndex eff;

        private UnsetConditionEffect(ConditionEffectIndex eff)
        {
            this.eff = eff;
        }

        public static UnsetConditionEffect Instance(ConditionEffectIndex eff)
        {
            UnsetConditionEffect ret;
            if (!instances.TryGetValue(eff, out ret))
                ret = instances[eff] = new UnsetConditionEffect(eff);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.ApplyConditionEffect(new ConditionEffect
            {
                Effect = eff,
                DurationMS = 0
            });
            return true;
        }
    }
}