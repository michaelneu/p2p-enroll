using System;
using System.Text;
using System.Threading;

namespace P2P.Enroll
{
    class Miner
    {
        private readonly Thread thread;
        private readonly EnrollRegisterHashService service;
        private readonly ulong start, end;

        public bool IsBusy
        {
            get
            {
                return thread.IsAlive;
            }
        }

        public ulong? Nonce
        {
            get;
            private set;
        }

        public string Hex
        {
            get;
            private set;
        }

        public Miner(EnrollRegisterHashService service, ulong start, ulong end)
        {
            this.service = service;
            this.start = start;
            this.end = end;
            
            thread = new Thread(Mine);
        }

        public void Start()
        {
            thread.Start();
        }

        public void Join(TimeSpan timeout)
        {
            thread.Join(timeout);
        }

        public void Stop()
        {
            try
            {
                thread.Abort();
            }
            catch
            { }
        }

        private void Mine()
        {
            for (ulong i = start; i < end; i++)
            {
                service.UpdateNonce(i);

                var hash = service.ComputeSha256();

                if (hash[28] == 0 && hash[29] == 0 && hash[30] == 0 && hash[31] == 0)
                {
                    var hex = new StringBuilder(hash.Length * 3);

                    foreach (var item in hash)
                    {
                        hex.AppendFormat("{0:x2}", item);
                    }

                    Hex = hex.ToString();
                    Nonce = i;
                    break;
                }
            }
        }
    }
}
