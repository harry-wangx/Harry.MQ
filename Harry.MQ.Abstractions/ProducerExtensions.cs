using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public static class ProducerExtensions
    {
        public static IProducer Publish(this IProducer producer, string msg)
        {
            if (producer == null)
                throw new ArgumentNullException(nameof(producer));

            if (string.IsNullOrWhiteSpace(msg))
                return producer;

            return producer.Publish(msg, MQDefaults.DefaultEncoding);
        }

        public static IProducer Publish(this IProducer producer, string msg, Encoding encoding)
        {
            if (producer == null)
                throw new ArgumentNullException(nameof(producer));

            if (string.IsNullOrWhiteSpace(msg))
                return producer;

            encoding = encoding ?? MQDefaults.DefaultEncoding;

            Message message = new Message()
            {
                Body = encoding.GetBytes(msg)
            };
            producer.Publish(message);
            return producer;
        }
    }
}
