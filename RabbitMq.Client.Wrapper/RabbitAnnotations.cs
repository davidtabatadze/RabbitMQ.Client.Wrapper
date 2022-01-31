

namespace RabbitMQ.Client.Wrapper
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

            public const string PublisherStart = "Starting RabbitMq publisher: {publisher}.";
            public const string PublisherPublished = "The publisher '{publisher}' has published {count} message(s) in {milliseconds} milliseconds.";
            public const string ConsumerStart = "Starting RabbitMq consumer: {consumer}.";
            public const string ConsumerHandled = "The consumer '{consumer}' has handled {count} message(s) in {milliseconds} milliseconds.";

        }

        /// <summary>
        /// Exception notification annotation
        /// </summary>
        internal class Exception
        {

            public const string FactoryArgumentExceptionGeneral = "Unable to create RabbitMq factory: parameters not enought.";
            public const string FactoryArgumentExceptionPublisher = "Unable to create RabbitMq publisher: parameters not enought.";
            public const string FactoryNotImplementedExceptionPublisher = "Unable to create RabbitMq publisher: exchange type '{0}' is not implemented, only 'topic', 'direct', 'fanout' and 'x-delayed-message' are allowed.";
            public const string PublishNotImplementedException = "Unable to publish RabbitMq message: route should be present. '{0}' is not a valid routing key.";
            public const string PublishException = "The publisher {publisher} has faild while publishing the message. exception: {@exception}. message: {@message}.";
            public const string HandlerException = "The consumer {consumer} has faild to handle the message. exception: {@exception}. message: {@message}.";
            public const string HandlerWarning = "The consumer {consumer} has gone wrong while handling the message. Retry in progress. exception: {@exception}. message: {@message}.";
            public const string ConsumerHandlerSingle = "Handle(T message, ...) is not implemented. Override the method.";
            public const string ConsumerHandlerMultiple = "Handle(IEnumerable<T> messages, ...) is not implemented. Override the method.";

        }

    }

}
