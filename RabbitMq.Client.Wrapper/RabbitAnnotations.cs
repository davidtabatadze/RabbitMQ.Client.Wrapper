

namespace RabbitMq.Client.Wrapper
{


    /// <summary>
    /// Represents annotations for RabbitMq wrapper
    /// </summary>
    internal class RabbitAnnotations
    {

        /// <summary>
        /// Retry message functionality annotation
        /// </summary>
        internal class Retry
        {

            public const string NameSuffix = "-retry";
            public const string Type = "x-delayed-message";
            public const string ArgumentKey = "x-delayed-type";
            public const string ArgumentValue = "fanout";
            public const string HeaderKey = "x-delay";

        }

        /// <summary>
        /// Dead message functionality annotation
        /// </summary>
        internal class Dead
        {

            public const string NameSuffix = "-dead";

        }

        /// <summary>
        /// Informative notification annotation
        /// </summary>
        internal class Information
        {

        }

        /// <summary>
        /// Exception notification annotation
        /// </summary>
        internal class Exception
        {

            public const string ConsumerHandlerSingle = "Handle(T message, ...) is not implemented. Override the method.";
            public const string ConsumerHandlerMultiple = "Handle(IEnumerable<T> messages, ...) is not implemented. Override the method.";

        }

        public const string Factory_ArgumentException_General = "Unable to create RabbitMq factory: parameters not enought";
        public const string Factory_ArgumentException_Publisher = "Unable to create RabbitMq publisher: parameters not enought";

        public const string Factory_NotImplementedException = "Unable to create RabbitMq publisher: exchange type '{0}' is not implemented, only 'topic', 'direct' and 'fanout' are allowed.";

        /// <summary>
        /// 
        /// </summary>
        //public const string Publish_ArgumentException = "Unable to publish RabbitMq message: parameters not enought";

        /// <summary>
        /// 
        /// </summary>
        public const string Publish_NotImplementedException = "Unable to publish RabbitMq message: route should be present. '{0}' is not a valid routing key.";

    }

}
