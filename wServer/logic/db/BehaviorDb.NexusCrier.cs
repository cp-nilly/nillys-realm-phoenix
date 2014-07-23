#region

using wServer.logic.taunt;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ NexusCrier = Behav()
            .Init(0x0e78, Behaves("Nexus Crier",
                Cooldown.Instance(30000,
                    new SimpleTaunt(
                        "Welcome everybody! I hope you enjoy this server, please note this is a legit server and we'll not spawn or give items."))
                ));
    }
}