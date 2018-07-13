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

        public event EventHandler<ReceiveMessage> Received;

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

            ReceiveMessage message = new ReceiveMessage(this, sender, e)
            {
                Body = e.Body
            };
            Received?.Invoke(this, message);
        }

        public void Begin(bool autoAck)
        {
            channel.BasicConsume(queue: channelName,
                                 autoAck: autoAck,
                                 consumer: consumer);
        }

        public void Ack(ReceiveMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            BasicDeliverEventArgs e = message.OriginArgs as BasicDeliverEventArgs;
            if (e == null)
                throw new ArgumentNullException(nameof(message.OriginArgs));
            channel.BasicAck(e.DeliveryTag,false);
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
