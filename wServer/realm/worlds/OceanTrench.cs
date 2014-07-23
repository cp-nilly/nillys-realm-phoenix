namespace wServer.realm.worlds
{
    public class OceanTrench : World
    {
        public OceanTrench()
        {
            Name = "Ocean Trench";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Ocean Trench");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.oceantrench.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new OceanTrench());
        }
    }
}