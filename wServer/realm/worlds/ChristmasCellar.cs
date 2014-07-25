namespace wServer.realm.worlds
{
    public class ChristmasCellarMap : World
    {
        public ChristmasCellarMap()
        {
            Name = "Christmas Cellar";
            Background = 0;
            AllowTeleport = false;
            SetMusic("Oryx");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.christmascellar.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new ChristmasCellarMap());
        }
    }
}