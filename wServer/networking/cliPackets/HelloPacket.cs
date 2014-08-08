namespace wServer.cliPackets
{
    public class HelloPacket : ClientPacket
    {
        public string Copyright { get; set; }
        public string BuildVersion { get; set; }
        public int GameId { get; set; }
        public string GUID { get; set; }
        public int AnotherThing { get; set; }
        public string Password { get; set; }
        public string Secret { get; set; }
        public int KeyTime { get; set; }
        public byte[] Key { get; set; }
        public string MapInfo { get; set; }
        public string __Rw { get; set; }
        public string __06U { get; set; }
        public string __LK { get; set; }
        public string PlayPlatform { get; set; }

        public override PacketID ID
        {
            get { return PacketID.Hello; }
        }

        public override Packet CreateInstance()
        {
            return new HelloPacket();
        }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            Copyright = rdr.ReadUTF();
            BuildVersion = rdr.ReadUTF();
            GameId = rdr.ReadInt32();
            GUID = RSA.Instance.Decrypt(rdr.ReadUTF());
            AnotherThing = rdr.ReadInt32();
            Password = RSA.Instance.Decrypt(rdr.ReadUTF());
            Secret = RSA.Instance.Decrypt(rdr.ReadUTF());
            KeyTime = rdr.ReadInt32();
            Key = rdr.ReadBytes(rdr.ReadInt16());
            MapInfo = rdr.Read32UTF();
            __Rw = rdr.ReadUTF();
            __06U = rdr.ReadUTF();
            __LK = rdr.ReadUTF();
            PlayPlatform = rdr.ReadUTF();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.WriteUTF(Copyright);
            wtr.WriteUTF(BuildVersion);
            wtr.Write(GameId);
            wtr.WriteUTF(RSA.Instance.Encrypt(GUID));
            wtr.Write(AnotherThing);
            wtr.WriteUTF(RSA.Instance.Encrypt(Password));
            wtr.WriteUTF(RSA.Instance.Encrypt(Secret));
            wtr.Write(KeyTime);
            wtr.Write((short) Key.Length);
            wtr.Write(Key);
            wtr.Write32UTF(MapInfo);
            wtr.WriteUTF(__Rw);
            wtr.WriteUTF(__06U);
            wtr.WriteUTF(__LK);
            wtr.WriteUTF(PlayPlatform);
        }
    }
}