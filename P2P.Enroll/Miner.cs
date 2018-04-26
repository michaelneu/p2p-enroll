using System;
using System.Threading;

namespace P2P.Enroll
{
    class Miner
    {
        private readonly Thread thread;
        private readonly EnrollRegisterHashService service;
        private readonly int minZeroes;
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

        public Miner(EnrollRegisterHashService service, long start, long end, int minZeroes = 32)
        {
            this.service = service;
            this.start = start;
            this.end = end;
            this.minZeroes = minZeroes;
            
            thread = new Thread(Mine);
        }

        public void Start()
        {
            thread.Start();
        }

        private void Mine()
        {
            for (long i = start; i < end; i++)
            {
                service.UpdateNonce(i);

                var zeroes = service.GetCurrentCountOfZeroes();

                if (zeroes >= minZeroes)
                {
                    Result = i;
                    break;
                }
            }
        }
    }
}
