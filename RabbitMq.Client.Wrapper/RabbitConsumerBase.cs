using System.Linq;
using System.Timers;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents base for <see cref="RabbitConsumer{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    internal class RabbitConsumerBase<T> : EventingBasicConsumer
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channel">Common AMPQ model</param>
        /// <param name="name">Name of the consumer</param>
        /// <param name="batch">Grouping size for packages</param>
        internal RabbitConsumerBase(IModel channel, string name, ushort batch) : base(channel)
        {
            // Setting batch size
            BatchSize = batch;
            Name = name;
            // When time is up...
            Timer.Elapsed += (sender, eventArgs) =>
            {
                System.Console.WriteLine("tick " + name);
                // We do discharge the packages(s)
                if (Packages.Count > 0)
                {
                    Discharge();
                }
            };
        }

        /// <summary>
        /// Package(s) handling delegate
        /// </summary>
        /// <param name="packages">Accumulated packages</param>
        /// <param name="tag">Last tag of package</param>
        /// <param name="name">Consumer worker name</param>
        internal protected delegate void HandleEvent(List<(T Message, ulong Delay)> packages, ulong tag, string name);

        /// <summary>
        /// Event for hendling the package(s)
        /// </summary>
        internal protected event HandleEvent Handle;

        /// <summary>
        /// Time to discharge the accumulated packages. 60 seconds
        /// </summary>
        private Timer Timer = new Timer(60 * 1000) { Enabled = true };

        /// <summary>
        /// Accumulated packages
        /// </summary>
        private List<(T, ulong)> Packages = new List<(T, ulong)> { };

        /// <summary>
        /// Grouping size for packages
        /// </summary>
        private ushort BatchSize { get; set; }

        /// <summary>
        /// Delivery tag of current channel
        /// </summary>
        private ulong LastTag { get; set; }

        /// <summary>
        /// Consumer name
        /// </summary>
        private string Name { get; set; }

        /// <summary>
        /// Discharge the package(s)
        /// </summary>
        private void Discharge()
        {
            // Handle the package(s)
            var packages = Packages.Select(i => i).ToList();
            Packages = new List<(T, ulong)> { };
            Handle?.Invoke(packages, LastTag, Name);
        }

        /// <summary>
        /// Preserve the package
        /// </summary>
        /// <param name="package">Package to preserve</param>
        /// <param name="tag">Package tag</param>
        /// <param name="delay">Retry delay milliseconds</param>
        internal protected void Preserve(T package, ulong tag, ulong delay)
        {
            // Preserving the data
            LastTag = tag;
            Packages.Add((package, delay));
            // When limit is reached ...
            if (Packages.Count >= BatchSize)
            {
                // We do discharge the packages
                Discharge();
            }
        }

    }

}