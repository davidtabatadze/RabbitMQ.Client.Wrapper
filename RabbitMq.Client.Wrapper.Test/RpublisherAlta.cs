using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Client.Wrapper.Test
{
    public class RpublisherAlta : RabbitPublisher<string>
    {
        public RpublisherAlta(RabbitPublisherConfiguration configuration) : base(configuration) { }
    }
}
