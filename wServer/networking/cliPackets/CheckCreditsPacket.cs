namespace wServer.cliPackets
{
    public class CheckCreditsPacket : ClientPacket
    {
        public override PacketID ID
        {
            get { return PacketID.CheckCredits; }
        }

        public override Packet CreateInstance()
        {
            return new CheckCreditsPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
        }
    }
}