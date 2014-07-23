#region

using wServer.logic.attack;
using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Drakes = Behav()
            .Init(0x1400, Behaves("Purple Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(300, new PurpleDrakeAttack(6))
                    )
                ))
            .Init(0x1403, Behaves("Orange Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(2000, new OrangeDrakeAttack(6))
                    )
                ))
            .Init(0x1404, Behaves("Green Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(300, new GreenDrakeAttack(6))
                    )
                ))
            .Init(0x1405, Behaves("Yellow Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(1000, new YellowDrakeAttack(6))
                    )
                ))
            .Init(0x1402, Behaves("Blue Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(5000, new BlueDrakeAttack(6))
                    )
                ))
            .Init(0x1401, Behaves("White Drake",
                new RunBehaviors
                    (
                    If.Instance(new PetBehaves(), PetChasing.Instance(6, 10, 3)),
                    Cooldown.Instance(500, new WhiteDrakeHeal())
                    )
                ))
            ;
    }
}