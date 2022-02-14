using System.Collections.Generic;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents configuration of <see cref="RabbitConsumer{T}"/>
    /// </summary>
    public class RabbitConsumerConfiguration : RabbitConfigurationBase
    {

        /// <summary>
        /// Retry intervals in milliseconds
        /// </summary>
        /// <remarks>
        /// Count of this list represents the number of tries.
        /// Elements will be sorted asceding.
        /// Elements less then 5000 will be ignored.
        /// If this list is not defined, no reties will happen, message goes do 'dead'.
        /// </remarks>
        public List<ulong> RetryIntervals { get; set; }

        /// <summary>
        /// Grouping size for messages
        /// </summary>
        public ushort BatchSize { get; set; }

        /// <summary>
        /// Consumer workers count
        /// </summary>
        public ushort Workers { get; set; }

        /// <summary>
        /// Gets retry exchange configuration based on the queue configuration
        /// </summary>
        internal RabbitPublisherConfiguration RetryExchangeConfiguration
        {
            get
            {
                return new RabbitPublisherConfiguration
                {
                    Hosts = Hosts,
                    VirtualHost = VirtualHost,
                    Port = Port,
                    User = User,
                    Password = Password,
                    Name = Name + RabbitAnnotations.Retry.NameSuffix,
                    Type = RabbitAnnotations.Retry.Type,
                    Arguments = new Dictionary<string, object> {
                        {
                            RabbitAnnotations.Retry.ArgumentKey,
                            RabbitAnnotations.Retry.ArgumentValue
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Gets dead queue configuration based on the queue configuration
        /// </summary>
        internal RabbitPublisherConfiguration DeadQueueConfiguration
        {
            get
            {
                return new RabbitPublisherConfiguration
                {
                    Hosts = Hosts,
                    VirtualHost = VirtualHost,
                    Port = Port,
                    User = User,
                    Password = Password,
                    Name = Name + RabbitAnnotations.Dead.NameSuffix
                };
            }
        }

    }

}