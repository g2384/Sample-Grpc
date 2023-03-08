namespace ServerNode
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using MassTransit;

    public class SubmitClaimConsumer : IConsumer<SubmitClaim>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public SubmitClaimConsumer(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<SubmitClaim> context)
        {
            LogContext.Info?.Log("Consuming (WorkerNode): {Index}/{Count} {ClaimId} {SourceAddress}", context.Message.Index, context.Message.Count,
                context.Message.Content, context.SourceAddress);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:response-node"));

            await endpoint.Send(new ClaimSubmitted { Content = "response: " + context.Message.Content + "|" + context.Message.Index });
        }
    }

    public class SubmitClaimJobConsumer : IConsumer<SubmitJobClaim>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public SubmitClaimJobConsumer(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<SubmitJobClaim> context)
        {
            LogContext.Info?.Log("Consuming (JobNode): {Index}/{Count} {ClaimId} {SourceAddress}", context.Message.Index, context.Message.Count,
                context.Message.Content, context.SourceAddress);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:response-node"));

            await endpoint.Send(new ClaimSubmitted { Content = "response: " + context.Message.Content + "|" + context.Message.Index });
        }
    }
}