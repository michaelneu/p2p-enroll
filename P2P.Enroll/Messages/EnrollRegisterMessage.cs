using System.IO;
using System.Text;

namespace P2P.Enroll.Messages
{
    class EnrollRegisterMessage : Message
    {
        public override short Size => (short)(base.Size + 8 + 2 + 2 + 8 + Body.Length);

        public ulong Challenge
        {
            get;
            set;
        }

        public short TeamNumber
        {
            get;
            set;
        }

        public ProjectChoice Project
        {
            get;
            set;
        }

        public ulong Nonce
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Firstname
        {
            get;
            set;
        }

        public string Lastname
        {
            get;
            set;
        }

        private string Body => $"{Email}\r\n{Firstname}\r\n{Lastname}";

        public EnrollRegisterMessage()
        {
            Type = MessageType.EnrollRegister;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            SerializeForHash(writer);
        }

        public void SerializeForHash(BinaryWriter writer)
        {
            writer.Write(Challenge);
            writer.Write(TeamNumber);
            writer.Write((short)Project);
            writer.Write(Nonce);
            writer.Write(Encoding.UTF8.GetBytes(Body));
        }
    }
}
