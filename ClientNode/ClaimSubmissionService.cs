namespace ClientNode
{
    using System;
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

            const int total = 10;

            var sender = await _bus.GetSendEndpoint(new Uri("queue:worker-node"));

            for (var i = 0; i < total; i++)
            {
                await Task.Delay(200, stoppingToken);

                await sender.Send(new SubmitClaim
                {
                    Content = NewId.NextGuid().ToString() + "WorkNode",
                    Index = i + 1,
                    Count = total
                }, new CancellationToken());
            }
            return;
        }
    }
}