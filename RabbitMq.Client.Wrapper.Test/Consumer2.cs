using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper.Test
{
    public class Consumer2 : RabbitConsumer<string>
    {

        public Consumer2(RabbitConfiguration configuration) : base(configuration)
        {
            var x = 1;
        }

        public override async Task Handle(string a, string message)
        {
            //System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Consumer2 handled " + message);
        }

    }
}
