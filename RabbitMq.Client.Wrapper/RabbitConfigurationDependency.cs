using System.Collections.Generic;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents RabbitMQ configuration for dependencies such as exchanges or queues
    /// </summary>
    public class RabbitConfigurationDependency
    {

        /// <summary>
        /// Routing / Binding key
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Name of the exchange/queue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the exchange
        /// </summary>
        /// <remarks>
        /// If this parameter is not present, the publisher will be considered as direct queue, not exchange
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// Additional arguments of the exchange/queue
        /// </summary>
        public Dictionary<string, object> Arguments { get; set; }

    }

}