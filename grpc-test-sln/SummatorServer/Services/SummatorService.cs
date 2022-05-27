using Google.Protobuf.Collections;
using Grpc.Core;
using Protos.GrpcSummator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummatorServer.Services
{
    public class SummatorService : Summator.SummatorBase
    {
        private readonly ILogger _logger;

        public SummatorService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SummatorService>();
        }

        public override async Task<SummReply> Summarize(IAsyncStreamReader<SummRequest> requestStream, ServerCallContext context)
        {
            var reply = new SummReply();

            int sum = 0;
            List<int> repeated = new List<int>();        

            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Incrementing count by {message.Count}");

                sum += message.Count;
                repeated.Add(message.Count);
            }

            reply.Summ = sum;
            reply.Nums.Add(repeated);

            return reply;
        }
    }
}
