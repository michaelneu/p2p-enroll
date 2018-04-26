using System.IO;

namespace P2P.Enroll.Messages
{
    class EnrollSuccessMessage : Message
    {
        public short TeamNumber
        {
            get;
            private set;
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var reservedField = reader.ReadInt16();

            TeamNumber = reader.ReadInt16();
        }
    }
}
