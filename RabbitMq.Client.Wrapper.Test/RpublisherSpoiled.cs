using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Client.Wrapper.Test
{
    public class RpublisherSpoiled : RabbitPublisher<string>
    {
        public RpublisherSpoiled(RabbitPublisherConfiguration configuration) : base(configuration)
        {
            var x = 1;
        }
    }
}
