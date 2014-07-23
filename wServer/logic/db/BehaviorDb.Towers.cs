#region

using System;
using wServer.logic.attack;
using wServer.logic.taunt;


#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Towers = Behav()
            .Init(0x140b, Behaves("Tower1",
                new RunBehaviors(
                    Cooldown.Instance(600, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x140c, Behaves("Tower2",
                new RunBehaviors(
                    Cooldown.Instance(550, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x140d, Behaves("Tower3",
                new RunBehaviors(
                    Cooldown.Instance(500, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x140e, Behaves("Tower4",
                new RunBehaviors(
                    Cooldown.Instance(450, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x140f, Behaves("Tower5",
                new RunBehaviors(
                    Cooldown.Instance(400, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x141a, Behaves("Tower6",
                new RunBehaviors(
                    Cooldown.Instance(350, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x141b, Behaves("Tower7",
                new RunBehaviors(
                    Cooldown.Instance(300, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x141c, Behaves("Tower8",
                new RunBehaviors(
                    Cooldown.Instance(250, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x141d, Behaves("Tower9",
                new RunBehaviors(
                    Cooldown.Instance(200, PetSimpleAttack.Instance(10, 0))
                    )
                ))
            .Init(0x5035, Behaves("War Turret",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1,0)),

                    IfEqual.Instance(-1,0,
                      new RunBehaviors(
                        Cooldown.Instance(100, PetSimpleAttack.Instance(15, 0)),
                        new QueuedBehavior(
                          new SimpleTaunt("Charging..."),
                          CooldownExact.Instance(8000),
                          new SetKey(-1,1)
                          )
                      )),
                    IfEqual.Instance(-1,1,
                      new RunBehaviors(
                        Flashing.Instance(100, 0xffffffff),
                        new QueuedBehavior(
                          new SimpleTaunt("Fire!"),
                          PetMultiAttack.Instance(15, 20 * (float)Math.PI / 360, 5, 40 * (float)Math.PI / 180, 1),
                          CooldownExact.Instance(400),
                          PetMultiAttack.Instance(15, 20 * (float)Math.PI / 360, 5, 20 * (float)Math.PI / 180, 1),
                          CooldownExact.Instance(400),
                          PetMultiAttack.Instance(15, 20 * (float)Math.PI / 360, 5, 0, 1),
                          CooldownExact.Instance(400),
                          PetMultiAttack.Instance(15, 20 * (float)Math.PI / 360, 5, -20f * (float)Math.PI / 180, 1),
                          CooldownExact.Instance(400),
                          PetMultiAttack.Instance(15, 20 * (float)Math.PI / 360, 5, -40 * (float)Math.PI / 180, 1),
                          CooldownExact.Instance(400),
                          new SetKey(-1,0)
                          )
                        ))
                    )
                ))
            ;
    }
}