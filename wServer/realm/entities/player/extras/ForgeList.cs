#region

using System.Collections.Generic;

#endregion

namespace wServer.realm.entities.player
{
    public class ForgeList
    {
        public Dictionary<string[], string> combos = new Dictionary<string[], string>();

        public ForgeList()
        {
            AddCombo("Staff of Unbound Prejudice", "Staff of Extreme Prejudice", "Staff of Extreme Prejudice");
            AddCombo("Staff of Unbound Prejudice", "Wand of the Bulwark", "Staff of Extreme Prejudice");
        }

        public void AddCombo(string result, params string[] items)
        {
            combos.Add(items, result);
        }
    }
}