namespace ClientNode
{
    using System.Threading.Tasks;
    using Common;
    using MassTransit;
    using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Response received: {content} {index}/{count} {ResponseAddress} - {SourceAddress}", message.Content, message.Index, message.Count, context.ResponseAddress, context.SourceAddress);

            return Task.CompletedTask;
        }
    }
}