using System.Net;

namespace Simulator.RobotEssentials
{
    class Message
    {
        readonly byte[] Bytes;
        FrameHeader framehead;
        MessageHeader messagehead;
        MessageBody messagebody;

        public Message(FrameHeader fh, MessageHeader mh, MessageBody mb)
        {
            framehead = fh;
            messagehead = mh;
            messagebody = mb;
            var length = fh.GetBytes().Length + mh.GetBytes().Length + mb.GetBytes().Length;
            Bytes = new byte[length];
            Buffer.BlockCopy(framehead.GetBytes(), 0, Bytes, 0, framehead.Length);
            Buffer.BlockCopy(messagehead.GetBytes(), 0, Bytes, framehead.Length, messagehead.Length);
            Buffer.BlockCopy(messagebody.GetBytes(), 0, Bytes, framehead.Length + messagehead.Length, messagebody.Length);
            //PrintBytes(Bytes);
        }

        public byte[] GetBytes()
        {
            return Bytes;
        }

        private void PrintBytes(IEnumerable<byte> toPrint)
        {
            Console.WriteLine("Printing Bytes = ");
            foreach (var b in toPrint)
            {
                Console.Write("\\x{0}", b.ToString().PadLeft(2, '0'));
            }
            Console.WriteLine("");
        }

    }

    class FrameHeader
    {
        public static byte Version = 2;
        public static byte Cipher = 0;
        public static byte Reserved = 0;
        public static byte Reserved2 = 0;
        byte[] Bytes;
        public int Length; public FrameHeader(uint payload)
        {
            Bytes = new byte[8];
            Bytes[0] = Version;
            Bytes[1] = Cipher;
            Bytes[2] = Reserved;
            Bytes[3] = Reserved2;
            var payloadbytes = GetBytesfrom32(payload);
            Bytes[4] = payloadbytes[0];
            Bytes[5] = payloadbytes[1];
            Bytes[6] = payloadbytes[2];
            Bytes[7] = payloadbytes[3];
            Length = Bytes.Length;
        }
        public byte[] GetBytes()
        {
            return Bytes;
        }
        public byte[] GetBytesfrom32(uint val)
        {
            var netorder = IPAddress.HostToNetworkOrder(checked((int)val));
            var res = BitConverter.GetBytes(netorder);
            return res;
        }
    }

    class MessageBody
    {
        byte[] Bytes;
        public int Length;
        public MessageBody(byte[] bytearray)
        {
            Bytes = bytearray;
            Length = Bytes.Length;
        }
        public byte[] GetBytes()
        {
            return Bytes;
        }
    }

    class MessageHeader
    {
        byte[] Bytes;
        public int Length;
        public MessageHeader(ushort cmp, ushort msg)
        {
            Bytes = new byte[4];
            var cmpBytes = GetBytesfrom16(cmp);
            var msgBytes = GetBytesfrom16(msg);
            Bytes[0] = cmpBytes[0];
            Bytes[1] = cmpBytes[1];
            Bytes[2] = msgBytes[0];
            Bytes[3] = msgBytes[1];
            Length = Bytes.Length;
        }
        public byte[] GetBytesfrom16(ushort val)
        {
            var netorder = IPAddress.HostToNetworkOrder(checked((short)val));
            var res = BitConverter.GetBytes(netorder);
            return res;
        }
        public byte[] GetBytes()
        {
            return Bytes;
        }
    }
}
