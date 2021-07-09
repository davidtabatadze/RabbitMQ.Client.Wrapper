using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using CoreKit.Extension.String;
using CoreKit.Extension.Collection;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class RabbitBase<T> : IDisposable
    {

        #region Dispose

        /// <summary>
        /// კლასის სიცოცხლის ციკლის დასრულება
        /// </summary>
        public void Dispose()
        {
            //
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// კლასის სიცოცხლის ციკლის დასრულება
        /// </summary>
        /// <param name="disposing">ციკლის დასრულების მნიშვნელობა</param>
        protected virtual void Dispose(bool disposing)
        {
            // თუკი ციკლი სრულდება
            if (disposing)
            {
                // ...
            }
        }

        #endregion

        internal RabbitBase(RabbitConfiguration configuration, ILogger logger = null)
        {
            // ...
            Channels = new List<IModel> { };
            Configuration = configuration;
            if (logger != null)
            {
                Logger = logger;
            }
            Start();
        }






        private IModel CreateChannel()
        {
            // Connection to rabbit
            var factory = new ConnectionFactory
            {
                NetworkRecoveryInterval = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            // Configuring virtual host
            if (Configuration.VirtualHost.HasValue())
            {
                factory.VirtualHost = Configuration.VirtualHost;
            }
            // Configuring port
            if (Configuration.Port > 0)
            {
                factory.Port = Configuration.Port;
            }
            // Configuring user
            if (Configuration.UserName.HasValue())
            {
                factory.UserName = Configuration.UserName;
            }
            // Configuring password
            if (Configuration.Password.HasValue())
            {
                factory.Password = Configuration.Password;
            }
            // Retunring the factory
            return factory.CreateConnection(Configuration.Hosts)
                          .CreateModel();
        }


        /// <summary>
        /// 
        /// </summary>
        internal protected void Start()
        {
            // Validations
            var invalid = Configuration == null ? "Configuration" :
                      Configuration.Hosts.IsEmpty() ? "Hosts" :
                      (Configuration.Queue.IsEmpty() && Configuration.Exchange.IsEmpty()) ? "Queue/Exchange" :
                      (Configuration.Exchange.HasValue() && Configuration.ExchangeType.IsEmpty()) ? "ExchangeType" :
                      string.Empty;
            if (invalid.HasValue())
            {
                throw new ArgumentException(RabbitAnnotations.Factory_ArgumentException_General, invalid);
            }
            if (GetType().IsSubclassOf(typeof(RabbitPublisher<T>)) && Configuration.Queue.HasValue() && Configuration.Exchange.HasValue())
            {
                throw new ArgumentException(RabbitAnnotations.Factory_ArgumentException_Publisher, "Queue/Exchange");
            }
            if (Configuration.ExchangeType != ExchangeType.Fanout && Configuration.ExchangeType != ExchangeType.Direct)
            {
                throw new NotImplementedException(string.Format(RabbitAnnotations.Factory_NotImplementedException, Configuration.ExchangeType));
            }
            // Fixings
            Configuration.Workers = Configuration.Workers < 1 ? (ushort)1 : Configuration.Workers;
            Configuration.BatchSize = Configuration.BatchSize < 1 ? (ushort)1 : Configuration.BatchSize;
            Configuration.RoutingKeys = Configuration.RoutingKeys.HasValue() ? Configuration.RoutingKeys : new List<string> { string.Empty };
            //
            for (int i = 1; i <= Configuration.Workers; i++)
            {
                // Declaring channel
                var channel = CreateChannel();
                // Defining arguments
                var arguments = new Dictionary<string, object> { };
                if (Configuration.Arguments.HasValue())
                {
                    foreach (var argument in arguments)
                    {
                        arguments.Add(argument.Key, argument.Value);
                    }
                }
                // Defining parameters
                var durable = true;
                var exclusive = false;
                var autoDelete = false;
                // Declaring exchange
                if (Configuration.Exchange.HasValue())
                {
                    channel.ExchangeDeclare(
                        exchange: Configuration.Exchange,
                        type: Configuration.ExchangeType,
                        durable: durable,
                        autoDelete: autoDelete,
                        arguments: arguments
                    );
                }
                // Declaring queue
                if (Configuration.Queue.HasValue())
                {
                    channel.QueueDeclare(
                       queue: Configuration.Queue,
                       durable: durable,
                       exclusive: exclusive,
                       autoDelete: autoDelete,
                       arguments: arguments
                    );
                    // If excahnge is present, we do bind the queue to it
                    if (Configuration.Exchange.HasValue())
                    {
                        foreach (var key in Configuration.RoutingKeys)
                        {
                            channel.QueueBind(
                                routingKey: key,
                                queue: Configuration.Queue,
                                exchange: Configuration.Exchange
                            );
                        }
                    }
                }
                // ???
                Channels.Add(channel);
                //OnStart(i/*channel.ChannelNumber*/);
            }
        }

        /// <summary>
        /// Configuration
        /// </summary>
        internal protected RabbitConfiguration Configuration { get; set; }

        /// <summary>
        /// Logger
        /// </summary>
        internal protected ILogger Logger { get; set; }

        /// <summary>
        /// Channels
        /// </summary>
        internal protected List<IModel> Channels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal protected IModel Channel { get { return Channels[0]; } }



        //public virtual void OnStart(int channelNumber)
        //{
        //    //var name = Configuration.Queue
        //    Console.WriteLine("starting rabbit channel " + channelNumber);
        //}

        public virtual void OnWarning(Exception exception, string message)
        {
            Console.WriteLine("warning");
        }

        public virtual void OnException(Exception exception, string message)
        {
            Console.WriteLine("exception");
        }

    }

}
