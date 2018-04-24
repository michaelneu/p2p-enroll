using System;
using System.Threading;

namespace P2P.Enroll
{
    class Miner
    {
        private readonly Thread thread;
        private readonly EnrollRegisterHashService service;
        private readonly int minZeroes;
        private readonly ulong start, end;

        public bool IsBusy
        {
            get
            {
                return thread.IsAlive;
            }
        }

        public ulong? Result
        {
            get;
            private set;
        }

        public Miner(EnrollRegisterHashService service, ulong start, ulong end, int minZeroes = 32)
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
            for (ulong i = start; i < end; i++)
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
