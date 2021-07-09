using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper.Test
{
    public class Consumer1 : RabbitConsumer<string>
    {

        public Consumer1(RabbitConfiguration configuration) : base(configuration)
        {
            var x = 1;
        }

        public override async Task Handle(string consumer, string message)
        {
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine(consumer + " handled " + message);
        }

    }
}
