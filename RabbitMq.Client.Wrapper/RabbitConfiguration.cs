using System.Collections.Generic;
using CoreKit.Extension.String;

namespace RabbitMq.Client.Wrapper
{

    /// <summary>
    /// Represents RabbitMQ configuration <see cref="RabbitBase"/>
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
        public ushort Port { get; set; }

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
        public string Exchange { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public string ExchangeType { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public List<string> RoutingKeys { get; set; }

        ///// <summary>
        ///// შეცდომების რიგის სახელი
        ///// </summary>
        //public string QueueError { get; set; }

        /////// <summary>
        /////// Message time to live
        /////// </summary>
        ////public ulong MessageTTL { get; set; }

        /// <summary>
        /// Grouping size for messages
        /// </summary>
        public ushort BatchSize { get; set; }

        /// <summary>
        /// Consumer workers count
        /// </summary>
        public ushort Workers { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public Dictionary<string, object> Arguments { get; set; }

        public string GetName()
        {
            return Queue.HasValue() ? Queue :
                   Exchange.HasValue() ? ExchangeType + Exchange :
                   "undefined";
        }

        ///// <summary>
        ///// კონფიგურაციის კოპირება
        ///// </summary>
        ///// <returns>ახალი კონფიგურაცია</returns>
        //public RabbitConfiguration Clone()
        //{
        //    var json = JsonConvert.SerializeObject(this);
        //    return JsonConvert.DeserializeObject<RabbitConfiguration>(json);
        //}

    }

}
