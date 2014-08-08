namespace wServer.svrPackets
{
    public class GuildAllyRequestPacket : ServerPacket
    {
        public string Guild;
        public string Name;

        public override PacketID ID
        {
            get { return PacketID.GuildAllyRequest; }
        }

        public override Packet CreateInstance()
        {
            return new GuildAllyRequestPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            Name = rdr.ReadUTF();
            Guild = rdr.ReadUTF();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.WriteUTF(Name);
            wtr.WriteUTF(Guild);
        }
    }
}