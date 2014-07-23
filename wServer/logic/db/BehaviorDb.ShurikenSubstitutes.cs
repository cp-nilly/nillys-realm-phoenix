#region

using wServer.logic.attack;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ ShurikenSubstitutes = Behav()
            .Init(0x142a, Behaves("ShurikenSubstitute1",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x142b, Behaves("ShurikenSubstitute2",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x142c, Behaves("ShurikenSubstitute3",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x142d, Behaves("ShurikenSubstitute4",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x142e, Behaves("ShurikenSubstitute5",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x142f, Behaves("ShurikenSubstitute6",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1431, Behaves("ShurikenSubstitute7",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
                .Init(0x4a10, Behaves("ShurikenSubstituteChristmas",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(20, 1, 0)),
                        Despawn.Instance
                        ))
                ))
			.Init(0x1432, Behaves("ShurikenSubstituteO37",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x1433, Behaves("ShurikenSubstituteO38",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        )
                    )
                ))
            ;
    }
}