namespace wServer.realm.worlds
{
  public class OryxChamberMap : World
  {
    public OryxChamberMap()
    {
      Name = "Oryx's Chamber";
      Background = 0;
      AllowTeleport = false;
      SetMusic("Oryx");
      base.FromWorldMap(
          typeof(RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.OryxChamber.wmap"));
    }

    public override World GetInstance(ClientProcessor psr)
    {
      return RealmManager.AddWorld(new OryxChamberMap());
    }
  }
}