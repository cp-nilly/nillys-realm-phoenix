namespace wServer.realm.worlds
{
    public class TurkeyMap : World
    {
        public TurkeyMap()
        {
            Name = "Turkey Hunting Grounds";
            Background = 0;
            AllowTeleport = false;
            SetMusic("Oryx");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.turkey.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new TurkeyMap());
        }
    }
}