using System;
using System.Collections.Generic;
using System.Text;
using static RabbitMq.Client.Wrapper.Test.Program;

namespace RabbitMq.Client.Wrapper.Test
{
    internal class PublisherAlta : RabbitPublisher<Response>
    {
        public PublisherAlta(RabbitPublisherConfiguration configuration) : base(configuration)
        {
        }
    }
}
