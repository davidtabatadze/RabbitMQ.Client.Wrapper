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
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Trace)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("hello");
            var message = new Model
            {
                Id = 99,
                Name = "datiko",
                Date = new DateTime(1988, 11, 11),
                Throw = false
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
                    Date = DateTime.Now,
                    Throw = true
                }
            };
            var no = "!!!No!!!";
            var ok = "OK";
            var validations = false;

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (validations)
            {
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
                Console.Write("consumer - no host - ");
                try
                {
                    using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Name = "simple-queue" }, loggerFactory))
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
                Console.Write("consumer - no name - ");
                try
                {
                    using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" } }, loggerFactory))
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

                // no dependencies
                Console.Write("consumer - no dependencies - name -");
                try
                {
                    using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Type = "type" } } }, loggerFactory))
                    {
                    }
                    Console.Write(no);
                }
                catch (Exception e)
                {
                    Console.Write(ok);
                    Console.WriteLine(" (" + e.Message + ")");
                }
                Console.Write("consumer - no dependencies - type -");
                try
                {
                    using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Name = "name" } } }, loggerFactory))
                    {
                    }
                    Console.Write(no);
                }
                catch (Exception e)
                {
                    Console.Write(ok);
                    Console.WriteLine(" (" + e.Message + ")");
                }
                Console.Write("consumer - no dependencies - type -");
                try
                {
                    using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Name = "name", Type = "type" } } }, loggerFactory))
                    {
                    }
                    Console.Write(no);
                }
                catch (Exception e)
                {
                    Console.Write(ok);
                    Console.WriteLine(" (" + e.Message + ")");
                }
                Console.WriteLine("Validations OK");
            }

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue" }))
            //{
            //    await publisher.Publish(messages);
            //}

            //var con = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue", RetryIntervals = new List<ulong> { 10000, 20000 } }, loggerFactory, new ModelResultPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue-results" }));

            //using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-fanout", Type = "fanout", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Route = "some-route", Name = "simple-fanout-queue" } } }))
            //{
            //    await publisher.Publish(message);
            //    await publisher.Publish(message, "other-route");
            //}

            //using (var consumer = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue-withex", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Name = "auto-exchange", Type = "fanout" } } }, loggerFactory))
            //{

            //}

            //using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-direct", Type = "direct", Dependencies = new List<RabbitConfigurationDependency> { new RabbitConfigurationDependency { Route = "direct-1", Name = "simple-direct-queue1" }, new RabbitConfigurationDependency { Route = "direct-2", Name = "simple-direct-queue2" } } }))
            //{
            //    await publisher.Publish(message, "direct-1");
            //    await publisher.Publish(message, "direct-2");
            //    await publisher.Publish(message);
            //}


            //using (var publisher = new ModelPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "test-queue" }))
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        await publisher.Publish(messages9);
            //    }
            //    await publisher.Publish(message);
            //}
            //System.Threading.Thread.Sleep(10000);
            //var con = new ModelConsumer(new RabbitConsumerConfiguration { Hosts = new List<string> { "localhost" }, Name = "test-queue", BatchSize = 3, RetryIntervals = new List<ulong> { 10000 } }, loggerFactory, new ModelResultPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue-results" }));

            Console.WriteLine();
            Console.WriteLine("all done");
            Console.ReadKey();
        }

    }
}
