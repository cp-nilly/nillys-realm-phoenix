namespace wServer.realm.worlds
{
    public class Nexus : World
    {
        public Nexus()
        {
            Id = NEXUS_ID;
            Name = "Nexus";
            Background = 2;
            SetMusic("Nexus", "Nexus2", "Nexus3");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.nexus.wmap"));
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time); //normal world tick

            CheckDupers();
            UpdatePortals();
        }

        private void CheckDupers()
        {
            foreach (var w in RealmManager.Worlds)
            {
                foreach (var x in RealmManager.Worlds)
                {
                    foreach (var y in w.Value.Players)
                    {
                        foreach (var z in x.Value.Players)
                        {
                            if (y.Value.AccountId == z.Value.AccountId && y.Value != z.Value)
                            {
                                y.Value.Client.Disconnect();
                                z.Value.Client.Disconnect();
                            }
                        }
                    }
                }
            }
        }
        private void UpdatePortals()
        {
            foreach (var i in RealmManager.Monitor.portals)
            {
                foreach (var it in RealmManager.allRealmNames)
                {
                    if (i.Value.Name.StartsWith(it))
                    {
                        i.Value.Name = it + " (" + i.Key.Players.Count + "/" + RealmManager.MAX_INREALM + ")";
                        i.Value.UpdateCount++;
                        break;
                    }
                }
            }
        }
    }
}