using System.IO;

namespace P2P.Enroll.Messages
{
    class EnrollSuccessMessage : Message
    {
        public ushort TeamNumber
        {
            get;
            private set;
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var reservedField = reader.ReadUInt16();

            TeamNumber = reader.ReadUInt16();
        }
    }
}
