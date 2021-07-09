using System;
using System.Linq;
using System.Collections.Generic;
using RabbitMQ.Client;
using CoreKit.Extension.String;
using CoreKit.Extension.Collection;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Defines basic functionality for rabbit wrapper
    /// </summary>
    public abstract class RabbitBase2 : IDisposable
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

        #region Constants

        /// <summary>
        /// Every exchange/queue is durable
        /// </summary>
        private const bool Durable = true;

        /// <summary>
        /// No exchange/queue is exclusive
        /// </summary>
        private const bool Exclusive = false;

        /// <summary>
        /// No exchange/queue is auto deletable
        /// </summary>
        private const bool Deletable = false;

        #endregion

        #region Properties

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Current channels
        /// </summary>
        internal protected List<IModel> Channels { get; set; }

        /// <summary>
        /// Current channel
        /// </summary>
        internal protected IModel Channel { get { return Channels.First(); } }

        /// <summary>
        /// Channel arguments
        /// </summary>
        private Dictionary<string, object> Arguments = new Dictionary<string, object> { };

        #endregion

        #region Constructors

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        /// <param name="logger">Logger</param>
        private RabbitBase2(RabbitConfigurationBase configuration, ILogger logger = null)
        {
            // ...
            Logger = logger;
            Channels = new List<IModel> { };
            // Base validations
            var invalid = configuration == null ? "Configuration" :
                          configuration.Hosts.IsEmpty() ? "Hosts" :
                          configuration.Name.IsEmpty() ? "Name" :
                          string.Empty;
            if (invalid.HasValue())
            {
                throw new ArgumentException(RabbitAnnotations.Factory_ArgumentException_General, invalid);
            }
        }

        /// <summary>
        /// Publisher constructor
        /// </summary>
        /// <param name="configuration">Publisher configuration</param>
        /// <param name="logger">Logger</param>
        internal RabbitBase2(RabbitPublisherConfiguration configuration, ILogger logger = null) :
            this((RabbitConfigurationBase)configuration, logger)
        {
            // Declaring channel
            var channel = ChannelCreate(configuration);
            // Validations
            if (configuration.IsExchange)
            {
                var invalid = //configuration.Queues.IsEmpty() ? "Queues" :
                              configuration.Queues.HasValue() && configuration.Queues.Any(q => q.Name.IsEmpty()) ? "Queues.Name" :
                              string.Empty;
                if (invalid.HasValue())
                {
                    throw new ArgumentException(RabbitAnnotations.Factory_ArgumentException_Publisher, invalid);
                }
                if (configuration.Type != ExchangeType.Fanout && configuration.Type != ExchangeType.Direct)
                {
                    throw new NotImplementedException(string.Format(RabbitAnnotations.Factory_NotImplementedException, configuration.Type));
                }
            }
            // Fixings
            configuration.Queues = !configuration.IsExchange || configuration.Queues.IsEmpty()
                                   ?
                                   new List<RabbitPublisherConfiguration.Queue>
                                   {
                                       new RabbitPublisherConfiguration.Queue
                                       {
                                           Name = configuration.Name,
                                           RoutingKeys = new List<string> { configuration.Name }
                                       }
                                   }
                                   :
                                   configuration.Queues;
            foreach (var queue in configuration.Queues)
            {
                queue.RoutingKeys = queue.RoutingKeys.Where(rk => rk.HasValue()).ToList();
                if (queue.RoutingKeys.IsEmpty())
                {
                    queue.RoutingKeys = new List<string> { queue.Name };
                }
            }
            // Declarations
            if (configuration.IsExchange)
            {
                ChannelDeclare(channel, configuration.Name, configuration.Type);
            }
            foreach (var queue in configuration.Queues)
            {
                ChannelDeclare(channel, queue.Name);
                if (configuration.IsExchange)
                {
                    ChannelDeclare(channel, configuration.Name, queue.Name, queue.RoutingKeys);
                }
            }
            // Finalize channel
            Channels.Add(channel);
        }

        /// <summary>
        /// Consumer constructor
        /// </summary>
        /// <param name="configuration">constructor configuration</param>
        /// <param name="logger">Logger</param>
        internal RabbitBase2(RabbitConsumerConfiguration configuration, ILogger logger = null) :
            this((RabbitConfigurationBase)configuration, logger)
        {
            // Validations
            // name

            // Fixing Workers, BatchSize

            //
            for (int i = 1; i <= configuration.Workers; i++)
            {
                // Declaring channel
                var channel = ChannelCreate(configuration);

                ChannelDeclare(channel, configuration.Name);

                // ???
                Channels.Add(channel);
            }
        }

        #endregion

        #region Channel functionality

        /// <summary>
        /// Create channel
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        /// <returns>Fresh channel</returns>
        private IModel ChannelCreate(RabbitConfigurationBase configuration)
        {
            // Connection to rabbit
            var factory = new ConnectionFactory
            {
                NetworkRecoveryInterval = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            // Configuring virtual host
            if (configuration.VirtualHost.HasValue())
            {
                factory.VirtualHost = configuration.VirtualHost;
            }
            // Configuring port
            if (configuration.Port > 0)
            {
                factory.Port = configuration.Port;
            }
            // Configuring user
            if (configuration.User.HasValue())
            {
                factory.UserName = configuration.User;
            }
            // Configuring password
            if (configuration.Password.HasValue())
            {
                factory.Password = configuration.Password;
            }
            // Retunring the factory
            return factory.CreateConnection(configuration.Hosts)
                          .CreateModel();
        }

        /// <summary>
        /// Declare in channel
        /// </summary>
        /// <param name="channel">Current channel</param>
        /// <param name="exchange">Exchange name</param>
        /// <param name="type">Exchange type</param>
        private void ChannelDeclare(IModel channel, string exchange, string type)
        {
            // Declaring exchange
            channel.ExchangeDeclare(
                exchange: exchange,
                type: type,
                durable: Durable,
                autoDelete: Deletable,
                arguments: Arguments
            );
        }

        /// <summary>
        /// Declare in channel
        /// </summary>
        /// <param name="channel">Current channel</param>
        /// <param name="queue">Queue name</param>
        private void ChannelDeclare(IModel channel, string queue)
        {
            // Declaring queue
            channel.QueueDeclare(
                queue: queue,
                durable: Durable,
                exclusive: Exclusive,
                autoDelete: Deletable,
                arguments: Arguments
            );
        }

        /// <summary>
        /// Declare in channel
        /// </summary>
        /// <param name="channel">Current channel</param>
        /// <param name="exchange">Exchange name</param>
        /// <param name="queue">Queue name</param>
        /// <param name="routes">Routing keys</param>
        private void ChannelDeclare(IModel channel, string exchange, string queue, List<string> routes)
        {
            // Declaring bindings
            foreach (var route in routes)
            {
                channel.QueueBind(
                    queue: queue,
                    exchange: exchange,
                    routingKey: route
                );
            }
        }

        #endregion

    }

}