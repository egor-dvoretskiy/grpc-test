using Grpc.Net.Client;
using System.Net.Mail;
using Mail;
using Grpc.Core;

namespace MailerClient
{ 
    public class Program
    {
        static public async Task Main(String[] args)
        {
            Console.WriteLine("Client. BD streaming RPC");

            var mailboxName = "BDStream";

            Console.WriteLine($"Creating client to mailbox '{mailboxName}'");
            Console.WriteLine();

            var channel = GrpcChannel.ForAddress("https://localhost:7042");

            channel.ConnectAsync().Wait();

            var client = new Mailer.MailerClient(channel);

            Console.WriteLine("Client created");
            Console.WriteLine("Press escape to disconnect. Press any other key to forward mail.");

            using (var call = client.Mailbox(headers: new Grpc.Core.Metadata { new Grpc.Core.Metadata.Entry("mailbox-name", mailboxName)}))
            {
                var responseTask = Task.Run(async () =>
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        Console.ForegroundColor = message.Reason == MailboxMessage.Types.Reason.Received ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine(message.Reason == MailboxMessage.Types.Reason.Received ? "Mail received" : "Mail forwarded");
                        Console.WriteLine($"New mail: {message.New}, Forwarded mail: {message.Forwarded}");
                        Console.ResetColor();
                    }
                });

                while (true)
                {
                    var result = Console.ReadKey(intercept: true);
                    if (result.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    await call.RequestStream.WriteAsync(new ForwardMailMessage());

                    Console.WriteLine("Disconnecting");
                    await call.RequestStream.CompleteAsync();
                    await responseTask;
                }

                Console.WriteLine("Disconnected. Press any key to exit.");
                Console.ReadKey();

            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}