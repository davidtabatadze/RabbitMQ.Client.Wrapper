using System.Collections.Generic;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// ???
    /// </summary>
    public abstract class RabbitConfigurationBase
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

    }

}