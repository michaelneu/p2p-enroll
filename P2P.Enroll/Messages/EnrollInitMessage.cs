using System.IO;

namespace P2P.Enroll.Messages
{
    class EnrollInitMessage : Message
    {
        public override short Size => (short)(base.Size + 8);

        public long Challenge
        {
            get;
            private set;
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Challenge = reader.ReadInt64();
        }
    }
}
