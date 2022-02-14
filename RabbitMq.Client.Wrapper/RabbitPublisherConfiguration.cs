using System.Collections.Generic;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents configuration of <see cref="RabbitPublisher{T}"/>
    /// </summary>
    public class RabbitPublisherConfiguration : RabbitConfigurationBase
    {

        /// <summary>
        /// Type of the publisher exchange
        /// </summary>
        /// <remarks>
        /// If this parameter is not present, the publisher will be considered as direct queue, not exchange
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// Publish headers
        /// </summary>
        public Dictionary<string, object> Headers { get; set; }

        /// <summary>
        /// Either current configuration is exchange or not.
        /// </summary>
        internal bool Exchange { get { return !string.IsNullOrEmpty(Type); } }

        /// <summary>
        /// Either current configuration is fanout exchange or not.
        /// </summary>
        internal bool Fanout { get { return Type == ExchangeType.Fanout; } }

    }

}