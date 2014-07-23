#region

using System;

#endregion

namespace wServer
{
    public class RC4
    {
        private readonly byte[] m_State = new byte[256];

        public RC4(byte[] key)
        {
            for (var i = 0; i < 256; i++)
            {
                m_State[i] = (byte) i;
            }

            X = 0;
            Y = 0;

            var index1 = 0;
            var index2 = 0;

            byte tmp;

            if (key == null || key.Length == 0)
            {
                throw new Exception();
            }

            for (var i = 0; i < 256; i++)
            {
                index2 = ((key[index1] & 0xff) + (m_State[i] & 0xff) + index2) & 0xff;

                tmp = m_State[i];
                m_State[i] = m_State[index2];
                m_State[index2] = tmp;

                index1 = (index1 + 1)%key.Length;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }

        public byte[] State
        {
            get
            {
                var buf = new byte[256];
                Array.Copy(m_State, buf, 256);
                return buf;
            }
            set { Array.Copy(value, m_State, 256); }
        }

        public byte[] Crypt(byte[] buf, int len)
        {
            int xorIndex;
            byte tmp;

            if (buf == null)
            {
                return null;
            }

            var result = new byte[len];

            for (var i = 0; i < len; i++)
            {
                X = (X + 1) & 0xff;
                Y = ((m_State[X] & 0xff) + Y) & 0xff;

                tmp = m_State[X];
                m_State[X] = m_State[Y];
                m_State[Y] = tmp;

                xorIndex = ((m_State[X] & 0xff) + (m_State[Y] & 0xff)) & 0xff;
                result[i] = (byte) (buf[i] ^ m_State[xorIndex]);
            }

            return result;
        }
    }
}