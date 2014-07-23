namespace wServer.realm.worlds
{
    public class ArenaMap : World
    {
        public ArenaMap()
        {
            Id = ARENA;
            Name = "Arena";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.arena.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new ArenaMap());
        }
    }
}