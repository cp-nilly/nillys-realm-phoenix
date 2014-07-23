#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class SetAltTexture : Behavior
    {
        private static readonly Dictionary<int, SetAltTexture> instances = new Dictionary<int, SetAltTexture>();
        private readonly int index;

        private SetAltTexture(int index)
        {
            this.index = index;
        }

        public static SetAltTexture Instance(int index)
        {
            SetAltTexture ret;
            if (!instances.TryGetValue(index, out ret))
                ret = instances[index] = new SetAltTexture(index);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if ((Host.Self as Enemy).AltTextureIndex != index)
            {
                (Host.Self as Enemy).AltTextureIndex = index;
                Host.Self.UpdateCount++;
            }
            return true;
        }
    }

    internal class TimedToggleTexture : Behavior
    {
      private static readonly Dictionary<Tuple<int, int, int>, TimedToggleTexture> instances =
        new Dictionary<Tuple<int, int, int>, TimedToggleTexture>();

      private readonly int cooldown;
      private readonly int texture1;
      private readonly int texture2;

      private TimedToggleTexture(int cooldown, int texture1, int texture2)
      {
        this.cooldown = cooldown;
        this.texture1 = texture1;
        this.texture2 = texture2;
      }

      public static TimedToggleTexture Instance(int cooldown, int texture1, int texture2)
      {
        var key = new Tuple<int, int, int>(cooldown, texture1, texture2);
        TimedToggleTexture ret;
        if (!instances.TryGetValue(key, out ret))
          ret = instances[key] = new TimedToggleTexture(cooldown, texture1, texture2);
        return ret;
      }
      bool texture = false;
      protected override bool TickCore(RealmTime time)
      {
        int index;
        bool ret;
        int remainingTick;
        object o;
        if (!Host.StateStorage.TryGetValue(Key, out o))
        {
          remainingTick = cooldown;
        }
        else
          remainingTick = (int)o;

        remainingTick -= time.thisTickTimes;

        if (remainingTick <= 0)
        {
          texture = !texture;
          remainingTick = cooldown;
          ret = true;
        }
        else
          ret = false;
        if (texture == false) index = texture1;
        else index = texture2;

        if ((Host.Self as Enemy).AltTextureIndex != index)
        {
          (Host.Self as Enemy).AltTextureIndex = index;
          Host.Self.UpdateCount++;
        }
        Host.StateStorage[Key] = remainingTick;
        return ret;
      }
    }
}