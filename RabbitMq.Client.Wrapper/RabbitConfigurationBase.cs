using System.Collections.Generic;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents RabbitMQ configuration basis
    /// </summary>
    public class RabbitConfigurationBase
    {
        /// <summary>
        /// The hosts to connect to
        /// </summary>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// Vistual address
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Connection port
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Connection user
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Connection password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Name of the exchange/queue
        /// </summary>
        /// <remarks>
        /// Depending of the configuration, the value is either exchange name or queue name
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Additional arguments of the exchange/queue
        /// </summary>
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Bindings <see cref="RabbitConfigurationBinding"/>
        /// </summary>
        /// <remarks>Named exchange/queue should already exist, it will not be created</remarks>
        public List<RabbitConfigurationBinding> Bindings { get; set; }

        /// <summary>
        /// Dependencies <see cref="RabbitConfigurationDependency"/>
        /// </summary>
        public List<RabbitConfigurationDependency> Dependencies { get; set; }

        /// <summary>
        /// Convert base to <see cref="RabbitPublisherConfiguration"/>
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        /// <param name="name">Name of the exchange/queue</param>
        /// <param name="type">Type of the publisher exchange</param>
        /// <param name="headers">Publish headers</param>
        /// <param name="dependencies">Dependencies <see cref="RabbitConfigurationDependency"/></param>        
        /// <returns>Fresh instance of <see cref="RabbitPublisherConfiguration"/></returns>
        internal static RabbitPublisherConfiguration ToPublisherConfiguration(
            RabbitConfigurationBase configuration,
            string name = null,
            string type = null,
            Dictionary<string, object> headers = null,
            List<RabbitConfigurationDependency> dependencies = null
        )
        {
            // Composing publisher configuration
            return new RabbitPublisherConfiguration
            {
                Hosts = configuration.Hosts,
                VirtualHost = configuration.VirtualHost,
                Port = configuration.Port,
                User = configuration.User,
                Password = configuration.Password,
                Arguments = configuration.Arguments,
                Name = name,
                Type = type,
                Headers = headers,
                Dependencies = dependencies
            };
        }

        /// <summary>
        /// Convert base to <see cref="RabbitConsumerConfiguration"/>
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        /// <param name="name">Name of the exchange/queue<</param>
        /// <param name="workers">Consumer workers count</param>
        /// <param name="batchSize">Grouping size for messages</param>
        /// <param name="retryIntervals">Retry intervals in milliseconds</param>
        /// <param name="dependencies">Dependencies <see cref="RabbitConfigurationDependency"/></param>
        /// <param name="bindings">Bindings <see cref="RabbitConfigurationBinding"/></param>
        /// <returns>Fresh instance of <see cref="RabbitConsumerConfiguration"/></returns>
        internal static RabbitConsumerConfiguration ToConsumerConfiguration(
            RabbitConfigurationBase configuration,
            string name,
            ushort workers = 0,
            ushort batchSize = 0,
            List<ulong> retryIntervals = null,
            List<RabbitConfigurationDependency> dependencies = null,
            List<RabbitConfigurationBinding> bindings = null
        )
        {
            // Composing consumer configuration
            return new RabbitConsumerConfiguration
            {
                Hosts = configuration.Hosts,
                VirtualHost = configuration.VirtualHost,
                Port = configuration.Port,
                User = configuration.User,
                Password = configuration.Password,
                Arguments = configuration.Arguments,
                Name = name,
                Workers = workers,
                BatchSize = batchSize,
                RetryIntervals = retryIntervals,
                Dependencies = dependencies,
                Bindings = bindings
            };
        }

    }

}