using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Client.Wrapper.Test
{
    public class RpublisherSuccess : RabbitPublisher<TestMessage>
    {
        public RpublisherSuccess(RabbitPublisherConfiguration config) : base(config) { }
    }
}
