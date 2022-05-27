using System.Threading.Tasks;
using Grpc.Net.Client;
using Protos.GrpcSummatorClient;

namespace SummatorClient
{
    public class Program
    {
        static public async Task Main(String[] args)
        {
            Console.WriteLine("Client. Client-side streaming RPC");
            using GrpcChannel? channel = GrpcChannel.ForAddress("https://localhost:7042");

            while (channel.State != Grpc.Core.ConnectivityState.Ready)
            {
                channel.ConnectAsync().Wait();
            }

            var client = new Summator.SummatorClient(channel);

            using var call = client.Summarize();

            Random random = new Random();

            int num = 30;
            var amount = random.Next(num);
            Console.WriteLine($"Amount of numbers: {amount}");

            for (int i = 0; i < amount; i++)
            {
                var number = random.Next(num);
                Console.Write(number + " ");
                await call.RequestStream.WriteAsync(new SummRequest() { Count = number });
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            await call.RequestStream.CompleteAsync();

            var response = await call;
            Console.WriteLine($"{Environment.NewLine}Summ: {response.Summ}");
            Console.WriteLine($"{string.Join(" ", response.Nums)}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}