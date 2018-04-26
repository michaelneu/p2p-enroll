using System;
using System.IO;

namespace P2P.Enroll.Messages
{
    abstract class Message
    {
        public virtual short Size => _size;
        private short _size = 4;

        protected virtual MessageType Type
        {
            get;
            set;
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            _size = (short)reader.ReadInt16();
            Type = (MessageType)reader.ReadInt16();
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Size);
            writer.Write((short)Type);
        }
    }
}
