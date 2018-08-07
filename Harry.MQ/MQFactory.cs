using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public class MQFactory : IMQFactory
    {
        private readonly List<IMQProvider> _lstProviders = new List<IMQProvider>();
        private volatile bool _disposed;

        #region 创建消息发布者

        //public IProducer CreateProducer(string channelName)
        //{
        //    return this.CreateProducer(channelName, false);
        //}

        public IProducer CreateProducer(string channelName, bool isBroadcast)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            channelName = channelName?.Trim();

            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentNullException(nameof(channelName));

            IProducer result = null;
            foreach (var provider in _lstProviders)
            {
                result = provider.CreateProducer(channelName,isBroadcast);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        #region 创建消息消费者
        //public IConsumer CreateConsumer(string channelName)
        //{
        //    return this.CreateConsumer(channelName, false);
        //}

        public IConsumer CreateConsumer(string channelName, bool isBroadcast)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            channelName = channelName?.Trim();

            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentNullException(nameof(channelName));

            IConsumer result = null;
            foreach (var provider in _lstProviders)
            {
                result = provider.CreateConsumer(channelName, isBroadcast);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        public IMQFactory AddProvider(IMQProvider provider)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _lstProviders.Add(provider);

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
