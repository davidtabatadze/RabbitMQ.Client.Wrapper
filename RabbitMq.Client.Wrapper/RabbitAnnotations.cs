using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Client.Wrapper
{

    internal class RabbitAnnotations
    {

        public const string Factory_ArgumentException_General = "Unable to create RabbitMq factory: parameters not enought";
        public const string Factory_ArgumentException_Publisher = "Unable to create RabbitMq publisher: parameters not enought";

        public const string Factory_NotImplementedException = "Unable to create RabbitMq publisher: exchange type '{0}' is not implemented, only 'fanout' and 'direct' are allowed.";

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
