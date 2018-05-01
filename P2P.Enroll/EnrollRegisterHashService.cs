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
                    bytes = stream.ToArray();
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

            bytes[12] = nonceBytes[7];
            bytes[13] = nonceBytes[6];
            bytes[14] = nonceBytes[5];
            bytes[15] = nonceBytes[4];
            bytes[16] = nonceBytes[3];
            bytes[17] = nonceBytes[2];
            bytes[18] = nonceBytes[1];
            bytes[19] = nonceBytes[0];
        }

        public byte[] ComputeSha256()
        {
            return sha.ComputeHash(bytes);
        }
    }
}
