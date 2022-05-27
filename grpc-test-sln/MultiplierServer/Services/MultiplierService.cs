using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mult;

namespace MultiplierServer.Services
{
    public class MultiplierService : Multiplier.MultiplierBase
    {
        private readonly ILogger _logger;

        public MultiplierService(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<MultiplierService>();
        }

        public override async Task Multiply(Empty request, IServerStreamWriter<MultiplyReply> responseStream, ServerCallContext context)
        {
            Random random = new Random();
            int amount = random.Next(6, 24);
            Console.WriteLine($"Numbers amount: {amount}");

            long mult = 1;

            for (int i = 0; i < amount; i++)
            {
                int num = random.Next(1, 14);

                mult *= num;

                Console.Write(num + " ");

                await responseStream.WriteAsync(new MultiplyReply { Res = num });
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            Console.WriteLine($"Mult: {mult}");
        }
    }
}
