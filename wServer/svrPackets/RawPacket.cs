namespace wServer.svrPackets
{
    public class RawPacket : ServerPacket
    {
        private PacketID id;
        public byte[] Content { get; set; }

        public PacketID PktID
        {
            set { id = value; }
        }

        public override PacketID ID
        {
            get { return id; }
        }

        public override Packet CreateInstance()
        {
            return new RawPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            //
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.Write(Content);
        }
    }
}