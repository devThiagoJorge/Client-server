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
            var listLog = DeserializeData(dataReceive);
            DatabaseOperations.ConnectionDatabase(DatabaseOperations.ConnectionString, listLog);
            stopwatch.Stop();
            Console.WriteLine($"\t\n\nTempo passado: {stopwatch.Elapsed}");

            Console.WriteLine("Press any key for the close console...");
            Console.ReadKey();
            
        }

        public static string dataReceive = null;

        public static void StartListening()
        {
            byte[] bytes = new Byte[16384];

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

                    int bytesRec = 0;
                    StringBuilder dataJson = new StringBuilder();
                    while (true)
                    {
                        bytesRec = handler.Receive(bytes);
                        dataReceive = null;
                        dataReceive = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        dataJson.Append(dataReceive);
                        if (dataReceive.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    dataReceive = dataJson.ToString();
                    //Console.WriteLine("Text received:\n {0}", dataJson);
                    byte[] msg = Encoding.ASCII.GetBytes(dataReceive);

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
