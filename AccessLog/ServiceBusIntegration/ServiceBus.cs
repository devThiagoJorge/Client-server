using Azure.Messaging.ServiceBus;
using ServiceBusIntegration.Classes;
using System;
using System.Net;
using System.Net.Mail;
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
                // create a processor that we can use to process the messages
                ServiceBusProcessor processor = client.CreateProcessor(ServiceBusConfigurations.QueueName, new ServiceBusProcessorOptions());

                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;


                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait...");


                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
        }

        public static void SendEmail(string message)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("thiagojorge.fatec@gmail.com", "gator147");
            MailMessage mail = new MailMessage();
            //mail.Sender = new MailAddress("email que vai enviar", "ENVIADOR");
            mail.From = new MailAddress("thiagojorge.fatec@gmail.com", "ENVIADOR");
            mail.To.Add(new MailAddress("devthiagojorge@gmail.com", "RECEBEDOR"));
            //mail.Subject = "Contato";
            mail.Body = " <br/> Mensagem : " + message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                client.Send(mail);
            }
            catch (Exception erro)
            {
                Console.WriteLine(erro);//trata erro
            }
            finally
            {
                mail = null;
            }
        }
    }
}
