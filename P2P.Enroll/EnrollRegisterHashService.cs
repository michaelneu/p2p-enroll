using Be.IO;
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
                using (var writer = new BeBinaryWriter(stream))
                {
                    message.SerializeForHash(writer);

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

        public void UpdateNonce(long nonce)
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

            if (hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && hash[3] == 0)
            {
                return 32;
            }

            return 0;
        }
    }
}
