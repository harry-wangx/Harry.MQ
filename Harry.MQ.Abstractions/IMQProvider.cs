using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public interface IMQProvider : IDisposable
    {
        IProducer CreateProducer(string channelName);

        IConsumer CreateConsumer(string channelName);
    }
}
