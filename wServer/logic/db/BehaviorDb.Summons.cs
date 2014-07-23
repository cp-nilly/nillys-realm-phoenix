#region

using System;
using wServer.logic.attack;
using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Summons = Behav()
            .Init(0x0f2f, Behaves("Bunny",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1000, PetSimpleAttack.Instance(10, 0))
                ))
            .Init(0x0f32, Behaves("Snake Summon",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1000, PetSimpleAttack.Instance(10, 0))
                ))
            .Init(0x0f34, Behaves("Wasp Summon",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1000, PetSimpleAttack.Instance(10, 0))
                ))
            .Init(0x0f36, Behaves("Wolf Summon",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1000, PetSimpleAttack.Instance(10, 0))
                ))
            .Init(0x0f38, Behaves("Drake Summon",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1200, PetMultiAttack.Instance(10, 10*(float) Math.PI/180, 3, 0, 0))
                ))
            .Init(0x0f3a, Behaves("Wisp Summon",
                new RunBehaviors(
                    IfNot.Instance(
                        PetChasingEnemy.Instance(8, 10, 0),
                        If.Instance(new PetBehaves(),
                            PetChasing.Instance(8, 20, 0))),
                    Cooldown.Instance(1000, PetMultiAttack.Instance(10, 0, 1, 10*(float) Math.PI/180, 0)),
                    Cooldown.Instance(1000, PetMultiAttack.Instance(10, 0, 1, 0, 1)),
                    Cooldown.Instance(1000, PetMultiAttack.Instance(10, 0, 1, -10*(float) Math.PI/180, 2))
                    )))
            .Init(0x0f3c, Behaves("Dragon Summon",
                new RunBehaviors(
                    IfNot.Instance(
                        PetChasingEnemy.Instance(8, 10, 0),
                        If.Instance(new PetBehaves(),
                            PetChasing.Instance(8, 20, 0))),
                    Cooldown.Instance(1000, PetMultiAttack.Instance(15, 5*(float) Math.PI/180, 5, 0, 0))
                    )))
            .Init(0x7111, Behaves("Brute Summon",
                new RunBehaviors(
                    IfNot.Instance(
                        PetChasingEnemy.Instance(8, 10, 0),
                        If.Instance(new PetBehaves(),
                            PetChasing.Instance(8, 20, 0))),
                    Cooldown.Instance(500, PetMultiAttack.Instance(15, 2.5f * (float)Math.PI / 180, 3, 0, 0))
                    )))
            .Init(0x7114, Behaves("Knight Summon",
                new RunBehaviors(
                    IfNot.Instance(
                        PetChasingEnemy.Instance(8, 10, 0),
                        If.Instance(new PetBehaves(),
                            PetChasing.Instance(8, 20, 0))),
                    Cooldown.Instance(300, PetMultiAttack.Instance(15, 0, 1, 0, 0))
                    )))

            .Init(0x0f3e, Behaves("Sheep Summon",
                IfNot.Instance(
                    PetChasingEnemy.Instance(8, 10, 0),
                    If.Instance(new PetBehaves(),
                        PetChasing.Instance(8, 20, 0))),
                Cooldown.Instance(1000, PetSimpleAttack.Instance(10, 0))
                ))
            ;
    }
}