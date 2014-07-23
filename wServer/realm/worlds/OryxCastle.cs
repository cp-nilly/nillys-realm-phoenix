namespace wServer.realm.worlds
{
  public class OryxCastleMap : World
  {
    public OryxCastleMap()
    {
      Name = "Oryx's Castle";
      Background = 0;
      AllowTeleport = false;
      SetMusic("Oryx");
      base.FromWorldMap(
          typeof(RealmManager).Assembly.GetManifestResourceStream("wServer.realm.worlds.OryxCastle.wmap"));
    }

    public override World GetInstance(ClientProcessor psr)
    {
      return RealmManager.AddWorld(new OryxCastleMap());
    }
  }
}