namespace wServer.realm.worlds
{
    public class EC : World
    {
        public EC()
        {
            Name = "The Eternal Crucible";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Haunted Cemetary"); //Placeholder
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.crucible.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new EC());
        }
    }
}