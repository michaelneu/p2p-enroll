using System;
using System.IO;
using System.Text;

namespace P2P.Enroll.Messages
{
    class EnrollResultMessage : Message
    {
        public bool WasSuccessful
        {
            get;
            private set;
        }

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

        public short TeamNumber
        {
            get;
            private set;
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            var reservedField = reader.ReadInt16();

            if (Type == MessageType.EnrollSuccess)
            {
                WasSuccessful = true;
                TeamNumber = reader.ReadInt16();
            }
            else
            {
                WasSuccessful = false;
                ErrorNumber = reader.ReadInt16();

                var messageBytes = reader.ReadBytes(Size - 4);
                ErrorMessage = Encoding.UTF8.GetString(messageBytes);
            }
        }
    }
}
