#region

using wServer.logic.attack;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ PearlSubstitutes = Behav()
            .Init(0x1421, Behaves("PearlSubstitute1",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1422, Behaves("PearlSubstitute2",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1423, Behaves("PearlSubstitute3",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 1, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1424, Behaves("PearlSubstitute4",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 2, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1425, Behaves("PearlSubstitute5",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 3, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1426, Behaves("PearlSubstitute6",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 3, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1427, Behaves("PearlSubstitute7",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 3, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1428, Behaves("PearlSubstitute8",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 3, 0)),
                        Despawn.Instance
                        ))
                ))
            .Init(0x1429, Behaves("PearlSubstitute9",
                new RunBehaviors(
                    new QueuedBehavior(
                        Once.Instance(PetAttackTarget.Instance(10, 3, 0)),
                        Despawn.Instance
                        ))
                ))
            ;
    }
}