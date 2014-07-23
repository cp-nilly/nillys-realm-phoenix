#region

using wServer.logic.movement;
using wServer.logic.taunt;
using wServer.logic.travoos;
using wServer.realm;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Herding = Behav()
            .Init(0x701a, Behaves("Herding Sheep",
                new QueuedBehavior(
                    RandomDelay.Instance(2500, 7500),
                    SimpleWandering.Instance(2, 2)
                    ),
                new RunBehaviors(
                    MaintainDist.Instance(5, 6, 6, null),
                    Cooldown.Instance(1000,
                        Rand.Instance(
                            new RandomTaunt(0.001, "baa"),
                            new RandomTaunt(0.001, "baa baa")
                            )
                        ),
                    CooldownExact.Instance(200,
                        If.Instance(CheckRegion.Instance(TileRegion.Enemy),
                            new RunBehaviors(
                                WorldEvent.Instance("sheep"),
                                Despawn.Instance
                                )
                            )
                        )
                    )
                ))
            .Init(0x701b, Behaves("Giant Herding Sheep",
                new QueuedBehavior(
                    RandomDelay.Instance(2500, 7500),
                    SimpleWandering.Instance(2, 1)
                    ),
                new RunBehaviors(
                    MaintainDist.Instance(2, 3, 3, null),
                    Cooldown.Instance(1000,
                        Rand.Instance(
                            new RandomTaunt(0.001, "baa"),
                            new RandomTaunt(0.001, "baa baa")
                            )
                        ),
                    CooldownExact.Instance(200,
                        If.Instance(CheckRegion.Instance(TileRegion.Enemy),
                            new RunBehaviors(
                                WorldEvent.Instance("sheep-g"),
                                Despawn.Instance
                                )
                            )
                        )
                    )
                ))
            .Init(0x701f, Behaves("Black Herding Sheep",
                new QueuedBehavior(
                    RandomDelay.Instance(2500, 7500),
                    SimpleWandering.Instance(3, 2)
                    ),
                new RunBehaviors(
                    MaintainDist.Instance(7, 7, 7, null),
                    Cooldown.Instance(1000,
                        Rand.Instance(
                            new RandomTaunt(0.001, "baa"),
                            new RandomTaunt(0.001, "baa baa")
                            )
                        ),
                    CooldownExact.Instance(200,
                        If.Instance(CheckRegion.Instance(TileRegion.Enemy),
                            new RunBehaviors(
                                WorldEvent.Instance("blackSheep"),
                                Despawn.Instance
                                )
                            )
                        )
                    )
                ));
    }
}