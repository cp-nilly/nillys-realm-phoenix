#region

using System;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.logic
{
    internal class ChatEvent : ConditionalBehavior
    {
        private readonly Behavior[] behaves;
        private bool adminOnly;
        private String[] chat;
        private Behavior[] falseBehaves;

        public ChatEvent(params Behavior[] behaves)
        {
            this.behaves = behaves;
        }

        public override BehaviorCondition Condition
        {
            get { return BehaviorCondition.OnChat; }
        }

        public ChatEvent SetChats(params String[] chat)
        {
            this.chat = chat;
            return this;
        }

        public ChatEvent FalseEvent(params Behavior[] falseBehaves)
        {
            this.falseBehaves = falseBehaves;
            return this;
        }

        public ChatEvent AdminOnly()
        {
            adminOnly = true;
            return this;
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state, string msg,
            Player player)
        {
            if (!adminOnly || player.Client.Account.Rank >= 2)
            {
                foreach (var s in chat)
                {
                    if (msg.ToLower() == s.ToLower())
                    {
                        foreach (var i in behaves)
                        {
                            i.Tick(Host, (RealmTime) time);
                        }
                        return;
                    }
                }
                if (falseBehaves != null)
                {
                    foreach (var f in falseBehaves)
                    {
                        f.Tick(Host, (RealmTime) time);
                    }
                }
            }
        }
    }
}