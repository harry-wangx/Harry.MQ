using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;


namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQProvider : IMQProvider
    {
        private readonly RabbitMQOptions _options;
        private IConnectionFactory factory;
        private IConnection connection;
        private readonly object locker = new object();
        private volatile bool _disposed;

        public RabbitMQProvider(RabbitMQOptions options)
        {
            this._options = options;
        }

        /// <summary>
        /// 创建生产者
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public IProducer CreateProducer(string channelName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProvider));
            }

            EnsureConnectionFactory();
            EnsureConnection();

            var channel = GetAndInitChannel(channelName);

            if (channel == null)
                return null;
            return new RabbitMQProducer(channel, channelName, _options);
        }

        /// <summary>
        /// 创建消费者
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public IConsumer CreateConsumer(string channelName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProvider));
            }

            EnsureConnectionFactory();
            EnsureConnection();

            var channel = GetAndInitChannel(channelName);

            if (channel == null)
                return null;
            return new RabbitMQConsumer(channel, channelName, _options);

        }

        private void EnsureConnectionFactory()
        {
            if (factory != null)
            {
                return;
            }
            lock (locker)
            {
                if (factory != null)
                {
                    return;
                }
                factory = new ConnectionFactory() { HostName = "localhost" };
                var func = _options?.OnCreateConnectionFactory;
                if (func != null)
                {
                    factory = func.Invoke((ConnectionFactory)factory);
                }
            }
        }

        private void EnsureConnection()
        {
            if (connection != null && connection.IsOpen)
            {
                return;
            }
            lock (locker)
            {
                if (connection != null && connection.IsOpen)
                {
                    return;
                }

                connection?.Dispose();
                connection = null;

                connection = _options.OnCreateConnection?.Invoke(factory);

                if (connection == null)
                {
                    connection = factory.CreateConnection();
                }
            }
        }

        public virtual IModel GetAndInitChannel(string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                channelName = MQDefaults.DefaultChannelName;
            }

            var canCreate = _options.ChannelFilter?.Invoke(channelName);
            if (canCreate == false)
                return null;

            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: channelName,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            return channel;
        }

        public void Dispose()
        {

            if (!_disposed)
            {
                _disposed = true;

                try
                {
                    connection?.Dispose();
                }
                catch
                {
                }
                connection = null;
                factory = null;
            }

        }


    }
}
