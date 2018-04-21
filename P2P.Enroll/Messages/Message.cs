using System.IO;

namespace P2P.Enroll.Messages
{
    abstract class Message
    {
        public virtual ushort Size => 4;

        protected virtual MessageType Type
        {
            get;
            set;
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            var messageSize = reader.ReadInt16();

            Type = (MessageType)reader.ReadInt16();
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Size);
            writer.Write((ushort)Type);
        }
    }
}
