using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMq.Client.Wrapper;

namespace RabbitMq.Client.Wrapper.Test
{
    public class Publisher1 : RabbitPublisher<string>
    {

        public Publisher1(RabbitPublisherConfiguration configuration) : base(configuration)
        {
            var x = 1;
        }

        //public override void OnStart()
        //{
        //    Console.WriteLine("starting rabbit");
        //}

        //public override void OnPublish(string message)
        //{
        //    Console.WriteLine("publishing '" + message + "'");
        //}

        //public override void OnException(Exception exception, string message)
        //{
        //    Console.WriteLine("exception: " + exception.Message);
        //}

        //public override async Task Publish(string message)
        //{
        //    //message += " (+)";
        //    await base.Publish(message);
        //}

    }
}
