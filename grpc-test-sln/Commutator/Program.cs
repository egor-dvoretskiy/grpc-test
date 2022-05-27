using System.Threading.Tasks;
using Grpc.Net.Client;
using Protos.GrpcCommutatorClient;

namespace CommutatorClient
{
    public class Program
    {
        static public async Task Main(String[] args)
        {
            Console.WriteLine("Client. Simple RPC");
            using GrpcChannel? channel = GrpcChannel.ForAddress("https://localhost:7042");

            while (channel.State != Grpc.Core.ConnectivityState.Ready)
            {
                channel.ConnectAsync().Wait();
            }

            var client = new Commutator.CommutatorClient(channel);
            var reply = await client.GetDataAsync(new ConnectionRequest() { MessageRequest = "GIVE ME THE DATA" });

            var data = reply.Data.ToArray();

            if (data != null)
            {
                Console.WriteLine($"Data received: {System.Text.Encoding.UTF8.GetString(data)}");
            }
            else
            {
                Console.WriteLine("No data.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
