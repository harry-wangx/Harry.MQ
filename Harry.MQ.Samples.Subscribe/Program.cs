using System;

namespace Harry.MQ.Samples.Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumer = new MQFactory()
                .AddRabbitMQ()
                .CreateConsumer("hello.subscribe",true);
            consumer.Received += Consumer_Received;
            consumer.Begin(true);
            Console.WriteLine("开始监听");
            Console.ReadLine();
        }

        private static void Consumer_Received(object sender, ReceiveArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }
    }
}
