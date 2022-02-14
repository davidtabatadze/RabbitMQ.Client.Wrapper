using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Wrapper.Test
{
    public class ModelConsumer : RabbitConsumer<Model>
    {
        ModelResultPublisher Publisher { get; set; }
        public ModelConsumer(RabbitConsumerConfiguration configuration, ILoggerFactory logger, ModelResultPublisher publisher = null)
            : base(configuration, logger.CreateLogger(typeof(ModelConsumer))) { Publisher = publisher; }
        protected override async Task Handle(Model message)
        {
            await Handle(new List<Model> { message });
        }
        protected override async Task Handle(IEnumerable<Model> messages)
        {
            if (messages.Any(m => m.Throw))
            {
                throw new System.Exception("something wrong...");
            }
            await Publisher.Publish(messages);
        }
    }
}
