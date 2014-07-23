#region

using System.IO;
using terrain;

#endregion

namespace wServer.realm.worlds
{
    public class Test : World
    {
        public string js = null;

        public Test()
        {
            Id = TEST_ID;
            Name = "Test";
            Background = 0;
            SetMusic("Island");
        }

        public void LoadJson(string json)
        {
            js = json;
            FromWorldMap(new MemoryStream(Json2Wmap.Convert(json)));
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (var i in Players)
            {
                if (i.Value.Client.Account.Rank < 3)
                {
                    i.Value.Client.Disconnect();
                }
            }
        }
    }
}