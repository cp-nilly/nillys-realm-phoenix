#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.setpieces;

#endregion

namespace wServer.logic
{
    internal class MonsterSetPiece : Behavior
    {
        private static readonly Dictionary<Tuple<string, int>, MonsterSetPiece> instances =
            new Dictionary<Tuple<string, int>, MonsterSetPiece>();

        private readonly string SetPiece;
        private readonly int offsetFix;

        private MonsterSetPiece(string SetPiece, int offsetFix)
        {
            this.SetPiece = SetPiece;
            this.offsetFix = offsetFix;
        }


        public static MonsterSetPiece Instance(string SetPiece, int offsetFix)
        {
            var key = new Tuple<string, int>(SetPiece, offsetFix);
            MonsterSetPiece ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new MonsterSetPiece(SetPiece, offsetFix);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var chr = Host as Character;

            var target = new Position
            {
                X = Host.Self.X,
                Y = Host.Self.Y
            };
            if (Host.Self.Owner.Name != "Battle Arena" && Host.Self.Owner.Name != "Free Battle Arena")
            {
                var piece = (ISetPiece) Activator.CreateInstance(Type.GetType(
                    "wServer.realm.setpieces." + SetPiece));
                piece.RenderSetPiece(Host.Self.Owner,
                    new IntPoint((int) Host.Self.X - offsetFix, (int) Host.Self.Y - offsetFix));
                return true;
            }
            return false;
        }
    }
}