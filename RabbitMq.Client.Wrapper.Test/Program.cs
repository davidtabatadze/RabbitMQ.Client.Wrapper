using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMq.Client.Wrapper.Test
{
    class Program
    {

        //public abstract class Dodo
        //{
        //    public int MyProperty { get; set; }
        //    public string MyProperty2 { get; set; }

        //    public string OK()
        //    {
        //        return "ok";
        //    }
        //}
        //public class voo : Dodo { }

        //public interface iii
        //{
        //    public int MyProperty { get; set; }
        //}

        //public class jjj : iii
        //{
        //    public int MyProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //}

        //class EV
        //{
        //    public event EventHandler Event;
        //    public void Raise()
        //    {
        //        Event.Invoke(this, EventArgs.Empty);
        //        Console.WriteLine("ok, raised");
        //    }
        //}

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

           
            Console.WriteLine("starting ...\n\n\n");

            //var aaa = new List<uint> { 1, 2, 3 };
            //var next = aaa.FirstOrDefault(i => i > 5);
            //var bbb = 0;
            //var fff = -bbb;


            //Dictionary<string, long> llll = new Dictionary<string, long> { { "ok", -9 } };
            //var delay = (long)0;
            //if (llll != null)
            //{
            //    llll.TryGetValue("ok", out delay);
            //}
            //var walodjwaod = 0;



            try
            {


                //using (var con = new RconsumerDead(new RabbitConsumerConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue-dead"
                //}))
                //{

                //}

                //using (var con = new Rconsumer(new RabbitConsumerConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue",
                //    //RetryIntervals = new List<ulong> { 10000, 20000 },
                //    Workers = 5,
                //    //BatchSize = 5
                //}, new RpublisherSuccess(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-success",
                //})))
                //{

                //}


                //using (var pub = new RpublisherSpoiled(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue"
                //}))
                //{
                //    for (int i = 1; i <= 1; i++)
                //    {
                //        await pub.Publish("abrakatabra" + i);
                //    }
                //}

                //using (var pub = new Rpublisher(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue"
                //}))
                //{
                //    var msg = new List<TestMessage> { };
                //    var cnt = 0;
                //    for (int i = 1; i <= 1; i++)
                //    {
                //        msg.Add(new TestMessage { Id = i, Name = "dt" + i });
                //        if (msg.Count == 1)
                //        {
                //            cnt += msg.Count;
                //            Console.WriteLine("messages " + cnt);
                //            System.Threading.Thread.Sleep(new Random().Next(0, 500));
                //            await pub.Publish(msg);
                //            msg = new List<TestMessage> { };
                //        }
                //    }
                //}

                //using (var pub = new RpublisherSpoiled(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue"
                //}))
                //{
                //    for (int i = 1; i <= 1000; i++)
                //    {
                //        await pub.Publish("abrakatabra" + i);
                //    }
                //}

                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "rabbit-queue-retry",
                //    Type = RabbitRetry.DelayExchangeType,
                //    Arguments = RabbitRetry.DelayExchangeArguments
                //}))
                //{
                //    await del.Publish("del-olaaa");
                //    await del.Publish("del-bolaaa", new Dictionary<string, object> { { "x-delay", 10000 } });
                //    await del.Publish("del-kolaaa", new Dictionary<string, object> { { "x-delay", 20000 } });
                //}




                //using (var coo = new RabbitConsumer<string>(new RabbitConsumerConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "delay-queue",
                //    Bindings = new Dictionary<string, string> { { "delay-queue", "delay-fanout" } }
                //}))
                //{

                //}

                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "delay-fanout",
                //    Type = "x-delayed-message",
                //    //Routings = new List<string> { "dtest" },
                //    Arguments = new Dictionary<string, object> { { "x-delayed-type", "fanout" } },
                //    Headers = new Dictionary<string, object> { { "x-delay", 10000 } }
                //}))
                //{
                //    await del.Publish("del-olaaa");
                //    await del.Publish("del-bolaaa");
                //    await del.Publish("del-kolaaa", new Dictionary<string, object> { { "x-delay", 30000 } });
                //}

                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "fun-fun",
                //    Type = "fanout"
                //}))
                //{
                //    await del.Publish("del-olaaa");
                //    await del.Publish("del-bolaaa");
                //    await del.Publish("del-kolaaa", new Dictionary<string, object> { { "x-delay", 30000 } });
                //}


                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "ex-delay-test",
                //    Type = "x-delayed-message",
                //    //Routings = new List<string> { "dtest" },
                //    Arguments = new Dictionary<string, object> { { "x-delayed-type", "fanout" } },
                //    Headers = new Dictionary<string, object> { { "x-delay", 10000 } }
                //}))
                //{
                //    await del.Publish("del-olaaa");
                //    await del.Publish("del-bolaaa");
                //    await del.Publish("del-kolaaa", new Dictionary<string, object> { { "x-delay", 30000 } });
                //}

                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "ex-delay-concurent",
                //    Type = "direct",
                //    Routings = new List<string> { "gooo" }
                //}))
                //{
                //    await del.Publish("concurent - 1");
                //}

                //using (var del = new RabbitPublisher<string>(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "ex-delay-queue"
                //}))
                //{
                //    await del.Publish("priamoi");
                //}

                //Console.WriteLine("ok");
                //return;

                //using (var con = new Rconsumer(new RabbitConsumerConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "exchange-fun-queue-dle",
                //    Bindings = new Dictionary<string, string> { { "-", "exchange-fun" } },
                //    BatchSize = 2,
                //    Workers = 0,
                //    RetryIntervals = new List<ulong> { },
                //    Arguments = new Dictionary<string, object> { { "x-dead-letter-exchange", "some.exchange.name" } }
                //}))
                //{
                //    var dt = 0;
                //}

                //using (var pub = new Publisher1(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "exchange-fun",
                //    Type = "fanout"
                //}))
                //{
                //    for (int i = 0; i < 2; i++)
                //    {
                //        // await pub.Publish("ola" + i);
                //        //await pub.Publish((string)null);
                //        //await pub.Publish(string.Empty);
                //        //await pub.Publish(new List<string> { "" });
                //        //await pub.Publish(new List<string> { "   " });
                //        //await pub.Publish(new List<string> { "-" });
                //    }
                //}

                //using (var pub = new Publisher1(new RabbitPublisherConfiguration
                //{
                //    Hosts = new List<string> { "localhost" },
                //    Name = "test-exchange-direct",
                //    Type = "direct",
                //    Queues = new List<RabbitPublisherConfiguration.Queue> {
                //        new RabbitPublisherConfiguration.Queue { Name = "test-exchange-direct-queue1", RoutingKeys = new List<string>{ "" } },
                //        new RabbitPublisherConfiguration.Queue { Name = "test-exchange-direct-queue2", RoutingKeys = new List<string>{ "" } }
                //    }
                //}))
                //{
                //    await pub.Publish(new List<string> { "OLA" }, "test-exchange-direct-queue1");
                //    await pub.Publish(new List<string> { "OLA" }, "test-exchange-direct-queue2");
                //}




            }
            catch (Exception e)
            {
                Console.WriteLine("!!! exception: " + e.Message);
            }



















            //using (var con = new Consumer1(new RabbitConsumerConfiguration
            //{
            //    Hosts = new List<string> { "localhost" },
            //    Name = "dodo-direct",
            //    Workers = 1
            //}))
            //{
            //    Console.WriteLine("con1");
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


            //var x = 0;





            //new RabbitPublisherConfiguration { Type = "", Queues = null };

            Console.WriteLine("\n\n\ndone ...");
            Console.ReadKey();
        }

    }
}
