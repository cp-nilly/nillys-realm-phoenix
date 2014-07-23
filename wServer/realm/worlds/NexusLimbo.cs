namespace wServer.realm.worlds
{
    public class NexusLimbo : World
    {
        public NexusLimbo()
        {
            Id = NEXUS_LIMBO;
            Name = "Nexus Tutorial";
            Background = 0;
            SetMusic("Nexus", "Nexus2", "Nexus3");
            base.FromWorldMap(
                typeof (RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.nexusLimbo.wmap"));
        }
    }
}