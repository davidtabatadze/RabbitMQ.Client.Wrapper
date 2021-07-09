using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RabbitMq.Client.Wrapper.Test
{
    class Program
    {

        /// <summary>
        /// ტრანზაქციის დამატებითი კონტექსტური მონაცემები
        /// </summary>
        public class Context
        {

            /// <summary>
            /// კლიენტის ნაწილი (კლიენტის მიერ გამოგზავნილი დამატებითი პარამეტრები უცვლელად)
            /// </summary>
            [JsonProperty("rq", NullValueHandling = NullValueHandling.Ignore)]
            public Dictionary<string, object> Request { get; set; }

            /// <summary>
            /// ოპტიოს ნაწილი (კატეგორიზაციის დამატებითი მონაცემები, აღწერები, ინსტრუქციები და ა.შ.)
            /// </summary>
            [JsonProperty("rp", NullValueHandling = NullValueHandling.Ignore)]
            public Dictionary<string, object> Response { get; set; }

        }

        /// <summary>
        /// ტრანზაქციის გატარების/გადახდის მდებარეობა - ტერმინალის მდებარეობა
        /// </summary>
        public class Location
        {

            /// <summary>
            /// ქვეყანა
            /// </summary>
            [JsonProperty("cu", NullValueHandling = NullValueHandling.Ignore)]
            public string Country { get; set; }

            /// <summary>
            /// ქალაქი
            /// </summary>
            [JsonProperty("ct", NullValueHandling = NullValueHandling.Ignore)]
            public string City { get; set; }

            /// <summary>
            /// მისამართი
            /// </summary>
            [JsonProperty("ad", NullValueHandling = NullValueHandling.Ignore)]
            public string Address { get; set; }

            /// <summary>
            /// გრძედი
            /// </summary>
            [JsonProperty("lo", NullValueHandling = NullValueHandling.Ignore)]
            public double Longitude { get; set; }

            /// <summary>
            /// განედი
            /// </summary>
            [JsonProperty("la", NullValueHandling = NullValueHandling.Ignore)]
            public double Latitude { get; set; }

        }

        /// <summary>
        /// დაკატეგორიზებული ტრანზაქცია
        /// </summary>
        public class Outcome
        {

            /// <summary>
            /// ტრანზაქციის იდენთიფიკატორი - უნიკალური გასაღები
            /// </summary>
            [JsonProperty("id")]
            public string Id { get; set; }

            /// <summary>
            /// კონტექსტური და/ან დამატებითი მონაცემები
            /// </summary>
            [JsonProperty("ct", NullValueHandling = NullValueHandling.Ignore)]
            public Context Context { get; set; }

            /// <summary>
            /// ტრანზაქციის გატარების/გადახდის მდებარეობა - ტერმინალის მდებარეობა
            /// </summary>
            [JsonProperty("lc", NullValueHandling = NullValueHandling.Ignore)]
            public Location Location { get; set; }

            /// <summary>
            /// გატარების/გადახდის ტერმინალი
            /// </summary>
            [JsonProperty("tr", NullValueHandling = NullValueHandling.Ignore)]
            public string Terminal { get; set; }

            /// <summary>
            /// კატეგორიის წარმომავლობა merchant | mcc | rule | none
            /// </summary>
            [JsonProperty("co")]
            public string CategoryOrigin { get; set; }

            /// <summary>
            /// rule წარმომავლობის შესაბამისი წესი - სახელი
            /// </summary>
            [JsonProperty("cr", NullValueHandling = NullValueHandling.Ignore)]
            public string CategoryRule { get; set; }

            /// <summary>
            /// კატეგორიის გასაღები
            /// </summary>
            [JsonProperty("ci")]
            public int CategoryId { get; set; }

            /// <summary>
            /// კატეგორიის სახელი
            /// </summary>
            [JsonProperty("cn")]
            public string CategoryName { get; set; }

            /// <summary>
            /// მშობელი(ზედა) კატეგორიის კოდი
            /// </summary>
            [JsonProperty("pi")]
            public int CategoryParentId { get; set; }

            /// <summary>
            /// მშობელი(ზედა) კატეგორიის სახელი
            /// </summary>
            [JsonProperty("pn")]
            public string CategoryParentName { get; set; }

            /// <summary>
            /// ტიპის გასაღები
            /// </summary>
            [JsonProperty("ti")]
            public short TypeId { get; set; }

            /// <summary>
            /// ტიპის სახელი
            /// </summary>
            [JsonProperty("tn")]
            public string TypeName { get; set; }

            /// <summary>
            /// დადასტურებული მერჩანტი
            /// </summary>
            [JsonProperty("mr", NullValueHandling = NullValueHandling.Ignore)]
            public string Merchant { get; set; }

        }

        /// <summary>
        /// კატეგორიზაციის შედეგი
        /// </summary>
        public class Response
        {

            /// <summary>
            /// კატეგორიზაციის ვერსია
            /// </summary>
            [JsonProperty("vr")]
            public string Version { get; set; }

            /// <summary>
            /// დაკატეგორიზებული მონაცემები
            /// </summary>
            [JsonProperty("dt")]
            public List<Outcome> Outcomes { get; set; }

        }

        public abstract class Dodo
        {
            public int MyProperty { get; set; }
            public string MyProperty2 { get; set; }

            public string OK()
            {
                return "ok";
            }
        }
        public class voo : Dodo { }

        public interface iii
        {
            public int MyProperty { get; set; }
        }

        public class jjj : iii
        {
            public int MyProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }

        class EV
        {
            public event EventHandler Event;
            public void Raise()
            {
                Event.Invoke(this, EventArgs.Empty);
                Console.WriteLine("ok, raised");
            }
        }

        static async Task Main(string[] args)
        {


            //var ddd = new voo { };
            //Console.WriteLine(ddd.OK());
            //var ok = "ok";

            //var demo = new EV();
            //demo.Event += async (a, b) =>
            //{
            //    await Task.Delay(1000);
            //    Console.WriteLine("gooch yaaa");
            //};
            //demo.Raise();

            //Console.ReadKey();
            //return;

            //https://blog.rabbitmq.com/posts/2015/04/scheduling-messages-with-rabbitmq
            // https://github.com/rabbitmq/rabbitmq-delayed-message-exchange
            // https://engineering.nanit.com/rabbitmq-retries-the-full-story-ca4cc6c5b493
            // https://olegkarasik.wordpress.com/2019/04/16/code-tip-how-to-work-with-asynchronous-event-handlers-in-c/
            Console.WriteLine("starting ...\n\n\n");



            //using (var pub = new Publisher1(new RabbitPublisherConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Name = "test-exchange-direct",
            //    Type = "direct",
            //    Queues = new List<RabbitPublisherConfiguration.Queue> {
            //        new RabbitPublisherConfiguration.Queue { Name = "dodo-direct", RoutingKeys = new List<string>{ "okok" } },
            //        new RabbitPublisherConfiguration.Queue { Name = "dudu-direct", RoutingKeys = new List<string>{ "koko" } }
            //    }
            //}))
            //{
            //    await pub.Publish(new List<string> { "OLA" }, "okok");
            //    await pub.Publish(new List<string> { "OLA" }, "koko");
            //    await pub.Publish(new List<string> { "OLA" }, "dodo");
            //}


            //using (var pub = new Publisher1(new RabbitPublisherConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Name = "test-exchange-fanout",
            //    Type = "fanout",
            //    Queues = new List<RabbitPublisherConfiguration.Queue> {
            //        new RabbitPublisherConfiguration.Queue { Name = "dodo-fanout", RoutingKeys = new List<string>{ "okok" } },
            //        new RabbitPublisherConfiguration.Queue { Name = "dudu-fanout", RoutingKeys = new List<string>{ "koko" } }
            //    }
            //}))
            //{
            //    await pub.Publish(new List<string> { "OLA" }, "hi");
            //    //await pub.Publish(new List<string> { "OLA" }, "koko");
            //    //await pub.Publish(new List<string> { "OLA" }, "dodo");
            //}



            //var iiii = 1;

            //System.Timers.Timer Timer = new System.Timers.Timer(10 * 1000) { Enabled = false };
            //Timer.Start();
            //Timer.Elapsed += (sender, eventArgs) =>
            //{
            //    System.Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-" + System.DateTime.Now.TimeOfDay.ToString());
            //};



            //System.Timers.Timer Timer2 = new System.Timers.Timer(5 * 1000) { Enabled = false };
            //Timer2.Start();
            //Timer2.Elapsed += (sender, eventArgs) =>
            //{
            //    System.Console.WriteLine("@@@restart aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-" + System.DateTime.Now.TimeOfDay.ToString());
            //    if (iiii <= 10)
            //    {

            //        Timer.Stop();
            //        Timer.Start();
            //        iiii++;
            //    }
            //};


            //using (var con = new Consumer1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Queue = "queue-fanout",
            //    Exchange = "exchange-fanout",
            //    ExchangeType = "fanout",
            //    Workers = 1
            //}))
            //{
            //    Console.WriteLine("con1");
            //}


            //using (Publisher1 pub = new Publisher1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Exchange = "exchange-fanout",
            //    ExchangeType = "fanout",
            //    RoutingKeys = new List<string> { "red", "black", "green" }
            //    //Queue = "queue-1"
            //}))
            //{
            //    for (int i = 1; i <= 10; i++)
            //    {
            //        await pub.Publish("OK-" + i, "red");
            //    }
            //    //for (int i = 1; i <= 10; i++)
            //    //{
            //    //    await pub.Publish("OK-" + i, "black");
            //    //}
            //    //for (int i = 1; i <= 10; i++)
            //    //{
            //    //    await pub.Publish("OK-" + i, "green");
            //    //}
            //}





            //using (Publisher1 pub = new Publisher1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    //Queue = "queue-fanout",
            //    Queue = "exchange-fanout",
            //    ExchangeType = "fanout",
            //    RoutingKeys = new List<string> { "red", "black", "green" }
            //    //Queue = "queue-1"
            //}))
            //{
            //    for (int i = 1; i <= 10; i++)
            //    {
            //        await pub.Publish("OK-" + i, "red");
            //    }
            //    for (int i = 1; i <= 10; i++)
            //    {
            //        await pub.Publish("OK-" + i, "black");
            //    }
            //    for (int i = 1; i <= 10; i++)
            //    {
            //        await pub.Publish("OK-" + i, "green");
            //    }
            //}


            var x = 100;

            //using (var con = new Consumer1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Queue = "test",
            //    Workers = 1
            //}))
            //{
            //    Console.WriteLine("con1");
            //}



            //using (Publisher1 direct = new Publisher1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Exchange = "exchange-direct",
            //    ExchangeType = "direct"
            //    //Queue = "queue-1"
            //}))
            //{
            //    for (int i = 1; i <= 10; i++)
            //    {
            //        await direct.Publish("OK-" + i, "");
            //    }
            //}



            //using (var con1 = new Consumer1(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Queue = "test",
            //    Workers = 1
            //}))
            //{
            //    Console.WriteLine("con1");
            //}


            //


            using (var pub2 = new PublisherAlta(new RabbitPublisherConfiguration
            {
                Hosts = new List<string> { "amqp.altasoft.ge" },
                VirtualHost = "OptioAi",
                Port = 5672, // 5672
                User = "optioai",
                Password = "optioai",
                Name = "OptioAiTestResults"
            }))
            {
                await pub2.Publish(new Response
                {
                    Version = "0.0",
                    Outcomes = new List<Outcome>
                    {
                        new Outcome {
                            Id = "333",
                            //Context = new Context {
                            //    Request = new Dictionary<string, object> { { "hi", "ola" } },
                            //    Response = new Dictionary<string, object> { { "bye", "pk" } }
                            //},
                            //Location = new Location {
                            //    Country = "georgia",
                            //    City = "tbilisi",
                            //    Address = "abashidze",
                            //    Longitude = 1.11,
                            //    Latitude = 2.22
                            //},
                            //Terminal = "t001",
                            CategoryOrigin = "merchant",
                            CategoryRule = "some-rule",
                            CategoryId = 222,
                            CategoryName = "222",
                            CategoryParentId = 111,
                            CategoryParentName = "111",
                            TypeId = 333,
                            TypeName = "333",
                            Merchant = "goodwill"
                        }
                    }
                });
            }

            //var x = 0;

            //using (var con2 = new Consumer2(new RabbitConfiguration
            //{
            //    Hosts = new List<string> { "amqp.altasoft.ge" },
            //    VirtualHost = "OptioAi",
            //    Port = 5672, // 5672
            //    UserName = "optioai",
            //    Password = "optioai",
            //    Queue = "OptioAiTestEvents"
            //}))
            //{
            //    Console.WriteLine("con2");
            //}

            //new RabbitPublisherConfiguration { Type = "", Queues = null };

            Console.WriteLine("\n\n\ndone ...");
            Console.ReadKey();
        }

    }
}
