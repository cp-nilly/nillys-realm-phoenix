#region

using System;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.taunt
{
    internal abstract class TauntBase : Behavior
    {
        protected void Taunt(string taunt, bool all)
        {
            if (taunt.Contains("{PLAYER}"))
            {
                float dist = 10;
                Entity player = GetNearestEntity(ref dist, null);
                if (player == null) return;
                taunt = taunt.Replace("{PLAYER}", player.nName);
            }
            taunt = taunt.Replace("{HP}", (Host as Enemy).HP.ToString());
            try
            {
                Host.Self.Owner.BroadcastPacket(new TextPacket
                {
                    Name = "#" + (Host.Self.ObjectDesc.DisplayId ?? Host.Self.ObjectDesc.ObjectId),
                    ObjectId = Host.Self.Id,
                    Stars = -1,
                    BubbleTime = 5,
                    Recipient = "",
                    Text = taunt,
                    CleanText = ""
                }, null);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("Crash halted - Nobody likes death...");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        protected void NoBubbleTaunt(string taunt, bool all)
        {
            if (taunt.Contains("{PLAYER}"))
            {
                float dist = 10;
                Entity player = GetNearestEntity(ref dist, null);
                if (player == null) return;
                taunt = taunt.Replace("{PLAYER}", player.nName);
            }
            taunt = taunt.Replace("{HP}", (Host as Enemy).HP.ToString());
            try
            {
                Host.Self.Owner.BroadcastPacket(new TextPacket
                {
                    Name = "#" + (Host.Self.ObjectDesc.DisplayId ?? Host.Self.ObjectDesc.ObjectId),
                    ObjectId = Host.Self.Id,
                    Stars = -1,
                    BubbleTime = 0,
                    Recipient = "",
                    Text = taunt,
                    CleanText = ""
                }, null);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("Crash halted - Nobody likes death...");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        protected abstract override bool TickCore(RealmTime time);
    }

    internal class SimpleTaunt : TauntBase
    {
        private readonly string taunt;

        public SimpleTaunt(string taunt)
        {
            this.taunt = taunt;
        }

        protected override bool TickCore(RealmTime time)
        {
            Taunt(taunt, false);
            return true;
        }
    }

    internal class QuietTaunt : TauntBase
    {
        private readonly string taunt;

        public QuietTaunt(string taunt)
        {
            this.taunt = taunt;
        }

        protected override bool TickCore(RealmTime time)
        {
            NoBubbleTaunt(taunt, false);
            return true;
        }
    }
}