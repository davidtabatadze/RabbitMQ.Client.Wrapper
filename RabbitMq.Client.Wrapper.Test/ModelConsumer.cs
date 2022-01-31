using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Wrapper.Test
{
    public class ModelConsumer : RabbitConsumer<Model>
    {
        public ModelConsumer(RabbitConsumerConfiguration configuration, ILoggerFactory logger)
            : base(configuration, logger.CreateLogger(typeof(ModelConsumer))) { }
        protected override async Task Handle(Model message)
        {
            if (message.Throw)
            {
                throw new System.Exception("something went wrong...");
            }
            using (var pub = new ModelResultPublisher(new RabbitPublisherConfiguration { Hosts = new List<string> { "localhost" }, Name = "simple-queue-results" }))
            {
                await pub.Publish(message);
            }
        }
    }
}
