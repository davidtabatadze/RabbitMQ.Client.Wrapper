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
    /// Represents message consume functionality
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    public class RabbitConsumer<T> : RabbitBase
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
                Log(LogLevel.Trace, RabbitAnnotations.Information.ConsumerDispose, Configuration.Name);
                Configuration = null;
                Retry.Dispose();
            }
            Dispose();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Represents retry message publisher
        /// </summary>
        private class RetryPublisher : RabbitPublisher<T>
        {

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="configuration">Configuration</param>
            public RetryPublisher(RabbitPublisherConfiguration configuration) : base(configuration) { }

        }

        /// <summary>
        /// Represents failed message publisher
        /// </summary>
        private class DeadPublisher : RabbitPublisher<RabbitDeadMessage>
        {

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="configuration">Configuration</param>
            public DeadPublisher(RabbitPublisherConfiguration configuration) : base(configuration) { }

        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration
        /// </summary>
        private RabbitConsumerConfiguration Configuration { get; set; }

        /// <summary>
        /// Retry publisher
        /// </summary>
        private RetryPublisher Retry { get; set; }

        /// <summary>
        /// Consumer workers
        /// </summary>
        private readonly List<IBasicConsumer> Consumers = new List<IBasicConsumer> { };

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration base</param>
        /// <param name="logger">Logger</param>
        /// <param name="name">Name of the exchange/queue<</param>
        /// <param name="workers">Consumer workers count</param>
        /// <param name="batchSize">Grouping size for messages</param>
        /// <param name="retryIntervals">Retry intervals in milliseconds</param>
        /// <param name="dependencies">Dependencies <see cref="RabbitConfigurationDependency"/></param>
        /// <param name="bindings">Bindings <see cref="RabbitConfigurationBinding"/></param>
        public RabbitConsumer(
            RabbitConfigurationBase configuration,
            ILogger logger = null,
            string name = null,
            ushort workers = 0,
            ushort batchSize = 0,
            List<ulong> retryIntervals = null,
            List<RabbitConfigurationDependency> dependencies = null,
            List<RabbitConfigurationBinding> bindings = null
        ) : this(
            RabbitConfigurationBase.ToConsumerConfiguration(configuration, name, workers, batchSize, retryIntervals, dependencies, bindings),
            logger
        )
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        public RabbitConsumer(RabbitConsumerConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
        {
            // ...
            Configuration = configuration;
            Retry = new RetryPublisher(Configuration.RetryExchangeConfiguration);
            // Creating consumers (a.k.a workers)
            for (short i = 0; i < Channels.Count; i++)
            {
                // Current channel
                var channel = Channels[i];
                // Declaring consumer
                var consumer = new RabbitConsumerBase<T>(channel, Configuration.BatchSize);
                OnStart(i);
                // The consumer will not recive messages, until current is not handled
                channel.BasicQos(0, Configuration.BatchSize, false);
                // Action for recieve message(s)
                consumer.Received += (sender, package) =>
                {
                    // Defining the package body
                    var body = Encoding.UTF8.GetString(package.Body.ToArray());
                    // Trying to preserve the body as actual message
                    try
                    {
                        // Convert to actual typed message
                        var message = JsonSerializer.Deserialize<T>(body, JsonOptions);
                        // Defining the next delay
                        var previous = (object)0;
                        if (package.BasicProperties.Headers != null)
                        {
                            package.BasicProperties.Headers.TryGetValue(RabbitAnnotations.Retry.HeaderKey, out previous);
                        }
                        var delay = Configuration.RetryIntervals.FirstOrDefault(interval =>
                            interval > (ulong)Math.Abs(Convert.ToInt64(previous))
                        );
                        // Preserve the message, make it ready for handling
                        consumer.Preserve(message, package.DeliveryTag, delay);
                    }
                    // If something went wrong on this stage ...
                    catch (Exception exception)
                    {
                        // Retrying it does not make sence
                        Task.Run(async () =>
                        {
                            // We do kill the message
                            await Handle(exception, new List<string> { body });
                            channel.BasicAck(package.DeliveryTag, false);
                        });
                    }
                };
                // Action for handle message(s)
                consumer.Handle += (packages, tag) =>
                {
                    // Run hendling
                    Task.Run(async () =>
                    {
                        // Trying to handle message(s)
                        try
                        {
                            // Handle beggin time
                            var stamp = DateTime.Now;
                            // Messages
                            var messages = packages.Select(m => m.Message);
                            // Handling single message
                            if (Configuration.BatchSize == 1)
                            {
                                await Handle(packages[0].Message);
                            }
                            // Handling grouped messages
                            else
                            {
                                await Handle(packages.Select(m => m.Message));
                            }
                            OnHandled((DateTime.Now - stamp).TotalMilliseconds, messages.Count());
                        }
                        // In case if something goes wrong
                        // We either retry the message(s) or kill them
                        catch (Exception exception)
                        {
                            // Handle retry
                            await Handle(
                                exception,
                                packages.Where(package => package.Delay > 0)
                            );
                            // Handle dead
                            await Handle(
                                exception,
                                packages.Where(package => package.Delay == 0)
                                        .Select(package => JsonSerializer.Serialize(package.Message, JsonOptions))
                            );
                        }
                        // Message(s) will be deleted from the queue
                        finally
                        {
                            // Suggestion, that everything is ok and we can delete the message(s) from the queue
                            channel.BasicAck(tag, true);
                        }
                    });
                };
                // Recording consumer workers
                Consumers.Add(consumer);
            }
        }

        #endregion

        #region Functinalities

        /// <summary>
        /// Do start actual consuming
        /// </summary>
        /// <returns>Rabbit consumer</returns>
        public RabbitConsumer<T> StartConsuming()
        {
            // For each channel...
            for (int i = 0; i < Channels.Count; i++)
            {
                // Binding workers
                // Message(s) will not be deleted automatically
                // The consumer will wait acknowledgement to do so
                //channel.BasicConsume(Configuration.Name, false, consumer);
                Channels[i].BasicConsume(Configuration.Name, false, Consumers[i]);
            }
            // Returning this consumer
            return this;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Handle exception with message retry
        /// </summary>
        /// <param name="exception">Arosed exception</param>
        /// <param name="packages">Failed messages with delay milliseconds</param>
        /// <returns>Task</returns>
        private async Task Handle(Exception exception, IEnumerable<(T Message, ulong Delay)> packages)
        {
            // Publish retry
            foreach (var (message, delay) in packages)
            {
                await Retry.Publish(message, delay);
                OnWarning(exception, message);
            }
        }

        /// <summary>
        /// Handle exception with message kill
        /// </summary>
        /// <param name="exception">Arosed exception</param>
        /// <param name="messages">Failed messages</param>
        /// <returns></returns>
        private async Task Handle(Exception exception, IEnumerable<string> messages)
        {
            if (messages.Count() > 0)
            {
                using (var dead = new DeadPublisher(Configuration.DeadQueueConfiguration))
                {
                    // Publish dead
                    foreach (var message in messages)
                    {
                        await dead.Publish(new RabbitDeadMessage
                        {
                            ExceptionType = exception.GetType().ToString(),
                            Exception = exception.Message,
                            MessageType = typeof(T).ToString(),
                            Message = message
                        });
                        OnException(exception, message);
                    }
                }
            }
        }

        /// <summary>
        /// Handle single message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        protected virtual async Task Handle(T message)
        {
            // This should be overriden
            await Task.Run(() =>
            {
                throw new NotImplementedException(RabbitAnnotations.Exception.ConsumerHandlerSingle);
            });
        }

        /// <summary>
        /// Handle batch messages
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns>Task</returns>
        protected virtual async Task Handle(IEnumerable<T> messages)
        {
            // This should be overriden
            await Task.Run(() =>
            {
                throw new NotImplementedException(RabbitAnnotations.Exception.ConsumerHandlerMultiple);
            });
        }

        #endregion

        #region Logs

        /// <summary>
        /// On start event log
        /// </summary>
        /// <param name="index">The consumer index</param>
        private void OnStart(short index)
        {
            // ...
            Log(LogLevel.Trace, RabbitAnnotations.Information.ConsumerStart, Configuration.Name + "[" + index + "]");
        }

        /// <summary>
        /// On handled event log
        /// </summary>
        /// <param name="milliseconds">Handle time in milliseconds</param>
        /// <param name="count">Handled messages</param>
        internal virtual void OnHandled(double milliseconds, int count)
        {
            // ...
            Log(LogLevel.Debug, RabbitAnnotations.Information.ConsumerHandled, Configuration.Name, count, milliseconds);
        }

        /// <summary>
        /// On warning event log
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="messages">Failed message</param>
        internal virtual void OnWarning(Exception exception, T message)
        {
            // ...
            Log(LogLevel.Warning, RabbitAnnotations.Exception.HandlerWarning, Configuration.Name, exception, message);
        }

        /// <summary>
        /// On exception event log
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="message">Failed message</param>
        internal virtual void OnException(Exception exception, object message)
        {
            // ...
            Log(LogLevel.Error, RabbitAnnotations.Exception.HandlerException, Configuration.Name, exception, message);
        }

        #endregion

    }

}