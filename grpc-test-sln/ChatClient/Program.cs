using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Protos.Chat;

namespace CommutatorClient
{
    public class Program
    {
        static public async Task Main(String[] args)
        {
            Console.WriteLine("Client. BD streaming RPC");

            var channel = GrpcChannel.ForAddress("https://localhost:7042");
            channel.ConnectAsync().Wait();

            var client = new Chat.ChatClient(channel);

            using (var call = client.SendMessage())
            {
                var responseTask = Task.Run(async () =>
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        if (message == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("No data");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(message.Text);
                            Console.ResetColor();
                        }
                    }
                });

                while (true)
                {
                    Console.Write("Enter message to send: ");
                    string? value = Console.ReadLine();

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    if (value == "exit")
                    {
                        break;
                    }

                    await call.RequestStream.WriteAsync(new ClientToServerMessage()
                    {
                        Text = value
                    });
                }

                await call.RequestStream.CompleteAsync();

                await responseTask;
                Console.WriteLine("You have left the loop.");
            }            

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
