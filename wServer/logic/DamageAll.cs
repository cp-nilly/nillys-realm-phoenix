#region

using System;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class DamageAll : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            foreach (var i in Host.Self.Owner.Players)
            {
                i.Value.Damage(200, Host.Self as Character);
            }

            return true;
        }
    }
}