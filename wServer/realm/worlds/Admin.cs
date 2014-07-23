namespace wServer.realm.worlds
{
    public class Admin : World
    {
        public Admin()
        {
            Id = ADM_ID;
            Name = "Admin Room";
            Background = 0;
            SetMusic("Nexus", "Nexus1", "Nexus2");
            base.FromWorldMap(typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.admin.wmap"));
        }
    }
}