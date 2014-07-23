namespace wServer.realm.worlds
{
    public class SnakePit : World
    {
        public SnakePit()
        {
            Name = "Snake Pit";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Snake Pit");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.snakepit.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new SnakePit());
        }
    }
}