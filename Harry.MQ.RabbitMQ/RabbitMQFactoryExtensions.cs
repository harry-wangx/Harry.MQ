using Harry.MQ.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public static class RabbitMQFactoryExtensions
    {
        public static IMQFactory AddRabbitMQ(this IMQFactory factory, Action<RabbitMQOptions> action)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            RabbitMQOptions options = new RabbitMQOptions();
            action?.Invoke(options);
            factory.AddProvider(new RabbitMQProvider(options));
            return factory;
        }

        public static IMQFactory AddRabbitMQ(this IMQFactory factory)
        {
            return AddRabbitMQ(factory, null);
        }
    }
}
