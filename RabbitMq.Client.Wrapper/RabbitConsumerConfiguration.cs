using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// ???
    /// </summary>
    public class RabbitConsumerConfiguration : RabbitConfigurationBase
    {

        /// <summary>
        /// Grouping size for messages
        /// </summary>
        public ushort BatchSize { get; set; }

        /// <summary>
        /// Consumer workers count
        /// </summary>
        public ushort Workers { get; set; }

    }

}