namespace GrpcNode2
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;
    using Microsoft.Extensions.Logging;

    public class SubmitClaimConsumer : IConsumer<SubmitClaim>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly ISendEndpointProvider _sendEndpointProvider;

        public SubmitClaimConsumer(IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider)
        {
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<SubmitClaim> context)
        {
            LogContext.Info?.Log("Consuming (GrpcNode2): {Index}/{Count} {ClaimId} {SourceAddress}", context.Message.Index, context.Message.Count,
                context.Message.Content, context.SourceAddress);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:response-node"));

            await endpoint.Send(new ClaimSubmitted { Content = "response: " + context.Message.Content + "|" + context.Message.Index });
        }
    }

    public class ResponseConsumer : IConsumer<ClaimSubmitted>
    {
        readonly ILogger<ClaimSubmitted> _logger;

        public ResponseConsumer(ILogger<ClaimSubmitted> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ClaimSubmitted> context)
        {
            var message = context.Message;
            _logger.LogInformation("Claim Submitted (WorkerNode): {content} {ResponseAddress} - {SourceAddress}", message.Content, context.ResponseAddress, context.SourceAddress);

            return Task.CompletedTask;
        }
    }
}