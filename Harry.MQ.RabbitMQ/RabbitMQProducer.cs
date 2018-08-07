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
        private readonly bool isBroadcast;
        private readonly IBasicProperties basicProperties;
        private string routingKey = "";

        public RabbitMQProducer(IModel channel, string channelName, RabbitMQOptions options, bool isBroadcast)
        {
            this.channel = channel;
            this.channelName = channelName;
            this.options = options;
            this.isBroadcast = isBroadcast;

            basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            options?.Events?.OnSetBasicProperties?.Invoke(basicProperties);

            if (isBroadcast)
            {

            }
            else
            {
                //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
                var queueName = channel.QueueDeclare(queue: channelName,
                         durable: options.QueueDeclare.Durable,
                         exclusive: options.QueueDeclare.Exclusive,
                         autoDelete: options.QueueDeclare.AutoDelete,
                         arguments: options.QueueDeclare.Arguments).QueueName;

                routingKey = options?.Exchange?.RoutingKey ?? queueName;

                channel.QueueBind(queueName, channelName, routingKey);
            }
        }

        public void Publish(Message msg)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProducer));
            }

            channel.BasicPublish(exchange: channelName,
                             routingKey: routingKey,
                             basicProperties: basicProperties,
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
