using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.Test
{
    public static class RabbitMQTestHelper
    {
        public static IProducer GetProducer(string channelName, bool isBroadcast=false)
        {
            return GetMQFactory().CreateProducer(channelName, isBroadcast);
        }

        public static IConsumer GetConsumer(string channelName, bool isBroadcast=false)
        {
            return GetMQFactory().CreateConsumer(channelName, isBroadcast);
        }

        public static IMQFactory GetMQFactory()
        {
            return new MQFactory().AddRabbitMQ();
        }
    }
}
