using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Util;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                cfg.Host(new Uri("rabbitmq://localhost/test"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                }));

            try
            {
                TaskUtil.Await(() => bus.StartAsync());

                var random = new Random();
                while (true)
                {
                    try
                    {
                        Console.Write("Enter batches count (quit exits): ");
                        var line = Console.ReadLine();
                        if (line == "quit")
                        {
                            break;
                        }

                        var batch = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        
                        var tasks = batch.Select(id => bus.Publish<AddPaymentMessage>(new { Id = id }));

                        await Task.WhenAll(tasks);

                        Console.WriteLine($"Batch published: ", string.Join(", ", batch));
                    }
                    catch
                    {
                        Console.WriteLine("Failed to parse batch data!");
                    }
                }
            }
            finally
            {
                TaskUtil.Await(() => bus.StopAsync());
            }
        }
    }
}