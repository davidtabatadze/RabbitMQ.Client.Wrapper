using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper.Test
{
    public class Rconsumer : RabbitConsumer<TestMessage>
    {

        public Rconsumer(RabbitConsumerConfiguration configuration, RpublisherSuccess success = null) : base(configuration)
        {
            Success = success;
        }

        private RpublisherSuccess Success { get; set; }

        public override async Task Handle(TestMessage message, string consumer)
        {
            await Task.Run(async () =>
            {
                //System.Threading.Thread.Sleep(new Random().Next(1000, 4000));
                if (message.Id > 0 && message.Id <= 110)
                {
                    throw new Exception("aloo");
                }
                await Success.Publish(message);
                Console.WriteLine("\nhandled " + consumer);
                Console.WriteLine(message.Name);
            });
        }

        public override async Task Handle(IEnumerable<TestMessage> messages, string consumer)
        {
            await Task.Run(async () =>
            {
                System.Threading.Thread.Sleep(new Random().Next(0, 1000));
                if (messages.Any(message => message.Id > 100 && message.Id < 150))
                {
                    throw new Exception("aloo");
                }
                await Success.Publish(messages);
                Console.WriteLine("\nhandled " + consumer + " (" + messages.Count() + ")");
                //Console.WriteLine(string.Join(" ", messages.Select(m => m.Name)));
            });
        }

    }
}
