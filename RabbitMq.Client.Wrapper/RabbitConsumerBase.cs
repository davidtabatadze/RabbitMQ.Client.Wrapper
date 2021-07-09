using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents base for <see cref="RabbitConsumer{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of rabbit message</typeparam>
    internal class RabbitConsumerBase<T> : EventingBasicConsumer
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channel">Common AMPQ model</param>
        public RabbitConsumerBase(IModel channel) : base(channel)
        {
            Timer.Elapsed += (sender, eventArgs) =>
            {
                // თუკი გვაქვს შეტყობინებები
                if (Messages.Count > 0)
                {
                    // გამომყენებელს დავაცარიელებთ
                    Discharge();
                }
            };
        }

        /// <summary>
        /// Time to discharge the accumulated messages. 10 seconds
        /// </summary>
        private Timer Timer = new Timer(10 * 1000) { Enabled = false };

        /// <summary>
        /// Accumulated messages
        /// </summary>
        private List<T> Messages = new List<T> { };

        private ulong LastTag { get; set; }

        private void Discharge()
        {

            System.Console.WriteLine("discharge-" + System.DateTime.Now.TimeOfDay.ToString());

            var messages = Messages.Select(m => m).ToList();

            Messages = new List<T> { };

            Timer.Stop();

            Handle?.Invoke(LastTag, messages);

        }

        internal protected delegate void HandleEvent(ulong tag, List<T> messages);

        internal protected event HandleEvent Handle;

        internal protected void Preserve(ulong tag, T message)
        {
            LastTag = tag;
            Messages.Add(message);


            Handle?.Invoke(LastTag, new List<T> { message });

            if (Messages.Count > 10)
            {
                Discharge();
            }

            // Restart the discharge timer
            Timer.Stop();
            Timer.Start();


        }

    }

}

