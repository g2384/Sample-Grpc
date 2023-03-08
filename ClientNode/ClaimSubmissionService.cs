namespace ClientNode
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using MassTransit;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    public class ClaimSubmissionService :
        BackgroundService
    {
        readonly IBusControl _bus;
        readonly ILogger<ClaimSubmissionService> _logger;

        public ClaimSubmissionService(ILogger<ClaimSubmissionService> logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken);

            IRequestClient<SubmitClaim> client = _bus.CreateRequestClient<SubmitClaim>();

            if (false)//_options.Value.Count.HasValue)
            {
                int count = 10;//_options.Value.Count.Value;

                await Task.Delay(200, stoppingToken);

                _logger.LogError("Sending: {0}", count);

                var timer = Stopwatch.StartNew();

                for (var i = 0; i < count / 40; i++)
                {
                    const int total = 10;

                    Response<ClaimSubmitted>[] responses = await Task.WhenAll(Enumerable.Range(0, total).Select(index =>
                        client.GetResponse<ClaimSubmitted>(new SubmitClaim
                        {
                            Content = NewId.NextGuid().ToString() + "from GrpcNode2",
                            Index = index + 1,
                            Count = total
                        }, stoppingToken))).ConfigureAwait(false);
                }

                timer.Stop();

                _logger.LogError("Send count: {0}", count);
                _logger.LogError("Total send duration: {0:g}", timer.Elapsed);
                _logger.LogError("Request rate: {0:F2} (req/s)", count * 1000 / timer.Elapsed.TotalMilliseconds);
            }
            else
            {
                const int total = 10;

                for (var i = 0; i < total; i++)
                {
                    await Task.Delay(200, stoppingToken);

                    Response<ClaimSubmitted> response = await client.GetResponse<ClaimSubmitted>(new SubmitClaim
                    {
                        Content = NewId.NextGuid().ToString() + "from GrpcNode2",
                        Index = i + 1,
                        Count = total
                    }, stoppingToken);

                    _logger.LogInformation("Claim Submitted (GrpcNode2): {Index}/{Count} {ClaimId} - {SourceAddress}", i + 1, total,
                        response.Message.Content, response.SourceAddress);
                }

                return;
                await Task.Delay(10000, stoppingToken);

                for (var i = 0; i < total; i++)
                {
                    await Task.Delay(200, stoppingToken);

                    Response<ClaimSubmitted> response = await client.GetResponse<ClaimSubmitted>(new SubmitClaim
                    {
                        Content = NewId.NextGuid().ToString() + "from GrpcNode2",
                        Index = i + 1,
                        Count = total
                    }, stoppingToken);

                    _logger.LogInformation("Claim Submitted (GrpcNode2): {Index}/{Count} {ClaimId} - {SourceAddress}", i + 1, total,
                        response.Message.Content, response.SourceAddress);
                }
            }
        }
    }
}