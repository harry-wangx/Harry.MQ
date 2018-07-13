using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public interface IMQFactory: IDisposable
    {
        IProducer CreateProducer(string channelName);

        IConsumer CreateConsumer(string channelName);

        IMQFactory AddProvider(IMQProvider provider);
    }
}
