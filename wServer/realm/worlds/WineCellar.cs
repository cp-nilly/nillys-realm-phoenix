namespace wServer.realm.worlds
{
    public class WineCellarMap : World
    {
        public WineCellarMap()
        {
            Id = WC;
            Name = "Wine Cellar";
            Background = 0;
            AllowTeleport = false;
            SetMusic("Oryx");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.winecellar.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new WineCellarMap());
        }
    }
}