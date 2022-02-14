using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace RabbitMQ.Client.Wrapper
{

    /// <summary>
    /// Represents message publish functionality
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    public class RabbitPublisher<T> : RabbitBase
    {

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">We are disposing or not</param>
        protected override void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }
            if (disposing)
            {
                Log(LogLevel.Trace, RabbitAnnotations.Information.PublisherDispose, Configuration.Name);
                Routes = null;
                Configuration = null;
                PublishProperties = null;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        /// <param name="logger">Logger</param>
        /// <param name="name">Name of the exchange/queue</param>
        /// <param name="type">Type of the publisher exchange</param>
        /// <param name="headers">Publish headers</param>
        /// <param name="dependencies">Dependencies <see cref="RabbitConfigurationDependency"/></param>   
        public RabbitPublisher(
            RabbitConfigurationBase configuration,
            ILogger logger = null,
            string name = null,
            string type = null,
            Dictionary<string, object> headers = null,
            List<RabbitConfigurationDependency> dependencies = null
        ) : this(
            RabbitConfigurationBase.ToPublisherConfiguration(configuration, name, type, headers, dependencies),
            logger
        )
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        public RabbitPublisher(RabbitPublisherConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
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
            // Defining the routes
            Routes = configuration.Dependencies
                                  .Select(dependency => dependency.Route)
                                  .Where(route => !string.IsNullOrWhiteSpace(route))
                                  .ToList();
            if (Routes.Count == 0)
            {
                Routes.Add(configuration.Name);
            }
            DefaultRoute = Routes.Count == 1 ? Routes[0] : string.Empty;
            // On start event
            OnStart();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration
        /// </summary>
        private RabbitPublisherConfiguration Configuration { get; set; }

        /// <summary>
        /// Routing / Binding keys
        /// </summary>
        public List<string> Routes { get; set; }

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
            return Configuration.Exchange == false || Configuration.Fanout || Routes.Contains(route);
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

        #region Publish

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="messages">Messages</param>
        public async Task Publish(IEnumerable<T> messages)
        {
            // Publishing ...
            await Publish(messages, DefaultRoute);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <param name="route">Route</param>
        public async Task Publish(IEnumerable<T> messages, string route)
        {
            // Publishing ...
            if (messages != null)
            {
                // Publish beggin time
                var stamp = DateTime.Now;
                foreach (var message in messages)
                {
                    await Publish(message, route, false);
                }
                OnPublished((DateTime.Now - stamp).TotalMilliseconds, messages.Count());
            }
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">Message</param>
        public async Task Publish(T message)
        {
            // Publishing ...
            await Publish(message, DefaultRoute);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="route">Route</param>
        public async Task Publish(T message, string route)
        {
            // Publishing ...
            await Publish(message, route, true);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="route">Route</param>
        /// <param name="single">Single message</param>
        private async Task Publish(T message, string route, bool single)
        {
            // Publish beggin time
            var stamp = DateTime.Now;
            // Publishing ...
            await Publish(message, route, null);
            if (single)
            {
                OnPublished((DateTime.Now - stamp).TotalMilliseconds, 1);
            }
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="delay">Delay milliseconds</param>
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
        /// Publish
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="route">Route</param>
        /// <param name="headers">Publish headers</param>
        private async Task Publish(T message, string route, Dictionary<string, object> headers = null)
        {
            // Transforming T message to string
            var messageString = string.Empty;
            try
            {
                // Validations
                if (!ValidateRoute(route))
                {
                    throw new ArgumentException(string.Format(RabbitAnnotations.Exception.PublishNotImplementedException, route), "route");
                }
                // Preparing publish headers
                PublishProperties.Headers = PublishHeaders(headers);
                // Json representation of the message
                messageString = JsonSerializer.Serialize(message, JsonOptions);
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
                });
            }
            catch (Exception exception)
            {
                OnException(exception, message);
            }
        }

        #endregion

        #region Logs

        /// <summary>
        /// On start event log
        /// </summary>
        private void OnStart()
        {
            // ...
            Log(LogLevel.Trace, RabbitAnnotations.Information.PublisherStart, Configuration.Name);
        }

        /// <summary>
        /// On published event log
        /// </summary>
        /// <param name="milliseconds">Publish time in milliseconds</param>
        /// <param name="count">Published messages</param>
        internal virtual void OnPublished(double milliseconds, int count)
        {
            // ...
            Log(LogLevel.Debug, RabbitAnnotations.Information.PublisherPublished, Configuration.Name, count, milliseconds);
        }

        /// <summary>
        /// On exception event log
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="message">Failed message</param>
        internal virtual void OnException(Exception exception, T message)
        {
            // ...
            Log(LogLevel.Error, RabbitAnnotations.Exception.PublishException, Configuration.Name, exception, message);
            throw exception;
        }

        #endregion

    }

}