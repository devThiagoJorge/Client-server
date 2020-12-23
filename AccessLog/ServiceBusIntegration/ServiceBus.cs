using Azure.Messaging.ServiceBus;
using ServiceBusIntegration.Classes;
using System;
using System.Threading.Tasks;

namespace ServiceBusIntegration
{
    public class ServiceBus
    {
        public static async Task SendMessageAsync(string messageSend)
        {
            // create a Service Bus client 
            await using (ServiceBusClient client = new ServiceBusClient(ServiceBusConfigurations.ConnectionString))
            {
                ServiceBusSender sender = client.CreateSender(ServiceBusConfigurations.QueueName);

                ServiceBusMessage message = new ServiceBusMessage(messageSend);

                await sender.SendMessageAsync(message);
                Console.WriteLine($"Sent a single message to the queue: {ServiceBusConfigurations.QueueName}");
            }
        }

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public static async Task ReceiveMessagesAsync()
        {
            await using (ServiceBusClient client = new ServiceBusClient(ServiceBusConfigurations.ConnectionString))
            {
                ServiceBusProcessor processor = client.CreateProcessor(ServiceBusConfigurations.QueueName, new ServiceBusProcessorOptions());

                processor.ProcessMessageAsync += MessageHandler;

                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
        }
    }
}
