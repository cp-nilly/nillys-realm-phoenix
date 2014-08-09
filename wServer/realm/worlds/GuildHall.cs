#region

using common;

#endregion

namespace wServer.realm.worlds
{
    public class GuildHall : World
    {
        public GuildHall(string guild)
        {
            Id = GHALL;
            Guild = guild;
            Name = "Guild Hall";
            Background = 0;
            AllowTeleport = true;
            SetMusic("Guild Hall");
            switch (Level())
            {
                case 0:
                    base.FromWorldMap(
                        typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.ghall0.wmap"));
                    break;
                case 1:
                    base.FromWorldMap(
                        typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.ghall1.wmap"));
                    break;
                case 2:
                    base.FromWorldMap(
                        typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.ghall2.wmap"));
                    break;
                case 3:
                    base.FromWorldMap(
                        typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.ghall3.wmap"));
                    break;
            }
            //base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.guildhall0old.wmap"));            
        }

        public string Guild { get; set; }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new GuildHall(Guild));
        }

        public int Level()
        {
            using (var dbx = new Database())
            {
                int id = dbx.GetGuildId(Guild);
                return dbx.GetGuildLevel(id);
            }
        }
    }
}