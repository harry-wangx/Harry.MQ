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
        private readonly RabbitMQOptions options;
        private string queueName;
        private readonly bool isBroadcast;

        public event EventHandler<ReceiveArgs> Received;

        public RabbitMQConsumer(IModel channel, string channelName, RabbitMQOptions options, bool isBroadcast)
        {
            this.channel = channel;
            this.options = options;
            //this.channelName = channelName;
            this.isBroadcast = isBroadcast;

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;

            
            if (isBroadcast)
            {
                queueName = channel.QueueDeclare(queue: "",
                         durable: false,
                         exclusive: true,
                         autoDelete: true,
                         arguments: options.QueueDeclare.Arguments).QueueName;
            }
            else
            {
                //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
                queueName = channel.QueueDeclare(queue: channelName,
                         durable: options.QueueDeclare.Durable,
                         exclusive: options.QueueDeclare.Exclusive,
                         autoDelete: options.QueueDeclare.AutoDelete,
                         arguments: options.QueueDeclare.Arguments).QueueName;
            }

            var routingKey = options?.Exchange?.RoutingKey ?? (isBroadcast ? "" : queueName);

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

#if NET451 || COREFX
            channel.BasicConsume(queue: queueName,
                     autoAck: autoAck,
                     consumer: consumer);
#elif NET45
            channel.BasicConsume(queueName, autoAck, consumer);
#else
            //防止有新增加的.net版本，且未正确实现当前方法
            //throw new NotImplementedException();
            throw
#endif
        }

        public void Ack(ReceiveArgs args)
        {
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
