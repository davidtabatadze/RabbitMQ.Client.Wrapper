using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents message publish functionality
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    public abstract class RabbitPublisher<T> : RabbitBase
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
            // Declaring publish properties.
            PublishProperties = Channel.CreateBasicProperties();
            PublishProperties.Headers = new Dictionary<string, object> { };
            // Persistent means, that message will be saved on hard disk
            // So it will never dissapear until deleted purposely
            PublishProperties.Persistent = true;
            // Defining the destination
            Destination = Configuration.Exchange ? Configuration.Name : string.Empty;
            // Defining the dafault road
            DefaultRoute = Configuration.Routings.Count > 1 ? string.Empty : Configuration.Routings.First();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration
        /// </summary>
        public RabbitPublisherConfiguration Configuration { get; set; }

        /// <summary>
        /// Publish properties
        /// </summary>
        private IBasicProperties PublishProperties { get; set; }

        /// <summary>
        /// Defines the default route
        /// </summary>
        private string DefaultRoute { get; set; }

        /// <summary>
        /// Publish destination - name of the exchange or queue
        /// </summary>
        private string Destination { get; set; }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if current routing key is valid
        /// </summary>
        /// <param name="route">Routing key</param>
        /// <returns>Valid / Not valid</returns>
        private bool ValidateRoute(string route)
        {
            return Configuration.Exchange == false || Configuration.Fanout || Configuration.Routings.Contains(route);
        }

        /// <summary>
        /// Getting publish headers
        /// </summary>
        /// <param name="merge">Headers per publish</param>
        /// <returns>Publish headers</returns>
        private Dictionary<string, object> PublishHeaders(Dictionary<string, object> merge)
        {
            // Defining default headers
            var sources = Configuration.Headers == null ? new List<(string Key, object Value)> { } :
                          Configuration.Headers.Select(h => (h.Key, h.Value)).ToList();
            // Defining publish headers
            sources.AddRange(merge == null ? new List<(string Key, object Value)> { } :
                            merge.Select(h => (h.Key, h.Value)).ToList());
            // Merging headers
            var headers = new Dictionary<string, object> { };
            foreach (var (Key, Value) in sources)
            {
                if (headers.ContainsKey(Key))
                {
                    headers[Key] = Value;
                }
                else
                {
                    headers.Add(Key, Value);
                }
            }
            // Returning final headers
            return headers;
        }

        #endregion

        #region Publish functionality

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async Task Publish(IEnumerable<T> messages)
        {
            // Publishing ...
            await Publish(messages, DefaultRoute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task Publish(IEnumerable<T> messages, string route)
        {
            // Publishing ...
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    await Publish(message, route);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Publish(T message)
        {
            // Publishing ...
            await Publish(message, DefaultRoute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task Publish(T message, string route)
        {
            // Publishing ...
            await Publish(message, route, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        internal async Task Publish(T message, ulong delay)
        {
            // Publishing ...
            await Publish(
                message,
                DefaultRoute,
                new Dictionary<string, object> { { RabbitAnnotations.Retry.HeaderKey, (long)delay } }
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="route"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task Publish(T message, string route, Dictionary<string, object> headers = null)
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
                // Preparing publish headers
                PublishProperties.Headers = PublishHeaders(headers);
                // Json representation of the message
                messageString = JsonSerializer.Serialize(message);
                // Performing message publish
                await Task.Run(() =>
                {
                    // Publishing the message
                    Channel.BasicPublish(
                       exchange: Destination,
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