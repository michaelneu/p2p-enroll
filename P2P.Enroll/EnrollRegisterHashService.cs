using P2P.Enroll.Messages;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace P2P.Enroll
{
    class EnrollRegisterHashService
    {
        private readonly SHA256 sha;
        private readonly byte[] bytes;

        public EnrollRegisterHashService(EnrollRegisterMessage message)
        {
            sha = SHA256.Create();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    message.Serialize(writer);

                    stream.Flush();
                    bytes = stream.GetBuffer();
                }
            }
        }

        public EnrollRegisterHashService(EnrollRegisterHashService otherService)
        {
            sha = new SHA256Managed();
            bytes = new byte[otherService.bytes.Length];

            Array.Copy(otherService.bytes, bytes, bytes.Length);
        }

        public void UpdateNonce(ulong nonce)
        {
            var nonceBytes = BitConverter.GetBytes(nonce);

            for (int i = 0; i < nonceBytes.Length; i++)
            {
                bytes[i + 16] = nonceBytes[i];
            }
        }

        private byte[] ComputeSha256()
        {
            return sha.ComputeHash(bytes);
        }

        public int GetCurrentCountOfZeroes()
        {
            var hash = ComputeSha256();
            var count = 0;

            for (var i = 0; i < hash.Length; i++)
            {
                var currentByte = hash[i];

                for (int k = 0; k < 8; k++)
                {
                    bool byteIsZero = ((currentByte >> k) & 1) == 1;

                    if (byteIsZero)
                    {
                        count++;
                    }
                    else
                    {
                        return count;
                    }
                }
            }

            return count;
        }
    }
}
