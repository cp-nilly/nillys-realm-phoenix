namespace wServer.realm.worlds
{
    public class LairofDraconis : World
    {
        public LairofDraconis()
        {
            Name = "Lair of Draconis";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Abyss"); //hehe get it
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.draconis.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new LairofDraconis());
        }
    }
}