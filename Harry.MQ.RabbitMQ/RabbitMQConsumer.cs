using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQConsumer : IConsumer
    {
        private readonly IModel channel;
        private volatile bool _disposed;
        private readonly EventingBasicConsumer consumer;
        private readonly RabbitMQOptions _options;
        private string queueName;
        private readonly bool isBroadcast;

        public event EventHandler<ReceiveArgs> Received;

        public RabbitMQConsumer(IModel channel, string channelName, RabbitMQOptions options, bool isBroadcast)
        {
            this.channel = channel;
            this._options = options;
            this.isBroadcast = isBroadcast;

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;

            //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
            if (isBroadcast)
            {
                queueName = channel.QueueDeclare(queue: "",
                         //广播模式下默认不持久化
                         durable: _options.QueueDeclare?.Durable == null ? false : _options.QueueDeclare.Durable.Value,
                         //广播模式下默认使用专属queue
                         exclusive: _options.QueueDeclare?.Exclusive == null ? true : _options.QueueDeclare.Exclusive.Value,
                         //广播模式下默认自动删除queue
                         autoDelete: _options.QueueDeclare?.AutoDelete == null ? true : _options.QueueDeclare.AutoDelete.Value,
                         arguments: _options.QueueDeclare.Arguments).QueueName;
            }
            else
            {
                queueName = channel.QueueDeclare(queue: channelName,
                         //非广播模式下默认持久化
                         durable: _options.QueueDeclare?.Durable == null ? true : _options.QueueDeclare.Durable.Value,
                         //非广播模式下默认不使用专属queue
                         exclusive: _options.QueueDeclare?.Exclusive == null ? false : _options.QueueDeclare.Exclusive.Value,
                         //非广播模式下默认不自动删除queue
                         autoDelete: _options.QueueDeclare?.AutoDelete == null ? false : _options.QueueDeclare.AutoDelete.Value,
                         arguments: _options.QueueDeclare.Arguments).QueueName;
            }

            var routingKey = _options.Exchange?.RoutingKey ?? (isBroadcast ? "" : queueName);

            channel.QueueBind(queueName, channelName, routingKey);
        }

        protected virtual void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            if (_disposed) return;

            Message message = new Message()
            {
                Body = e.Body
            };
            Received?.Invoke(this, new ReceiveArgs(sender, e, message));
        }

        public void Begin(bool autoAck)
        {
            //每次只向一个消费者发送一个消息
            channel.BasicQos(0, 1, false);

            //广播模式下不需要发送回执
#if NET451 || COREFX
            channel.BasicConsume(queue: queueName,
                     autoAck: isBroadcast ? true : autoAck,
                     consumer: consumer);
#elif NET45
            channel.BasicConsume(queueName, (isBroadcast ? true : autoAck), consumer);
#else
            //防止有新增加的.net版本，且未正确实现当前方法
            //throw new NotImplementedException();
            throw
#endif
        }

        public void Ack(ReceiveArgs args)
        {
            //广播模式下不需要发送回执
            if (isBroadcast) return;

            if (args == null)
                throw new ArgumentNullException(nameof(args));

            BasicDeliverEventArgs e = args.OriginArgs as BasicDeliverEventArgs;
            if (e == null)
                throw new ArgumentNullException(nameof(args.OriginArgs));

            channel.BasicAck(e.DeliveryTag, false);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    Received = null;
                    consumer.Received -= OnReceived;
                }
                catch { }

                try
                {
                    channel?.Dispose();
                }
                catch { }
            }
        }
    }
}
