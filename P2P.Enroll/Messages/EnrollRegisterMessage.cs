using System.IO;
using System.Text;

namespace P2P.Enroll.Messages
{
    class EnrollRegisterMessage : Message
    {
        public override ushort Size => base.Size;

        public ulong Challenge
        {
            get;
            set;
        }

        public ushort TeamNumber
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

        public EnrollRegisterMessage()
        {
            Type = MessageType.EnrollRegister;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Challenge);
            writer.Write(TeamNumber);
            writer.Write((ushort)Project);
            writer.Write(Nonce);
            writer.Write(Encoding.UTF8.GetBytes(Email));
            writer.Write(Encoding.UTF8.GetBytes(Firstname));
            writer.Write(Encoding.UTF8.GetBytes(Lastname));
        }
    }
}
