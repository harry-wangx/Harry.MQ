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
        public IProducer CreateProducer(string channelName, bool isBroadcast)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProvider));
            }

            EnsureConnectionFactory();
            EnsureConnection();

            var channel = GetAndInitChannel(channelName, isBroadcast);

            if (channel == null)
                return null;
            return new RabbitMQProducer(channel, channelName, _options, isBroadcast);
        }

        /// <summary>
        /// 创建消费者
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public IConsumer CreateConsumer(string channelName, bool isBroadcast)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQProvider));
            }

            EnsureConnectionFactory();
            EnsureConnection();

            var channel = GetAndInitChannel(channelName, isBroadcast);

            if (channel == null)
                return null;
            return new RabbitMQConsumer(channel, channelName, _options, isBroadcast);

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
                var func = _options?.Events?.OnCreateConnectionFactory;
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

                connection = _options?.Events?.OnCreateConnection?.Invoke(factory);

                if (connection == null)
                {
                    connection = factory.CreateConnection();
                }
            }
        }

        public virtual IModel GetAndInitChannel(string channelName, bool isBroadcast)
        {
            var canCreate = _options?.Events?.ChannelFilter?.Invoke(channelName);
            if (canCreate == false)
                return null;

            var channel = connection.CreateModel();


            //定义Exchange
            channel.ExchangeDeclare(
                channelName,
                isBroadcast ? "fanout" : "direct",
                durable: _options?.Exchange?.Durable == null ? (!isBroadcast) : _options.Exchange.Durable.Value, //改成默认保存的
                autoDelete: _options?.Exchange?.AutoDelete == null ? isBroadcast : _options.Exchange.AutoDelete.Value,
                arguments: _options?.Exchange?.Arguments);


            //if (isBroadcast)
            //{

            //}
            //else
            //{
            //    //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
            //    channel.QueueDeclare(queue: channelName,
            //             durable: _options.QueueDeclare.Durable,
            //             exclusive: _options.QueueDeclare.Exclusive,
            //             autoDelete: _options.QueueDeclare.AutoDelete,
            //             arguments: _options.QueueDeclare.Arguments);
            //}


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
