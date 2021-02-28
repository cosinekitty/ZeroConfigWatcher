﻿using System;
using CosineKitty.ZeroConfigWatcher;
using Heijden.DNS;

namespace Watcher
{
    class Program
    {
        static int Main(string[] args)
        {
            using (var monitor = new TrafficMonitor())
            {
                monitor.OnReceive += OnTrafficReceived;
                monitor.Start();
                Console.WriteLine("Listening for traffic on {0} adapter(s). Press ENTER to quit.", monitor.ListeningAdapterCount);
                Console.WriteLine();
                Console.ReadLine();
            }
            return 0;
        }

        static void OnTrafficReceived(object sender, Packet e)
        {
            Console.WriteLine("=========================================================================");
            Console.WriteLine("{0} : packet from {1}", e.UtcArrival.ToString("o"), e.RemoteEndPoint.Address);
            Console.WriteLine();
            HexDump(e.Data);
            Console.WriteLine();
            Interpret(e.Data);
            Console.WriteLine();
        }

        static void HexDump(byte[] data)
        {
            Console.WriteLine("       0  1  2  3  4  5  6  7    8  9  a  b  c  d  e  f");
            Console.WriteLine("      -- -- -- -- -- -- -- --   -- -- -- -- -- -- -- --");
            for (int row = 0; row < data.Length; row += 0x10)
            {
                Console.Write("{0} ", row.ToString("x4"));
                for (int col = 0; col < 0x10; ++col)
                {
                    int ofs = row + col;
                    if (col == 8)
                        Console.Write("  ");
                    if (ofs < data.Length)
                        Console.Write(" {0}", data[ofs].ToString("x2"));
                    else
                        Console.Write("   ");
                }

                Console.Write("  ");

                for (int col = 0; col < 0x10; ++col)
                {
                    int ofs = row + col;
                    if (ofs >= data.Length)
                        break;
                    if (data[ofs] >= 0x20 && data[ofs] <= 0x7f)
                        Console.Write("{0}", (char)data[ofs]);
                    else
                        Console.Write(".");
                }

                Console.WriteLine();
            }
        }

        static void Interpret(byte[] data)
        {
            var response = new Response(data);

            foreach (Question q in response.Questions)
                PrintQuestion(q);

            foreach (AnswerRR a in response.Answers)
                PrintRR(a);
        }

        private static void PrintQuestion(Question q)
        {
            Console.WriteLine("Question: [{0}] type={1} class={2}", q.QName, q.QType, q.QClass);
        }

        private static void PrintRR(RR a)
        {
            Console.WriteLine("Answer: [{0}] type={1} class={2}", a.NAME, a.Type, a.Class);
        }
    }
}
