using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper.Test
{
    public class RconsumerDead : RabbitConsumer<RabbitDeadMessage>
    {
        public RconsumerDead(RabbitConsumerConfiguration configuration) : base(configuration)
        {
            Publisher = new RabbitPublisher<TestMessage>(new RabbitPublisherConfiguration
            {
                Hosts = new List<string> { "localhost" },
                Name = "rabbit-queue"
            });
        }

        public RabbitPublisher<TestMessage> Publisher { get; set; }

        public override async Task Handle(RabbitDeadMessage message, string consumer)
        {
            Console.WriteLine("\nhandled " + consumer);
            Console.WriteLine(message.Message);

            var msg = JsonSerializer.Deserialize<TestMessage>(message.Message);

            await Publisher.Publish(msg);
        }

    }
}
