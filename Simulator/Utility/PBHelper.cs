using System.Net;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Simulator.RobotEssentials
{
    class Message
    {
        readonly byte[] Bytes;

        byte[]? init_vector;
        FrameHeader framehead;
        MessageHeader messagehead;
        MessageBody messagebody;

        public Message(FrameHeader fh, MessageHeader mh, MessageBody mb, string? keyphrase = null)
        {
            framehead = fh;
            messagehead = mh;
            messagebody = mb;
            var length = fh.GetBytes().Length + mh.GetBytes().Length + mb.GetBytes().Length;
            Bytes = new byte[length];

            if(keyphrase != null) {
                framehead.SetCypher();
            }
            Buffer.BlockCopy(framehead.GetBytes(), 0, Bytes, 0, framehead.Length);

            if(keyphrase != null) {
                init_vector = GenerateIV();

                //Encrypt message
                byte[] data = new byte[messagehead.Length + messagebody.Length];
                Buffer.BlockCopy(messagehead.GetBytes(), 0, data, 0, messagehead.Length);
                Buffer.BlockCopy(messagebody.GetBytes(), 0, data, messagehead.Length, messagebody.Length);
                byte[] enc_data = Encrypt(keyphrase, init_vector, data);

                //Resize buffer to fit padding and iv
                Array.Resize(ref Bytes, enc_data.Length + init_vector.Length + framehead.Length);

                Buffer.BlockCopy(init_vector, 0, Bytes, framehead.Length, init_vector.Length);
                Buffer.BlockCopy(enc_data, 0, Bytes, framehead.Length + init_vector.Length, enc_data.Length);
            } else {
                Buffer.BlockCopy(messagehead.GetBytes(), 0, Bytes, framehead.Length, messagehead.Length);
                Buffer.BlockCopy(messagebody.GetBytes(), 0, Bytes, framehead.Length + messagehead.Length, messagebody.Length);
            }
        }

        public byte[] Encrypt(string keyphrase, byte[] iv, byte[] data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKey(keyphrase, 8);
                // Use the derived 32-byte key (AES-256)
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        private static byte[] DeriveKey(string pwd, int count)
        {
            byte[] data = Encoding.UTF8.GetBytes(pwd);
            List<byte> hashList = new List<byte>();
            byte[] currentHash = new byte[0];

            int preHashLength = data.Length;
            byte[] preHash = new byte[preHashLength];

            System.Buffer.BlockCopy(data, 0, preHash, 0, data.Length);

            using (SHA256 hash = SHA256.Create())
            {
                currentHash = hash.ComputeHash(preHash);

                for (int i = 1; i < count; i++)
                {
                    currentHash = hash.ComputeHash(currentHash);
                }

                hashList.AddRange(currentHash);

                while (hashList.Count < 48) // for 32-byte key and 16-byte iv
                {
                    preHashLength = currentHash.Length + data.Length;
                    preHash = new byte[preHashLength];

                    System.Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                    System.Buffer.BlockCopy(data, 0, preHash, currentHash.Length, data.Length);

                    currentHash = hash.ComputeHash(preHash);

                    for (int i = 1; i < count; i++)
                    {
                        currentHash = hash.ComputeHash(currentHash);
                    }

                    hashList.AddRange(currentHash);
                }
            }

            byte[] key = new byte[32];
            hashList.CopyTo(0, key, 0, 32);
            return key;
        }
        // Method to generate a random IV
        public byte[] GenerateIV()
        {
            byte[] iv = new byte[16]; // AES block size is 16 bytes
            RandomNumberGenerator.Fill(iv);
            return iv;
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
        public static readonly byte AES_256_CBC_BYTE = 0x4;
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
        public void SetCypher() {
            Bytes[1] = AES_256_CBC_BYTE;
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
