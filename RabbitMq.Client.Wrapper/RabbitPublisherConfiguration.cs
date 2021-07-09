using CoreKit.Extension.String;
using System.Collections.Generic;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents configuration of <see cref="RabbitPublisher{T}"/>
    /// </summary>
    public class RabbitPublisherConfiguration : RabbitConfigurationBase
    {


        /// <summary>
        /// Exchange related queue
        /// </summary>
        public class Queue
        {

            /// <summary>
            /// Name of the queue
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Related routes
            /// </summary>
            /// <remarks>
            /// If this parameter is not present, default routing key will become the name of the queue
            /// </remarks>
            public List<string> RoutingKeys { get; set; }

        }

        /// <summary>
        /// Type of the publisher exchange
        /// </summary>
        /// <remarks>
        /// If this parameter is not present, the publisher will be considered as direct queue, not exchange
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        /// Connected queues
        /// </summary>
        /// <remarks>
        /// If the 'Type' parameter is not present, this will be ignored.
        /// If the 'Type' parameter is present, but this is empty, queue will be created with the same name as exchange
        /// </remarks>
        public List<Queue> Queues { get; set; }

        /// <summary>
        /// Either current configuration is exchange or queue.
        /// </summary>
        internal bool IsExchange { get { return Type.HasValue(); } }

    }

}