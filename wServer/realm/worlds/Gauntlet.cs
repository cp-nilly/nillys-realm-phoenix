namespace wServer.realm.worlds
{
    public class GauntletMap : World
    {
        public GauntletMap()
        {
            Id = GAUNTLET;
            Name = "The Gauntlet";
            Background = 0;
            AllowTeleport = false;
            SetMusic("Overworld");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.gauntlet.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new GauntletMap());
        }
    }
}