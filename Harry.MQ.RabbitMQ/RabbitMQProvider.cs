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
        /// 创建发布者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
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
        /// 创建消息消费者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
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

        /// <summary>
        /// 配置ConnectionFactory
        /// </summary>
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
                var func = _options.Events?.OnCreateConnectionFactory;
                if (func != null)
                {
                    factory = func.Invoke((ConnectionFactory)factory);
                }
            }
        }

        /// <summary>
        /// 配置Connection
        /// </summary>
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

                connection = _options.Events?.OnCreateConnection?.Invoke(factory);

                if (connection == null)
                {
                    connection = factory.CreateConnection();
                }
            }
        }

        /// <summary>
        /// 生成IModel
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="isBroadcast"></param>
        /// <returns></returns>
        public virtual IModel GetAndInitChannel(string channelName, bool isBroadcast)
        {
            //使用channelName作为exchange的名称
            string exchange = channelName;

            var canCreate = _options.Events?.ChannelFilter?.Invoke(channelName);
            //如果返回null或true,生成model；否则返回null
            if (canCreate == false)
                return null;

            var channel = connection.CreateModel();


            //定义Exchange
            channel.ExchangeDeclare(
                exchange,
                isBroadcast ? "fanout" : "direct",
                //广播模式下默认不持久化
                durable: _options.Exchange?.Durable == null ? (!isBroadcast) : _options.Exchange.Durable.Value, 
                //广播模式下默认自动删除
                autoDelete: _options.Exchange?.AutoDelete == null ? isBroadcast : _options.Exchange.AutoDelete.Value,
                arguments: _options.Exchange?.Arguments);

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
