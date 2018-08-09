using System;
using System.Threading;

namespace Harry.MQ.Samples.Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = CreateMQFactory();


            ////以广播模式监听
            //var consumer = factory.CreateConsumer("hello.subscribe", true);
            //consumer.Received += Consumer_Received;
            //consumer.Begin(false);
            //Console.WriteLine("开始监听");
            //Console.ReadLine();
            //consumer.Dispose()


            //工作队列模式
            var consumer = factory.CreateConsumer("hello", false);
            consumer.Received += Consumer_Received;
            consumer.Begin(true);
            Console.WriteLine("开始监听");

            var producer = factory.CreateProducer("hello", false);
            for (int i = 0; i < 10; i++)
            {
                producer.Publish(i.ToString());
            }
            Console.ReadLine();
            consumer.Dispose();
            producer.Dispose();


            factory.Dispose();
        }

        private static IMQFactory CreateMQFactory()
        {
            return new MQFactory()
                .AddRabbitMQ(options =>
                {
                    options.Events.OnCreateConnectionFactory = f =>
                    {
                        //RabbitMQ设置
                        f.HostName = "localhost";
                        f.Port = 5672;
                        f.UserName = "guest";
                        f.Password = "guest";
                        return f;
                    };
                });
        }
        private static void Consumer_Received(object sender, ReceiveArgs e)
        {
            Console.WriteLine(e.Message.GetString());
        }
    }
}
