using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RabbitMQ.Client;
using Newtonsoft.Json;
using CoreKit.Extension.String;
using CoreKit.Extension.Collection;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RabbitPublisher<T> : RabbitBase2
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        public RabbitPublisher(RabbitPublisherConfiguration configuration, ILogger logger = null) : base(configuration, logger)
        {
            // ...
            Configuration = configuration;
            if (configuration.Queues.HasValue())
            {
                RoutingKeys = configuration.Queues.SelectMany(s => s.RoutingKeys ?? new List<string> { }).ToList();
            }
            // Declaring publish properties.
            // Persistent means, that message will be saved on hard disk
            // So it will never dissapear until deleted purposely
            PublishProperties = Channel.CreateBasicProperties();
            PublishProperties.Persistent = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration
        /// </summary>
        public RabbitPublisherConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private List<string> RoutingKeys { get; set; }

        /// <summary>
        /// Publish properties
        /// </summary>
        private IBasicProperties PublishProperties { get; set; }

        /// <summary>
        /// Either the route is fanout or not.
        /// </summary>
        private bool FanRoute { get { return Configuration.Type == ExchangeType.Fanout; } }

        /// <summary>
        /// Defines the default route
        /// </summary>
        private string DefaultRoute
        {
            get
            {
                return RoutingKeys.Count > 1 ? string.Empty : RoutingKeys.First();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if current routing key is valid
        /// </summary>
        /// <param name="route">Routing key</param>
        /// <returns>Valid / Not valid</returns>
        private bool ValidateRoute(string route)
        {
            return FanRoute || RoutingKeys.Contains(route);
        }

        #endregion

        #region Publish functionality

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="messages">T messages</param>
        /// <returns>Empty</returns>
        public virtual async Task Publish(IEnumerable<T> messages)
        {
            // Publishing ...
            await Publish(messages, DefaultRoute);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="messages">T messages</param>
        /// <param name="route">Routing key</param>
        /// <returns>Empty</returns>
        public virtual async Task Publish(IEnumerable<T> messages, string route)
        {
            // Publishing ...
            foreach (var message in messages)
            {
                // Each message
                await Publish(message, route);
            }
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">T message</param>
        /// <returns>Empty</returns>
        public virtual async Task Publish(T message)
        {
            // Publishing ...
            await Publish(message, DefaultRoute);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">T message</param>
        /// <param name="route">Routing key</param>
        /// <returns>Empty</returns>
        public virtual async Task Publish(T message, string route)
        {
            // Transforming T message to string
            var messageString = string.Empty;
            try
            {
                // Validations 
                if (!ValidateRoute(route))
                {
                    throw new ArgumentException(string.Format(RabbitAnnotations.Publish_NotImplementedException, route), "route");
                }
                // Json representation of the message
                messageString = JsonConvert.SerializeObject(message);
                // Performing message publish
                await Task.Run(() =>
                {
                    // Publishing the message
                    Channels[0].BasicPublish(
                       exchange: Configuration.IsExchange ? Configuration.Name : string.Empty,
                       routingKey: route, // .IsExchange ? "" : .Queue,
                       basicProperties: PublishProperties,
                       body: Encoding.UTF8.GetBytes(messageString)
                    );
                    // The message is published...
                    //OnPublish(messageString);
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                // An exception has happened
                //OnException(exception, messageString);
                // Rethrow ...
                //throw new Exception(
                //    string.Format("unable to publish publisher {0} for ??? message {1}", Configuration.Name, messageString),
                //    exception
                //);
            }
        }

        #endregion

    }

}