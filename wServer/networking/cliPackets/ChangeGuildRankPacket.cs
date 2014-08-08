namespace wServer.cliPackets
{
    public class ChangeGuildRankPacket : ClientPacket
    {
        public int GuildRank;
        public string Name;

        public override PacketID ID
        {
            get { return PacketID.ChangeGuildRank; }
        }

        public override Packet CreateInstance()
        {
            return new ChangeGuildRankPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            Name = rdr.ReadUTF();
            GuildRank = rdr.ReadInt32();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.WriteUTF(Name);
            wtr.Write(GuildRank);
        }
    }
}