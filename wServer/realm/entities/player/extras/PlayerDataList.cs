#region

using System.Collections.Concurrent;

#endregion

namespace wServer.realm.entities.player
{
    public class PlayerDataList
    {
        public static ConcurrentDictionary<string, GlobalPlayerData> Datas =
            new ConcurrentDictionary<string, GlobalPlayerData>();

        public static GlobalPlayerData GetData(string name)
        {
            if (!Datas.IsEmpty)
            {
                foreach (var i in Datas)
                {
                    if (i.Key == name)
                    {
                        return i.Value;
                    }
                }
            }
            var n = new GlobalPlayerData();
            Datas.TryAdd(name, n);
            return n;
        }
    }
}