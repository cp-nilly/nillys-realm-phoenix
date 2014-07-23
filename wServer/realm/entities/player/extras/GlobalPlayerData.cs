#region

using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace wServer.realm.entities.player
{
    public class GlobalPlayerData
    {
        public GlobalPlayerData()
        {
            JGroup = new ConcurrentDictionary<int, string>();
            UsingGroup = false;
            Solo = false;
            VShare = false;
            Guild = false;
            invited = new List<string>();
        }

        public ConcurrentDictionary<int, string> JGroup { get; set; }
        public bool UsingGroup { get; set; }
        public bool Solo { get; set; }
        public bool VShare { get; set; }
        public bool Guild { get; set; }
        public List<string> invited { get; set; }
    }
}