namespace ServerNode
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Serilog;


    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    log.MinimumLevel.Information();

                    log.WriteTo.Console();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        //x.AddDelayedMessageScheduler();

                        x.AddConsumer<SubmitClaimConsumer>();

                        //x.AddConsumer<SubmitClaimJobConsumer>()
                        //    .Endpoint(e => e.Name = "job-node");

                        //x.AddSagaRepository<JobSaga>().InMemoryRepository();
                        //x.AddSagaRepository<JobTypeSaga>().InMemoryRepository();
                        //x.AddSagaRepository<JobAttemptSaga>().InMemoryRepository();

                        // clients
                        x.UsingGrpc((context, cfg) =>
                        {
                            cfg.Host(h =>
                            {
                                h.Host = "localhost";

                                h.Port = 19797;

                                foreach (var host in new[] { new Uri("http://127.0.0.1:19796/") })
                                    h.AddServer(host);
                            });

                            //cfg.UseDelayedMessageScheduler();

                            //var options = new ServiceInstanceOptions();

                            //cfg.ServiceInstance(options, instance =>
                            //{
                            //    instance.ConfigureJobServiceEndpoints(js =>
                            //    {
                            //        js.SagaPartitionCount = 1;
                            //        js.FinalizeCompleted = false; // for demo purposes, to get state

                            //        js.ConfigureSagaRepositories(context);
                            //    });

                            //    // configure the job consumer on the job service endpoints
                            //    instance.ConfigureEndpoints(context, f => f.Include<SubmitClaimJobConsumer>());
                            //});

                            cfg.ReceiveEndpoint("worker-node",
                                e =>
                                {
                                    e.ConfigureConsumer<SubmitClaimConsumer>(context, config => { });
                                });
                        });
                    });

                    services.AddOptions<StartupOptions>()
                        .Configure<IConfiguration>((options, config) =>
                        {
                            config.Bind(options);
                        });
                });
        }
    }
}