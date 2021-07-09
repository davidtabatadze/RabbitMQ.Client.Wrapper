using CoreKit.Extension.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper
{


    public class RabbitConsumer<T> : RabbitBase<T>
    {



        public RabbitConsumer(RabbitConfiguration configuration, ILogger logger = null) : base(configuration, logger)
        {
            ssss();

            //for (int i = 0; i < length; i++)
            //{

            //}
        }



        private async Task ssss()
        {
            for (int i = 0; i < Channels.Count; i++)
            {
                // 
                Thread.Sleep(1000);
                // რიგის გამომყენებლის მიმთითებლის შექმნა
                var channel = Channels[i];
                var consumer = new RabbitConsumerBase<T>(channel);
                var consumerName = string.Format("{0}-{1}-{2}", "consumer", Configuration.GetName(), i + 1);
                OnStart(consumerName);

                // Action for recieve messages
                consumer.Received += (sender, args) =>
                {
                    // Defining the message body
                    var body = Encoding.UTF8.GetString(args.Body.ToArray());
                    try
                    {
                        // Convert to actual typed message
                        var message = JsonConvert.DeserializeObject<T>(body);


                        Console.WriteLine("===DONE=== " + message);
                        channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);


                        //channel.BasicReject(args.DeliveryTag, true);

                        // Preserve the message, make it ready for handling
                        //consumer.Preserve(args.DeliveryTag, message);
                    }
                    catch (Exception e)
                    {
                        var ec = 0;
                    }
                };
                // Action for handle messages
                consumer.Handle += (tag, messages) =>
                {
                    Console.WriteLine("davai dahendle " + messages.Count);
                };


                // გამომყენებელი აღარ მიიღებს ზედმეტ შეტყობინებებს, სანამ არსებულებს არ დაამუშავებს
                channel.BasicQos(0, Configuration.BatchSize, false);
                // რიგის გამომყენებლის აწყობა-დაკონფიგურირება
                channel.BasicConsume(
                    queue: Configuration.Queue,
                    // შეტყობინების წაშლა ავტომატურად არ მოხდება - 
                    // უნდა დაველოდოთ "მინიშნებას" რომ შეტყობინება დამუშავდა
                    autoAck: false,
                    consumer: consumer
                );

            }
        }

        private void HandleException(IModel channel, ulong tag)
        {
            channel.BasicReject(tag, false);
        }

        public virtual void OnStart(string consumer)
        {
            //Configuration
            //var name = Configuration.Queue
            Console.WriteLine("starting rabbit channel " + consumer);
        }

        public virtual async Task Handle(string consumer, T message)
        {
            throw new NotImplementedException();
        }


    }

}
