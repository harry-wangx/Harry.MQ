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
            for (int i = 0; i < 100; i++)
            {
                producer.Publish("hello:" + i.ToString());
            }

            Assert.True(true);
        }

        [Fact]
        public void PublishForSubscribe()
        {
            var producer = RabbitMQTestHelper.GetProducer("hello.subscribe",true);
            producer.Publish("hello:" + DateTime.Now.ToString());

            Assert.True(true);
        }

        [Fact]
        public void PublishMail()
        {
            var producer = RabbitMQTestHelper.GetProducer("send.email");

            producer.Publish("{'subject':'测试邮件','body':'邮件内容" + DateTime.Now.ToString() + "','address':'harry_w@foxmail.com'}");

            Assert.True(true);
        }
    }
}
