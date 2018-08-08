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
        private readonly string exchange;
        private readonly RabbitMQOptions _options;
        private readonly bool isBroadcast;
        private readonly IBasicProperties basicProperties;
        private string routingKey = "";

        public RabbitMQProducer(IModel channel, string channelName, RabbitMQOptions options, bool isBroadcast)
        {
            this.channel = channel;
            //使用channelName作为exchange
            this.exchange = channelName;
            this._options = options;
            this.isBroadcast = isBroadcast;

            basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            _options.Events?.OnSetBasicProperties?.Invoke(basicProperties);

            if (isBroadcast)
            {
                //广播模式下，不需要创建queue
            }
            else
            {
                //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
                var queueName = channel.QueueDeclare(queue: channelName,
                         //非广播模式下默认持久化
                         durable: _options.QueueDeclare?.Durable == null ? true : _options.QueueDeclare.Durable.Value,
                         //非广播模式下默认不使用专属queue
                         exclusive: _options.QueueDeclare?.Exclusive == null ? false : _options.QueueDeclare.Exclusive.Value,
                         //非广播模式下默认不自动删除queue
                         autoDelete: _options.QueueDeclare?.AutoDelete == null ? false : _options.QueueDeclare.AutoDelete.Value,
                         arguments: _options.QueueDeclare.Arguments).QueueName;

                //如果没有指定routingKey，直接使用queueName作为routingKey
                routingKey = _options.Exchange?.RoutingKey ?? queueName;

                channel.QueueBind(queueName, exchange, routingKey);
            }
        }

        public void Publish(Message msg)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProducer));
            }

            channel.BasicPublish(exchange: exchange,
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
