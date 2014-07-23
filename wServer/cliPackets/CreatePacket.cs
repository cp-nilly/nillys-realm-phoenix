namespace wServer.cliPackets
{
    public class CreatePacket : ClientPacket
    {
        public short ObjectType { get; set; }

        public override PacketID ID
        {
            get { return PacketID.Create; }
        }

        public override Packet CreateInstance()
        {
            return new CreatePacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            ObjectType = rdr.ReadInt16();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.Write(ObjectType);
        }
    }
}