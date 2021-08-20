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
    /// Represents message consume functionality
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    public abstract class RabbitConsumer<T> : RabbitBase
    {

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
        public RabbitConsumerConfiguration Configuration { get; set; }

        /// <summary>
        /// Retry publisher
        /// </summary>
        private RetryPublisher Retry { get; set; }

        /// <summary>
        /// Dead publisher
        /// </summary>
        private DeadPublisher Dead { get; set; }


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="logger">Logger</param>
        public RabbitConsumer(RabbitConsumerConfiguration configuration, ILogger logger = null) : base(configuration, logger)
        {
            // ...
            Configuration = configuration;
            Retry = new RetryPublisher(Configuration.RetryExchangeConfiguration);
            Dead = new DeadPublisher(Configuration.DeadQueueConfiguration);
            // Creating consumers (a.k.a workers)
            for (int i = 0; i < Channels.Count; i++)
            {
                // Current channel
                var channel = Channels[i];
                // Declaring consumer
                var name = Configuration.Name + "[consumer-" + (i + 1) + "]";
                var consumer = new RabbitConsumerBase<T>(channel, name, Configuration.BatchSize);
                // The consumer will not recive messages, until current is not handled
                channel.BasicQos(0, Configuration.BatchSize, false);
                // Binding queue to the consumer
                // Message(s) will not be deleted automatically
                // The consumer will wait acknowledgement to do so
                channel.BasicConsume(Configuration.Name, false, consumer);
                // Action for recieve message(s)
                consumer.Received += (sender, package) =>
                {
                    // Defining the package body
                    var body = Encoding.UTF8.GetString(package.Body.ToArray());
                    // Trying to preserve the body as actual message
                    try
                    {
                        // Convert to actual typed message
                        var message = JsonSerializer.Deserialize<T>(body);
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
                            // ???
                            channel.BasicReject(package.DeliveryTag, false);
                        });
                    }
                };
                // Action for handle message(s)
                consumer.Handle += (packages, tag, consumer) =>
                {
                    // Run hendling
                    Task.Run(async () =>
                    {
                        // Trying to handle message(s)
                        try
                        {
                            // Handling single message
                            if (Configuration.BatchSize == 1)
                            {
                                await Handle(packages[0].Message, consumer);
                            }
                            // Handling grouped messages
                            else
                            {
                                await Handle(packages.Select(m => m.Message), consumer);
                            }
                        }
                        // In case if something goes wrong
                        // We either retry the message(s) or kill them
                        catch (Exception exception)
                        {
                            // Hanle retry
                            await Handle(
                                exception,
                                packages.Where(package => package.Delay > 0)
                            );
                            // Hanle dead
                            await Handle(
                                exception,
                                packages.Where(package => package.Delay == 0)
                                        .Select(package => JsonSerializer.Serialize(package.Message))
                            );
                        }
                        // In any case, message(s) will be deleted from the queue
                        finally
                        {
                            // Suggestion, that everything is ok and we can delete the message(s) from the queue
                            channel.BasicAck(tag, true);
                        }
                    });
                };
            }
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
            foreach (var (Message, Delay) in packages)
            {
                await Retry.Publish(Message, Delay);
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
            // Publish dead
            await Dead.Publish(messages.Select(message => new RabbitDeadMessage
            {
                ExceptionType = exception.GetType().ToString(),
                Exception = exception.Message,
                MessageType = typeof(T).ToString(),
                Message = message
            }));
        }

        /// <summary>
        /// Handle single message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="consumer">Consumer name</param>
        /// <returns>Task</returns>
        public virtual async Task Handle(T message, string consumer)
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
        /// <param name="consumer">Consumer name</param>
        /// <returns>Task</returns>
        public virtual async Task Handle(IEnumerable<T> messages, string consumer)
        {
            // This should be overriden
            await Task.Run(() =>
            {
                throw new NotImplementedException(RabbitAnnotations.Exception.ConsumerHandlerMultiple);
            });
        }

        #endregion










        public virtual void OnStart(string consumer)
        {
            //Configuration
            //var name = Configuration.Queue
            Console.WriteLine("starting rabbit channel " + consumer);
        }




    }

}