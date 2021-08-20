

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents dead message
    /// </summary>
    public class RabbitDeadMessage
    {

        /// <summary>
        /// Type of the arose exception
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Message of the arose exception
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Type of the failed message
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Failed message
        /// </summary>
        public string Message { get; set; }

    }

}