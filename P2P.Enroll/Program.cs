﻿using Be.IO;
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
            var pool = new Miner[4];
            var joinTimeout = new TimeSpan(0, 0, 0, 29);
            var mineStep = long.MaxValue / pool.Length;
            var difficulty = 32;

            while (true)
            {
                using (var client = new TcpClient("fulcrum.net.in.tum.de", 34151))
                using (var stream = client.GetStream())
                using (var reader = new BeBinaryReader(stream))
                using (var writer = new BeBinaryWriter(stream))
                {
                    var init = new EnrollInitMessage();
                    init.Deserialize(reader);

                    Console.WriteLine($"-> received challenge {init.Challenge}");

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
                        var start = (long)i * mineStep;
                        var end = start + mineStep;
                        var miner = new Miner(service, start, end, difficulty);

                        miner.Start();
                        pool[i] = miner;
                    }

                    pool[0].Join(joinTimeout);
                    foreach (var item in pool.Skip(1))
                    {
                        item.Join(new TimeSpan());
                    }

                    var nonce = pool.Select(x => x.Result)
                                    .Where(x => x.HasValue)
                                    .FirstOrDefault();

                    if (nonce == null)
                    {
                        Console.WriteLine($"-> no luck");
                        nonce = 0;
                    }
                    else
                    {
                        Console.WriteLine($"-> found nonce {nonce}");
                    }

                    register.Nonce = nonce.Value;
                    register.Serialize(writer);
                    writer.Flush();

                    var resultMessage = new EnrollResultMessage();

                    resultMessage.Deserialize(reader);

                    if (resultMessage.WasSuccessful)
                    {
                        Console.WriteLine(resultMessage.TeamNumber);

                        File.WriteAllText("result.txt", resultMessage.TeamNumber + "");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine($"-> error #{resultMessage.ErrorNumber}: {resultMessage.ErrorMessage}");
                    }
                }
            }
        }
    }
}
