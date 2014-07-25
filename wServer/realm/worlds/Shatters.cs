namespace wServer.realm.worlds
{
    public class Shatters : World
    {
        public Shatters()
        {
            Name = "The Shatters";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Arena");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.shatters.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Shatters());
        }
    }
}