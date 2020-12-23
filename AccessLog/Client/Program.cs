using Client.Classes;
using ServiceBusIntegration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        
        public static Stopwatch TimeRead = new Stopwatch();
        public static Stopwatch ClientTime = new Stopwatch();
        public static Stopwatch TimeParse = new Stopwatch();
        public static Stopwatch TimeSendToServer = new Stopwatch();
       
        static async Task Main(string[] args)
        {
            ClientTime.Start();
            StartClient();
            ClientTime.Stop();
            Console.WriteLine($"\n\nFinished time: {ClientTime.Elapsed}");

            await SendServiceBus($"Reading Access Log: {TimeRead.Elapsed}");
            await SendServiceBus($"Parse time: {TimeParse.Elapsed}");
            await SendServiceBus($"Time send to server: {TimeSendToServer.Elapsed}");
            await SendServiceBus($"Time to finish: {ClientTime.Elapsed}");
        }

        public static async Task SendServiceBus(string message)
        {
            await ServiceBus.SendMessageAsync(message);
        }
        public static string ReadAccessLogAndStructure(string path)
        {
            TimeRead.Start();
            string[] readLog = System.IO.File.ReadAllLines(path);
            TimeRead.Stop();
            var listLog = StructureAccessLog(readLog);

            string jsonString = JsonSerializer.Serialize(listLog);
            return jsonString;
        }

        public static List<AccessLogData> StructureAccessLog(string[] log)
        {
            TimeParse.Start();
            string[] dataSplit = { };
            var dataLog = new List<AccessLogData>();
            foreach (string line in log)
            {
                dataSplit = line.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x)).ToArray();

                dataLog.Add(new AccessLogData(
                    dataSplit[0],
                    long.Parse(dataSplit[1]),
                    dataSplit[2],
                    dataSplit[3],
                    int.Parse(dataSplit[4]),
                    dataSplit[5],
                    dataSplit[6],
                    dataSplit[7],
                    dataSplit[8],
                    dataSplit[9]
                    ));
            }
            TimeParse.Stop();
            return dataLog;
        }

        public static void StartClient()
        {
            Console.WriteLine("Starting client...");
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    string dataSent = ReadAccessLogAndStructure(ApplicationConstants.PathLog);

                    TimeSendToServer.Start();

                    byte[] msg = Encoding.ASCII.GetBytes($"{dataSent}<EOF>");

                    Console.WriteLine("Sending data...");
                    int bytesSent = sender.Send(msg);

                    int bytesRec = sender.Receive(bytes);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    TimeSendToServer.Stop();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
