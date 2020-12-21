using Newtonsoft.Json;
using Server.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            StartListening();
            stopwatch.Stop();

            var listLog = DeserializeData(data);

            Console.WriteLine($"\t\n\nTempo passado: {stopwatch.Elapsed}");

            Console.WriteLine("Press any key for the close console...") ;
            Console.ReadKey();
        }

        public static string data = null;

        public static void StartListening()
        {
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                    Console.WriteLine("Waiting for a connection...\n\nPlease wait...");  
                while (true)
                {
                    Socket handler = listener.Accept();
                    data = null;

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received:\n {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine("Connection terminated");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static List<AccessLogData> DeserializeData(string dataClient)
        {
            var dataJson = dataClient.Split("<EOF>")
                                        .Where(x => x != "<EOF>")
                                        .FirstOrDefault();

            var listLog = JsonConvert.DeserializeObject<List<AccessLogData>>(dataJson);

            return listLog;
        }
    }
}
