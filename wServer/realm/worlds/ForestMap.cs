namespace wServer.realm.worlds
{
    public class ForestMap : World
    {
        public ForestMap()
        {
            Name = "Forest Sanctuary";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Overworld");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.forestmap.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new ForestMap());
        }
    }
}