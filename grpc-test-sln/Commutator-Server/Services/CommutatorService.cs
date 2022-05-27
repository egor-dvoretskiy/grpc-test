using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Protos.GrpcCommutatorServer;

namespace CommutatorServer.Services
{
    public class CommutatorService : Commutator.CommutatorBase
    {
        private readonly ILogger _logger;

        List<int> orderedList = Enumerable.Range(3, 66).ToList();

        public CommutatorService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CommutatorService>();
        }

        public override Task<ConnectionReply> GetData(ConnectionRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received from client: {request.MessageRequest}");

            byte[] bytesReply = orderedList
                .SelectMany(BitConverter.GetBytes)
                .ToArray();

            _logger.LogInformation($"bytes to reply: {System.Text.Encoding.UTF8.GetString(bytesReply)}");

            return Task.FromResult(new ConnectionReply() { MessageReply = "Here yours data, mfck.", Data = ByteString.CopyFrom(bytesReply) });
        }
    }
}
