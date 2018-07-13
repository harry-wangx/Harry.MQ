using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Harry.MQ.RabbitMQ;

namespace Harry.MQ.Test
{
    public class RabbitMQProducerTest
    {
        [Fact]
        public void Publish()
        {
            var producer = RabbitMQTestHelper.GetProducer("hello");
            producer.Publish("hello:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.True(true);
        }


    }
}
