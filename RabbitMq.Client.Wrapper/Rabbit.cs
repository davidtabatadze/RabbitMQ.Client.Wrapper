using System;
using System.Timers;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Client.Wrapper
{
    class Rabbit
    {

        public static ConnectionFactory CreateFactory(RabbitConfiguration configuration)
        {
            return new ConnectionFactory { };
        }

        public static IConnection CreateConnection(RabbitConfiguration configuration)
        {
            return new ConnectionFactory { }.CreateConnection();
        }

        public static IModel CreateChannel(RabbitConfiguration configuration)
        {
            return new ConnectionFactory { }.CreateConnection().CreateModel();
        }

    }
}
