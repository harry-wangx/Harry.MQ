﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public class MQFactory : IMQFactory
    {
        private readonly List<IMQProvider> _lstProviders = new List<IMQProvider>();
        private volatile bool _disposed;

        public IProducer CreateProducer(string channel)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            IProducer result = null;
            foreach (var provider in _lstProviders)
            {
                result = provider.CreateProducer(channel);
                if (result != null)
                    return result;
            }
            return null;
        }

        public IConsumer CreateConsumer(string channel)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            IConsumer result = null;
            foreach (var provider in _lstProviders)
            {
                result = provider.CreateConsumer(channel);
                if (result != null)
                    return result;
            }
            return null;
        }

        public IMQFactory AddProvider(IMQProvider provider)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MQFactory));
            }

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _lstProviders.Add(provider);

            return this;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
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
}
