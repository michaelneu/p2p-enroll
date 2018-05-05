using Be.IO;
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
        private static int attempt = 1;

        private static void Log(string message)
        {
            var loggedMessage = $"[{attempt,5}] [{DateTime.Now}] -> {message}";

            Console.WriteLine(loggedMessage);
            File.AppendAllLines("log.txt", new string[] { loggedMessage });
        }

        static void Main(string[] args)
        {
            var pool = new Miner[8];
            var joinTimeout = new TimeSpan(0, 0, 10, 0);
            var mineStep = ulong.MaxValue / (ulong)pool.Length;
            var didOverrideNonce = false;

            while (true)
            {
                using (var client = new TcpClient("fulcrum.net.in.tum.de", 34151))
                using (var stream = client.GetStream())
                using (var reader = new BeBinaryReader(stream))
                using (var writer = new BeBinaryWriter(stream))
                {
                    var init = new EnrollInitMessage();
                    init.Deserialize(reader);

                    Log($"received challenge {init.Challenge}");

                    var register = new EnrollRegisterMessage
                    {
                        Challenge = init.Challenge,
                        TeamNumber = 0,
                        Project = ProjectChoice.DHT,
                        Email = "",
                        Firstname = "",
                        Lastname = "",
                    };

                    var baseService = new EnrollRegisterHashService(register);

                    for (int i = 0; i < pool.Length; i++)
                    {
                        var service = new EnrollRegisterHashService(baseService);
                        var start = (ulong)i * mineStep;
                        var end = start + mineStep;
                        var miner = new Miner(service, start, end);

                        miner.Start();
                        pool[i] = miner;
                    }

                    var targetTime = DateTime.Now.Add(joinTimeout);
                    var isBusy = true;

                    while (isBusy && targetTime > DateTime.Now)
                    {
                        isBusy = pool.Any(x => x.IsBusy);
                        Thread.Sleep(1000);
                    }

                    foreach (var miner in pool)
                    {
                        miner.Stop();
                    }

                    var nonce = pool.Select(x => x.Nonce)
                                    .Where(x => x.HasValue)
                                    .FirstOrDefault();

                    if (nonce == null)
                    {
                        Log($"no luck");
                        nonce = 0;
                        didOverrideNonce = true;
                    }
                    else
                    {
                        var hex = pool.Where(x => x.Nonce == nonce)
                                      .Select(x => x.Hex)
                                      .FirstOrDefault();

                        Log($"found nonce {nonce} (hex: {hex})");
                    }

                    register.Nonce = nonce.Value;
                    register.Serialize(writer);
                    writer.Flush();

                    var resultMessage = new EnrollResultMessage();

                    resultMessage.Deserialize(reader);

                    if (resultMessage.WasSuccessful)
                    {
                        Log($"received team number {resultMessage.TeamNumber}");

                        File.WriteAllText("result.txt", resultMessage.TeamNumber + "");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Log($"error #{resultMessage.ErrorNumber}: {resultMessage.ErrorMessage}");

                        if (nonce.HasValue && !didOverrideNonce)
                        {
                            Log("should've matched, nonce was found!");
                            Environment.Exit(1);
                        }
                    }
                }

                attempt++;
                Console.WriteLine();
            }
        }

        static void CheckNonce(string[] args)
        {
            ulong challenge = 0;
            ulong nonce = 0;

            var message = new EnrollRegisterMessage
            {
                Challenge = challenge,
                Nonce = nonce,
                TeamNumber = 0,
                Project = ProjectChoice.DHT,
                Email = "",
                Firstname = "",
                Lastname = "",
            };

            var service = new EnrollRegisterHashService(message);
            var hash = service.ComputeSha256();
            var bytes = BitConverter.ToString(hash).Replace("-", "").ToLower();

            Console.WriteLine(bytes);
        }
    }
}
