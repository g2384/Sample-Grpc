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

            await endpoint.Send(new ClaimSubmitted {
                Content = "response: " + context.Message.Content,
                Count = context.Message.Count,
                Index = context.Message.Index,
                ClaimId = context.Message.ClaimId
            });
        }
    }

    public class SubmitClaimJobConsumer : IJobConsumer<SubmitJobClaim>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public SubmitClaimJobConsumer(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Run(JobContext<SubmitJobClaim> context)
        {
            LogContext.Info?.Log("Consuming (JobNode): {Index}/{Count} {ClaimId} {SourceAddress}", context.Job.Index, context.Job.Count,
                context.Job.Content, context.SourceAddress);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:response-node"));

            await endpoint.Send(new ClaimSubmitted
            {
                Content = "response: " + context.Job.Content,
                Count = context.Job.Count,
                Index = context.Job.Index,
                ClaimId = context.Job.ClaimId
            });
        }
    }
}