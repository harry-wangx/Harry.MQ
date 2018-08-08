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

            //options.QueueDeclare = options.QueueDeclare ?? new RabbitMQOptions.QueueDeclareOptions();
            //options.Events = options.Events ?? new RabbitMQOptions.EventsOptions();
            //options.Exchange = options.Exchange ?? new RabbitMQOptions.ExchangeOptions();

            factory.AddProvider(new RabbitMQProvider(options));
            return factory;
        }

        public static IMQFactory AddRabbitMQ(this IMQFactory factory)
        {
            return AddRabbitMQ(factory, null);
        }
    }
}
