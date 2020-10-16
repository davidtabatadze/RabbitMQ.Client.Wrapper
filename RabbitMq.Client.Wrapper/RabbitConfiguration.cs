using Newtonsoft.Json;
using System.Collections.Generic;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents RabbitMQ configuration <see cref="Rabbit"/>
    /// </summary>
    public class RabbitConfiguration
    {

        /// <summary>
        /// The hosts to connect to
        /// </summary>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// Vistual address
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Host(s) port
        /// </summary>
        public short Port { get; set; }

        /// <summary>
        /// Host(s) user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Host(s) password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// რიგის სახელი
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// ექსჩეინჯ-რიგის სახელი
        /// </summary>
        public bool QueueExchange { get; set; }

        /// <summary>
        /// შეცდომების რიგის სახელი
        /// </summary>
        public string QueueError { get; set; }

        /// <summary>
        /// Message time to live
        /// </summary>
        public long MessageTTL { get; set; }

        /// <summary>
        /// Grouping size for messages
        /// </summary>
        public ushort BatchSize { get; set; }

        /// <summary>
        /// Consumer workers count
        /// </summary>
        public short Workers { get; set; }

        /// <summary>
        /// შეერთებული სახელის აღება
        /// </summary>
        /// <param name="index">მიმდინარე ნომერი</param>
        /// <returns>შეერთებული სახელი</returns>
        public string GetCombinedName(short index)
        {
            return Queue + " " + "[" + Workers + "-" + index + "]";
        }

        /// <summary>
        /// კონფიგურაციის კოპირება
        /// </summary>
        /// <returns>ახალი კონფიგურაცია</returns>
        public RabbitConfiguration Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<RabbitConfiguration>(json);
        }

    }

}
