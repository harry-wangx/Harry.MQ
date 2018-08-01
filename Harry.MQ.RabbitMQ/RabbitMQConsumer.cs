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
        private readonly string channelName;

        public event EventHandler<ReceiveArgs> Received;

        public RabbitMQConsumer(IModel channel, string channelName, RabbitMQOptions options)
        {
            this.channel = channel;
            this.options = options;
            this.channelName = channelName;

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;
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
#if NET451 || COREFX
            channel.BasicConsume(queue: channelName,
                     autoAck: autoAck,
                     consumer: consumer);
#elif NET45
            channel.BasicConsume(channelName, autoAck, consumer);
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
