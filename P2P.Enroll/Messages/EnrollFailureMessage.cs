using System.IO;

namespace P2P.Enroll.Messages
{
    class EnrollFailureMessage : Message
    {
        public ushort ErrorNumber
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var reservedField = reader.ReadUInt16();

            ErrorNumber = reader.ReadUInt16();
            ErrorMessage = reader.ReadString();
        }
    }
}
