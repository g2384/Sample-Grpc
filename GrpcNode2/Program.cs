namespace GrpcNode2
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
                        //x.AddConsumer<SubmitClaimConsumer>();
                        x.AddConsumer<ResponseConsumer>();

                        // server
                        x.UsingGrpc((context, cfg) =>
                        {
                            cfg.Host(h =>
                            {
                                h.Host = "localhost";

                                h.Port = 19796;

                                // foreach (var host in new[] { new Uri("http://127.0.0.1:19796/") })
                                //     h.AddServer(host);
                            });

                            //                            if (string.IsNullOrWhiteSpace(options.Value.Servers))
                            //cfg.ConfigureEndpoints(context, filter => filter.Include<SubmitClaimConsumer>());
                            cfg.ReceiveEndpoint("response-node",
                                e =>
                                {
                                    //SetResiliencyRules(e, configuration);
                                    e.ConfigureConsumer<ResponseConsumer>(context, config => { });
                                });
                        });
                    });

                    //services.AddMassTransit<ISecondBus>(x =>
                    //{
                    //    // server
                    //    x.UsingGrpc((context, cfg) =>
                    //    {
                    //        cfg.Host(h =>
                    //        {
                    //            h.Host = "localhost";

                    //            h.Port = 19796 + 10000;

                    //            h.AddServer(new Uri("http://127.0.0.1:19796"));
                    //        });
                    //    });
                    //});

                    //services.AddHostedService<ClaimSubmissionService>();

                    services.AddOptions<StartupOptions>()
                        .Configure<IConfiguration>((options, config) =>
                        {
                            config.Bind(options);
                        });
                });
        }
    }


    public interface ISecondBus : IBus
    { }
}