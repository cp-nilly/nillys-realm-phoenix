#region

using System;
using System.Collections.Generic;
using wServer.realm;

#endregion

namespace wServer.logic
{
    internal class State : Behavior
    {
        private static readonly Dictionary<string, State> instances = new Dictionary<string, State>();
        private readonly Behavior[] behave;
        private readonly string name;

        private Random rand = new Random();

        public State(string name, params Behavior[] behave)
        {
            this.name = name;
            this.behave = behave;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.State == name)
            {
                foreach (var i in behave)
                {
                    i.Tick(Host, time);
                }
                return true;
            }
            return false;
        }
    }

    internal class NotState : Behavior
    {
        private static readonly Dictionary<string, NotState> instances = new Dictionary<string, NotState>();
        private readonly Behavior[] behave;
        private readonly string name;

        private Random rand = new Random();

        public NotState(string name, params Behavior[] behave)
        {
            this.name = name;
            this.behave = behave;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.State != name)
            {
                foreach (var i in behave)
                {
                    i.Tick(Host, time);
                }
                return true;
            }
            return false;
        }
    }

    internal class SubState : Behavior
    {
        private static readonly Dictionary<string, SubState> instances = new Dictionary<string, SubState>();
        private readonly Behavior[] behave;
        private readonly string name;

        private Random rand = new Random();

        public SubState(string name, params Behavior[] behave)
        {
            this.name = name;
            this.behave = behave;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.SubState == name)
            {
                foreach (var i in behave)
                {
                    i.Tick(Host, time);
                }
                return true;
            }
            return false;
        }
    }

    internal class SetState : Behavior
    {
        private static readonly Dictionary<string, SetState> instances = new Dictionary<string, SetState>();
        private readonly string state;
        private Random rand = new Random();

        private SetState(string state)
        {
            this.state = state;
        }

        public static SetState Instance(string state)
        {
            SetState ret;
            if (!instances.TryGetValue(state, out ret))
                ret = instances[state] = new SetState(state);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var states = state.Split('.');
            if (states[0] != null || states[0] != "")
            {
                Host.Self.State = states[0];
            }
            else
            {
                if (states.Length == 1)
                {
                    Host.Self.State = "idle";
                }
            }
            if (states.Length > 1)
            {
                if (states[1] != null || states[1] != "")
                {
                    Host.Self.SubState = states[1];
                }
                else
                {
                    Host.Self.SubState = "1";
                }
            }
            Host.StateTable.Clear();
            return true;
        }
    }

    internal class StateOnce : Behavior
    {
        private static readonly Dictionary<Behavior[], StateOnce> instances = new Dictionary<Behavior[], StateOnce>();
        private readonly Behavior[] x;

        private StateOnce(params Behavior[] x)
        {
            this.x = x;
        }

        public static StateOnce Instance(params Behavior[] x)
        {
            StateOnce ret;
            if (!instances.TryGetValue(x, out ret))
                ret = instances[x] = new StateOnce(x);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (!Host.StateTable.ContainsKey(Key))
            {
                foreach (var i in x)
                {
                    i.Tick(Host, time);
                }
                Host.StateTable.Add(Key, true);
                return true;
            }
            return false;
        }
    }

    internal class ConditionalState : ConditionalBehavior
    {
        private readonly Behavior[] behave;
        private readonly BehaviorCondition cond;
        private readonly string name;
        private Random rand = new Random();

        public ConditionalState(BehaviorCondition cond, string name, params Behavior[] behave)
        {
            this.cond = cond;
            this.name = name;
            this.behave = behave;
        }

        public override BehaviorCondition Condition
        {
            get { return cond; }
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            if (Host.Self.State == name)
            {
                foreach (var i in behave)
                {
                    i.Tick(Host, (RealmTime) time);
                }
            }
        }
    }

    internal class Uniqueify : Behavior
    {
        private Random rand = new Random();
        private string unname;

        public Uniqueify(string unname)
        {
            this.unname = unname;
        }

        //static readonly Dictionary<string, SubState> instances = new Dictionary<string, SubState>();

        protected override bool TickCore(RealmTime time)
        {
            return true;
        }
    }
}