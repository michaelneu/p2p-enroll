using System.IO;
using System.Text;

namespace P2P.Enroll.Messages
{
    class EnrollFailureMessage : Message
    {
        public short ErrorNumber
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

            var reservedField = reader.ReadInt16();

            ErrorNumber = reader.ReadInt16();

            var messageBytes = reader.ReadBytes(Size - 4);
            ErrorMessage = Encoding.UTF8.GetString(messageBytes);
        }
    }
}
