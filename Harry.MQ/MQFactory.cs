using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// MQ工厂类
    /// </summary>
    public class MQFactory : IMQFactory
    {
        private readonly List<IMQProvider> _lstProviders = new List<IMQProvider>();
        private readonly Dictionary<string, IProducer> _dicProducer = new Dictionary<string, IProducer>(StringComparer.Ordinal);
        private readonly Dictionary<string, IConsumer> _dicConsumer = new Dictionary<string, IConsumer>(StringComparer.Ordinal);
        private volatile bool _disposed;

        private readonly object _sync = new object();

        public MQFactory() { }

        public MQFactory(IEnumerable<IMQProvider> providers)
        {
            _lstProviders.AddRange(providers);
        }

        /// <summary>
        /// 创建发布者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
        /// <returns></returns>
        public IProducer CreateProducer(string channelName, bool isBroadcast)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }
            channelName = channelName?.Trim();

            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentNullException(nameof(channelName));

            lock (_sync)
            {
                if (!_dicProducer.TryGetValue(channelName, out var result) || result.CheckDisposed())
                {
                    result = null;
                    foreach (var provider in _lstProviders)
                    {
                        result = provider.CreateProducer(channelName, isBroadcast);
                        if (result != null)
                        {
                            _dicProducer[channelName] = result;
                            return result;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 创建消息消费者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
        /// <returns></returns>
        public IConsumer CreateConsumer(string channelName, bool isBroadcast)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            channelName = channelName?.Trim();

            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentNullException(nameof(channelName));

            //lock (_sync)
            //{
            //    if (!_dicConsumer.TryGetValue(channelName, out var result) || result.CheckDisposed())
            //    {
            //        result = null;
            //        foreach (var provider in _lstProviders)
            //        {
            //            result = provider.CreateConsumer(channelName, isBroadcast);
            //            if (result != null)
            //            {
            //                _dicConsumer[channelName] = result;
            //                return result;
            //            }
            //        }
            //    }
            //    return result;
            //}

            //消费者不使用缓存，想不出使用缓存的理由
            IConsumer result = null;
            foreach (var provider in _lstProviders)
            {
                result = provider.CreateConsumer(channelName, isBroadcast);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 添加 <see cref="IMQProvider">IMQProvider</see>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public IMQFactory AddProvider(IMQProvider provider)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_sync)
            {
                _lstProviders.Add(provider);
            }
            return this;
        }

        protected virtual bool CheckDisposed() => _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            foreach (var provider in _lstProviders)
            {
                try
                {
                    provider.Dispose();
                }
                catch
                {
                }
            }

        }

    }
}
