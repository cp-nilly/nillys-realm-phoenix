namespace wServer.realm.worlds
{
    public class MarketMap : World
    {
        public MarketMap()
        {
            Id = MARKET;
            Name = "Market";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Nexus", "Nexus2", "Nexus3");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.MarketMap.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new GauntletMap());
        }
    }
}