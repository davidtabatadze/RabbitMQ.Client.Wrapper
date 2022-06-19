

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents RabbitMQ configuration for bindings
    /// </summary>
    public class RabbitConfigurationBinding
    {

        /// <summary>
        /// Routing / Binding key
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Name of the exchange/queue
        /// </summary>
        public string Name { get; set; }

    }

}