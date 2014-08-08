namespace wServer.cliPackets
{
    public class BroadcastPacket : ClientPacket
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }

        public override PacketID ID
        {
            get { return PacketID.Broadcast; }
        }

        public override Packet CreateInstance()
        {
            return new BroadcastPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            Username = rdr.Read32UTF();
            Password = rdr.Read32UTF();
            Message = rdr.Read32UTF();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.Write32UTF(Username);
            wtr.Write32UTF(Password);
            wtr.Write32UTF(Message);
        }
    }
}