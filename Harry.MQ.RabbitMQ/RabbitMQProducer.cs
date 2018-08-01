using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQProducer : IProducer
    {
        private readonly IModel channel;
        private volatile bool _disposed;
        private readonly string channelName;
        private readonly RabbitMQOptions options;

        public RabbitMQProducer(IModel channel, string channelName, RabbitMQOptions options)
        {
            this.channel = channel;
            this.channelName = channelName;
            this.options = options;
        }

        public void Publish(Message msg)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProducer));
            }

            channel.BasicPublish(exchange: options.Exchange,
                                 routingKey: channelName,
                                 basicProperties: options.BasicProperties,
                                 body: msg.Body);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                try
                {
                    channel?.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}
