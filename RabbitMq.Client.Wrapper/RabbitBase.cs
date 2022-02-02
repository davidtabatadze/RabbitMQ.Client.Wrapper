﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Defines basic functionality for rabbit wrapper
    /// </summary>
    public abstract class RabbitBase : IDisposable
    {

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">We are disposing or not</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }
            // If disposing
            if (disposing)
            {
                // ...
                Logger = null;
                Arguments = null;
                foreach (var channel in Channels)
                {
                    channel.Dispose();
                }
                Channels = null;
            }
            Disposed = true;
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

        /// <summary>
        /// Json options
        /// </summary>
        protected readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        #endregion

        #region Properties

        /// <summary>
        /// Either disposed or not
        /// </summary>
        protected bool Disposed { get; set; } = false;

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
        private RabbitBase(RabbitConfigurationBase configuration, ILogger logger = null)
        {
            // ...
            Logger = logger;
            Channels = new List<IModel> { };
            if (configuration.Arguments != null)
            {
                Arguments = configuration.Arguments;
            }
            // Base validations
            var invalid = configuration == null ? "Configuration" :
                          configuration.Hosts == null || configuration.Hosts.Count == 0 ? "Hosts" :
                          string.IsNullOrWhiteSpace(configuration.Name) ? "Name" :
                          string.Empty;
            if (!string.IsNullOrWhiteSpace(invalid))
            {
                throw new ArgumentException(RabbitAnnotations.Exception.FactoryArgumentExceptionGeneral, invalid);
            }
        }

        /// <summary>
        /// Publisher constructor
        /// </summary>
        /// <param name="configuration">Publisher configuration</param>
        /// <param name="logger">Logger</param>
        internal RabbitBase(RabbitPublisherConfiguration configuration, ILogger logger = null) :
            this((RabbitConfigurationBase)configuration, logger)
        {
            // Fixings
            if (configuration.Routings == null)
            {
                configuration.Routings = new List<string> { configuration.Name };
            }
            configuration.Routings = configuration.Routings?.Where(route => !string.IsNullOrWhiteSpace(route)).ToList();
            // Validations
            if (configuration.Routings.Count == 0)
            {
                throw new ArgumentException(RabbitAnnotations.Exception.FactoryArgumentExceptionPublisher, "Routings");
            }
            if (configuration.Exchange)
            {
                if (!new List<string> { "x-delayed-message", "topic", "direct", "fanout" }.Contains(configuration.Type))
                {
                    throw new NotImplementedException(string.Format(RabbitAnnotations.Exception.FactoryNotImplementedExceptionPublisher, configuration.Type));
                }
            }
            // Declaring channel
            var channel = DeclareChannel(configuration);
            // Declaring exchange
            if (configuration.Exchange)
            {
                DeclareExchange(channel, configuration.Name, configuration.Type, Arguments);
            }
            // Declaring queue
            else
            {
                DeclareQueue(channel, configuration.Name, null);
            }
            // Register channel
            Channels.Add(channel);
        }

        /// <summary>
        /// Consumer constructor
        /// </summary>
        /// <param name="configuration">constructor configuration</param>
        /// <param name="logger">Logger</param>
        internal RabbitBase(RabbitConsumerConfiguration configuration, ILogger logger = null) :
            this((RabbitConfigurationBase)configuration, logger)
        {
            // Fixings
            configuration.Workers = configuration.Workers < 1 ? (ushort)1 : configuration.Workers;
            configuration.BatchSize = configuration.BatchSize < 1 ? (ushort)1 : configuration.BatchSize;
            configuration.Bindings ??= new Dictionary<string, string> { };
            configuration.RetryIntervals ??= new List<ulong> { };
            //configuration.RetryIntervals.Add(5000); // D.T. trick
            configuration.RetryIntervals = configuration.RetryIntervals
                                                        .Where(interval => interval >= 5000)
                                                        .OrderBy(interval => interval)
                                                        .ToList();
            // Preparations
            var retry = configuration.RetryExchangeConfiguration;
            configuration.Bindings.Add(configuration.Name, retry.Name);
            // Declarations
            for (int i = 1; i <= configuration.Workers; i++)
            {
                // Declaring channel
                var channel = DeclareChannel(configuration);
                // Declaring paired retry exchange
                DeclareExchange(channel, retry.Name, retry.Type, retry.Arguments);
                // Declaring queue and bindings
                DeclareQueue(channel, configuration.Name, configuration.Bindings);
                // Registering channel
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
        private IModel DeclareChannel(RabbitConfigurationBase configuration)
        {
            // Connection to rabbit
            var factory = new ConnectionFactory
            {
                NetworkRecoveryInterval = TimeSpan.FromSeconds(120),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            // Configuring virtual host
            if (!string.IsNullOrWhiteSpace(configuration.VirtualHost))
            {
                factory.VirtualHost = configuration.VirtualHost;
            }
            // Configuring port
            if (configuration.Port > 0)
            {
                factory.Port = configuration.Port;
            }
            // Configuring user
            if (!string.IsNullOrWhiteSpace(configuration.User))
            {
                factory.UserName = configuration.User;
            }
            // Configuring password
            if (!string.IsNullOrWhiteSpace(configuration.Password))
            {
                factory.Password = configuration.Password;
            }
            // Retunring the factory
            return factory.CreateConnection(configuration.Hosts)
                          .CreateModel();
        }

        /// <summary>
        /// Declare exchange
        /// </summary>
        /// <param name="channel">Current channel</param>
        /// <param name="exchange">Exchange name</param>
        /// <param name="type">Exchange type</param>
        /// <param name="arguments">Channel arguments</param>
        private void DeclareExchange(IModel channel, string exchange, string type, Dictionary<string, object> arguments)
        {
            // Declaring exchange
            channel.ExchangeDeclare(
                exchange: exchange,
                type: type,
                durable: Durable,
                autoDelete: Deletable,
                arguments: arguments
            );
        }

        /// <summary>
        /// Declare queue
        /// </summary>
        /// <param name="channel">Current channel</param>
        /// <param name="queue">Queue name</param>
        /// <param name="bindings">Pair of routing - exchange</param>
        private void DeclareQueue(IModel channel, string queue, Dictionary<string, string> bindings)
        {
            // Declaring queue
            channel.QueueDeclare(
                queue: queue,
                durable: Durable,
                exclusive: Exclusive,
                autoDelete: Deletable,
                arguments: Arguments
            );
            // Declaring bindings
            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    channel.QueueBind(
                        queue: queue,
                        exchange: binding.Value,
                        routingKey: binding.Key
                    );
                }
            }
        }

        #endregion

        #region Logging

        /// <summary>
        /// Logging
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="message">Message</param>
        /// <param name="args">Arguments</param>
        internal protected void Log(LogLevel level, string message, params object[] args)
        {
            // If logger exists
            if (Logger != null)
            {
                // We do log
                Logger.Log(level, message, args);
            }
        }

        #endregion

    }

}