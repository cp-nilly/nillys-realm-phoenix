#region

using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Unknown = Behav()
            .Init(0x0f02, Behaves("Unknown Giant Golem",
                new RunBehaviors(
                    SimpleWandering.Instance(2, 5)
                    )
                ));
    }
}