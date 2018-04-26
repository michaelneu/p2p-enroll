using System;
using System.Threading;

namespace P2P.Enroll
{
    class Miner
    {
        private readonly Thread thread;
        private readonly EnrollRegisterHashService service;
        private readonly long start, end;

        public bool IsBusy
        {
            get
            {
                return thread.IsAlive;
            }
        }

        public long? Result
        {
            get;
            private set;
        }

        public Miner(EnrollRegisterHashService service, long start, long end)
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

        private void Mine()
        {
            for (long i = start; i < end; i++)
            {
                service.UpdateNonce(i);

                var hash = service.ComputeSha256();

                if (hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && hash[3] == 0)
                {
                    Result = i;
                    break;
                }
            }
        }
    }
}
