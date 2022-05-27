using Grpc.Core;
using Grpc.Net.Client;
using Mult;
using System.Threading.Tasks;

namespace MultiplierClient
{
    public class Program
    {
        static public async Task Main(String[] args)
        {
            Console.WriteLine("Client. Server-side streaming RPC");

            using var channel = GrpcChannel.ForAddress("https://localhost:7042");

            channel.ConnectAsync().Wait();

            var client = new Multiplier.MultiplierClient(channel);

            using var call = client.Multiply(new Google.Protobuf.WellKnownTypes.Empty());

            long res = 1;

            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                if (message == null)
                {
                    Console.WriteLine("No data");
                }
                else {
                    Console.Write(message.Res + " ");

                    res *= message.Res;
                }
            }

            Console.WriteLine($"{Environment.NewLine}Res: {res}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}