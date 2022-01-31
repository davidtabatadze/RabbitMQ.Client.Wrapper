using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace RabbitMQ.Client.Wrapper.Test
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("hello");
            var message = new Model
            {
                Id = 99,
                Name = "datiko",
                Date = new DateTime(1988, 11, 11),
                Throw = true
            };
            var messages = new List<Model>{
                new Model {
                    Id = 1,
                    Name = "name1",
                    Date = DateTime.Now,
                    Throw = false
                },
                    new Model {
                    Id = 2,
                    Name = "name2",
                    Date = DateTime.Now,
                    Throw = true
                }
            };
            var messages9 = new List<Model>{
                new Model {
                    Id = 1,
                    Name = "name1",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 2,
                    Name = "name2",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 3,
                    Name = "name3",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 4,
                    Name = "name4",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 5,
                    Name = "name5",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 6,
                    Name = "name6",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 7,
                    Name = "name7",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 8,
                    Name = "name8",
                    Date = DateTime.Now
                },
                new Model {
                    Id = 9,
                    Name = "name9",
                    Date = DateTime.Now
                }
            };
            var no = "!!!No!!!";
            var ok = "OK";

            var consumer = new ModelConsumer(new RabbitConsumerConfiguration
            {
                Hosts = new List<string> { "localhost" },
                Name = "simple-queue",
                //RetryIntervals = new List<ulong> { 30000, 60000 },
                BatchSize = 100
            }, loggerFactory);

            #region Publisher

            // no host
            Console.Write("publisher - no host - ");
            try
            {
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Name = "simple-queue" }, loggerFactory))
                {
                }
                Console.Write(no);
            }
            catch (Exception e)
            {
                Console.Write(ok);
                Console.WriteLine(" (" + e.Message + ")");
            }

            // no name
            Console.Write("publisher - no name - ");
            try
            {
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" } }, loggerFactory))
                {
                }
                Console.Write(no);
            }
            catch (Exception e)
            {
                Console.Write(ok);
                Console.WriteLine(" (" + e.Message + ")");
            }

            // no route
            Console.Write("publisher - no route - ");
            try
            {
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", Routings = new List<string> { "  " } }, loggerFactory))
                {
                }
                Console.Write(no);
            }
            catch (Exception e)
            {
                Console.Write(ok);
                Console.WriteLine(" (" + e.Message + ")");
            }

            // no exchange
            Console.Write("publisher - no exchange - ");
            try
            {
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", Type = "dodola" }, loggerFactory))
                {
                }
                Console.Write(no);
            }
            catch (Exception e)
            {
                Console.Write(ok);
                Console.WriteLine(" (" + e.Message + ")");
            }

            // simple exchange
            Console.WriteLine("\nsimple exchange");
            using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-direct", Type = "direct" }, loggerFactory))
            {
                await publisher.Publish(message);
            }

            // simple queue
            Console.WriteLine("\nsimple queue");
            using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }, loggerFactory))
            {
                //await publisher.Publish(message);
                //await publisher.Publish(message);
                await publisher.Publish(messages);
                //await publisher.Publish(messages9);
            }



            using (var publisher = new RabbitPublisher<string>(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }))
            {
                await publisher.Publish("dodolaaa");
            }
            Console.ReadKey(); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // simple queue no logs
            Console.WriteLine("\nsimple queue no logs");
            using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }))
            {
                await publisher.Publish(message);
                Console.WriteLine("----- no logs");
            }

            // simple queue override logs
            Console.WriteLine("\nsimple queue override logs");
            using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }, loggerFactory, true))
            {
                await publisher.Publish(message);
            }

            // simple queue override logs
            Console.WriteLine("\nsimple queue override logs");
            using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }, loggerFactory, true, true))
            {
                await publisher.Publish(message);
                Console.WriteLine("----- hidden logs");
            }

            // simple exchange false route
            Console.WriteLine("\nsimple exchange false route");
            try
            {
                // simple exchange false route
                Console.WriteLine("\nsimple exchange false route");
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-exchange", Type = "direct", Routings = new List<string> { "r1", "r2" } }, loggerFactory))
                {
                    await publisher.Publish(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-exchange", Type = "direct", Routings = new List<string> { "r1", "r2" } }, loggerFactory))
                {
                    await publisher.Publish(messages, "some route");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            #endregion

            Console.WriteLine();
            Console.WriteLine("all done");
            Console.ReadKey();
        }

    }
}
