using Be.IO;
using P2P.Enroll.Messages;
using System;
using System.Collections;
using System.IO;
using System.Linq;
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

            Array.Copy(nonceBytes, 0, bytes, 16, nonceBytes.Length);
        }

        public byte[] ComputeSha256()
        {
            return sha.ComputeHash(bytes);
        }
    }
}
