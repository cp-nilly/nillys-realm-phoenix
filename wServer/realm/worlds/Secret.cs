namespace wServer.realm.worlds
{
    public class Secret : World
    {
        public Secret()
        {
            Name = "?????";
            Background = 0;
            AllowTeleport = false;
            SetMusic("clooooeebe mixex");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.secret.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Secret());
        }
    }
}