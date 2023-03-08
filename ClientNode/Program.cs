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
                        x.AddConsumer<ResponseConsumer>();

                        // server
                        x.UsingGrpc((context, cfg) =>
                        {
                            cfg.Host(h =>
                            {
                                h.Host = "localhost";

                                h.Port = 19796;
                            });

                            cfg.ReceiveEndpoint("response-node",
                                e =>
                                {
                                    e.ConfigureConsumer<ResponseConsumer>(context, config => { });
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