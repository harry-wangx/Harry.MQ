using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.Test
{
    public static class RabbitMQTestHelper
    {
        public static IProducer GetProducer(string channelName)
        {
            return GetMQFactory().CreateProducer(channelName);
        }

        public static IConsumer GetConsumer(string channelName)
        {
            return GetMQFactory().CreateConsumer(channelName);
        }

        public static IMQFactory GetMQFactory()
        {
            return new MQFactory().AddRabbitMQ();
        }
    }
}
