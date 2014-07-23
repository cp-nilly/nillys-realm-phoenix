using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

namespace wServer.logic.attack
{
  class InfiniteSpiralAttack : Behavior
  {
    int cooldown;
    int arms;
    float offsetIncrement;
    int projectileIndex;
    int incrementMultiplier = 0;
    private InfiniteSpiralAttack(int cooldown, int arms, float offsetIncrement, int projectileIndex)
    {
      this.cooldown = cooldown;
      this.arms = arms;
      this.offsetIncrement = offsetIncrement;
      this.projectileIndex = projectileIndex;
    }
    static readonly Dictionary<Tuple<int, int, float, int>, InfiniteSpiralAttack> instances = new Dictionary<Tuple<int, int, float, int>, InfiniteSpiralAttack>();
    public static InfiniteSpiralAttack Instance(int cooldown, int arms, float offsetIncrement = 1, int projectileIndex = 0)
    {
      var key = new Tuple<int, int, float, int>(cooldown, arms, offsetIncrement, projectileIndex);
      InfiniteSpiralAttack ret;
      if (!instances.TryGetValue(key, out ret))
        ret = instances[key] = new InfiniteSpiralAttack(cooldown, arms, offsetIncrement, projectileIndex);
      return ret;
    }

    Random rand = new Random();
    protected override bool TickCore(RealmTime time)
    {

      Behavior behav = RingAttack.Instance(arms, 0, (offsetIncrement * (float)Math.PI / 180) * incrementMultiplier, projectileIndex);

      int remainingTick;
      object o;
      if (!Host.StateStorage.TryGetValue(Key, out o))
        remainingTick = rand.Next(0, cooldown);
      else
        remainingTick = (int)o;

      remainingTick -= time.thisTickTimes;
      bool ret;
      if (remainingTick <= 0)
      {
        if (behav != null)
          behav.Tick(Host, time);
        if (behav != null)
          incrementMultiplier += 1;
        remainingTick = rand.Next((int)(cooldown * 0.95), (int)(cooldown * 1.05));
        ret = true;
      }
      else
        ret = false;
      Host.StateStorage[Key] = remainingTick;
      return ret;
    }
  }

  class InfiniteSpiralAttack2 : Behavior
  {
    int cooldown;
    int arms;
    float offsetIncrement;
    float offsetBase;
    int projectileIndex;
    int incrementMultiplier = 0;
    private InfiniteSpiralAttack2(int cooldown, int arms, float offsetIncrement, float offsetBase, int projectileIndex)
    {
      this.cooldown = cooldown;
      this.arms = arms;
      this.offsetIncrement = offsetIncrement;
      this.offsetBase = offsetBase;
      this.projectileIndex = projectileIndex;
    }
    static readonly Dictionary<Tuple<int, int, float, float, int>, InfiniteSpiralAttack2> instances = new Dictionary<Tuple<int, int, float, float, int>, InfiniteSpiralAttack2>();
    public static InfiniteSpiralAttack2 Instance(int cooldown, int arms, float offsetIncrement = 1, float offsetBase = 0, int projectileIndex = 0)
    {
      var key = new Tuple<int, int, float, float, int>(cooldown, arms, offsetIncrement, offsetBase, projectileIndex);
      InfiniteSpiralAttack2 ret;
      if (!instances.TryGetValue(key, out ret))
        ret = instances[key] = new InfiniteSpiralAttack2(cooldown, arms, offsetIncrement, offsetBase, projectileIndex);
      return ret;
    }

    Random rand = new Random();
    protected override bool TickCore(RealmTime time)
    {

      Behavior behav = RingAttack.Instance(arms, 0, ((offsetIncrement * (float)Math.PI / 180) * incrementMultiplier) + offsetBase, projectileIndex);

      int remainingTick;
      object o;
      if (!Host.StateStorage.TryGetValue(Key, out o))
        remainingTick = rand.Next(0, cooldown);
      else
        remainingTick = (int)o;

      remainingTick -= time.thisTickTimes;
      bool ret;
      if (remainingTick <= 0)
      {
        if (behav != null)
          behav.Tick(Host, time);
        if (behav != null)
          incrementMultiplier += 1;
        remainingTick = rand.Next((int)(cooldown * 0.95), (int)(cooldown * 1.05));
        ret = true;
      }
      else
        ret = false;
      Host.StateStorage[Key] = remainingTick;
      return ret;
    }
  }

  class TimedInfSpiralAttack : Behavior
  {
    int initCooldown;
    int cooldown;
    int arms;
    float offsetIncrement;
    float offsetBase;
    int projectileIndex;
    int incrementMultiplier = 0;
    private TimedInfSpiralAttack(int initCooldown, int cooldown, int arms, float offsetIncrement, float offsetBase, int projectileIndex)
    {
      this.initCooldown = initCooldown;
      this.cooldown = cooldown;
      this.arms = arms;
      this.offsetIncrement = offsetIncrement;
      this.offsetBase = offsetBase;
      this.projectileIndex = projectileIndex;
    }
    static readonly Dictionary<Tuple<int, int, int, float, float, int>, TimedInfSpiralAttack> instances = new Dictionary<Tuple<int, int, int, float, float, int>, TimedInfSpiralAttack>();
    public static TimedInfSpiralAttack Instance(int initCooldown, int cooldown, int arms, float offsetIncrement = 1, float offsetBase = 0, int projectileIndex = 0)
    {
      var key = new Tuple<int, int, int, float, float, int>(initCooldown, cooldown, arms, offsetIncrement, offsetBase, projectileIndex);
      TimedInfSpiralAttack ret;
      if (!instances.TryGetValue(key, out ret))
        ret = instances[key] = new TimedInfSpiralAttack(initCooldown, cooldown, arms, offsetIncrement, offsetBase, projectileIndex);
      return ret;
    }

    protected override bool TickCore(RealmTime time)
    {

      Behavior behav = RingAttack.Instance(arms, 0, ((offsetIncrement * (float)Math.PI / 180) * incrementMultiplier) + offsetBase, projectileIndex);

      int remainingTick;
      object o;
      if (!Host.StateStorage.TryGetValue(Key, out o))
        remainingTick = initCooldown;
      else
        remainingTick = (int)o;

      remainingTick -= time.thisTickTimes;
      bool ret;
      if (remainingTick <= 0)
      {
        if (behav != null)
          behav.Tick(Host, time);
        if (behav != null)
          incrementMultiplier += 1;
        remainingTick = cooldown;
        ret = true;
      }
      else
        ret = false;
      Host.StateStorage[Key] = remainingTick;
      return ret;
    }
  }
}
