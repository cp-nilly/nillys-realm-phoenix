namespace wServer.realm.worlds
{
    public class PartyCellarMap : World
    {
        public PartyCellarMap()
        {
            Name = "Party Cellar";
            Background = 0;
            AllowTeleport = false;
            SetMusic("Oryx");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.partycellar.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new PartyCellarMap());
        }
    }
}