using P2P.Enroll.Messages;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace P2P.Enroll
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var challenge = random.Next();
            
            var watch = new Stopwatch();
            watch.Start();

            var message = new EnrollRegisterMessage
            {
                Challenge = (ulong)challenge,
                TeamNumber = 0,
                Project = ProjectChoice.NSE,
                Nonce = ulong.MaxValue,
                Email = "",
                Firstname = "",
                Lastname = ""
            };

            var baseService = new EnrollRegisterHashService(message);
            var pool = new Miner[4];

            ulong maxValue = 10_000_000;
            var step = maxValue / (ulong)pool.Length;

            for (int i = 0; i < pool.Length; i++)
            {
                var service = new EnrollRegisterHashService(baseService);
                var start = (ulong)i * step;
                var end = start + step;

                pool[i] = new Miner(service, start, end, 32);
            }

            watch.Stop();
            Console.WriteLine($"setup took {watch.ElapsedMilliseconds}ms, starting threads");
            watch.Restart();

            foreach (var item in pool)
            {
                item.Start();
            }

            var poolFinished = true;

            do
            {
                poolFinished = pool.Any(x => !x.IsBusy);
                Thread.Sleep(10);
            } while (!poolFinished);

            watch.Stop();
            var mhs = maxValue / (double)watch.ElapsedMilliseconds / 1000;
            Console.WriteLine($"mining through {maxValue} nonces took {watch.ElapsedMilliseconds}ms, that's {mhs:0.00} MH/s");
        }

        static void TestConnection(string[] args)
        {
            using (var client = new TcpClient("fulcrum.net.in.tum.de", 34151))
            {
                using (var stream = client.GetStream())
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var init = new EnrollInitMessage();
                        init.Deserialize(reader);
                    }
                }
            }
        }
    }
}
