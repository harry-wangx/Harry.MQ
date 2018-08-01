using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Harry.MQ.Test
{
    public class RabbitMQConsumerTest
    {
        ITestOutputHelper outputHelper;

        public RabbitMQConsumerTest(ITestOutputHelper output)
        {
            this.outputHelper = output;
        }

        [Fact]
        public void Receive()
        {
            var channel = RabbitMQTestHelper.GetConsumer("hello");
            channel.Received += Channel_Received;
            channel.Begin(false);
            Thread.Sleep(300);
            Assert.True(true);
        }

        int count = 0;
        private void Channel_Received(object sender, ReceiveArgs e)
        {
            outputHelper.WriteLine($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}】【{count++}】{e.Message.GetString()}");
            Thread.Sleep(100);
            ((IConsumer)sender).Ack(e);
        }

    }
}
