using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMq.Client.Wrapper;

namespace RabbitMq.Client.Wrapper.Test
{
    public class Rpublisher : RabbitPublisher<TestMessage>
    {

        public Rpublisher(RabbitPublisherConfiguration configuration) : base(configuration)
        {
            var x = 1;
        }

    }
}
