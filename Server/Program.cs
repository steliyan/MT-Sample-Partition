﻿using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.RedisIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Server.Consumers;
using Server.StateMachines;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<ProcessPaymentConsumer>();
                        x.AddSagaStateMachine<AddPaymentStateMachine, AddPaymentState>(typeof(AddPaymentSagaDefinition))
                            .RedisRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                                r.DatabaseConfiguration("127.0.0.1");
                            });

                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            cfg.Host(new Uri("rabbitmq://localhost/test"), h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(provider);
                        }));
                    });

                    services.AddSingleton<IHostedService, Service>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog(dispose: true);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                });

            await builder.RunConsoleAsync();
        }
    }
}