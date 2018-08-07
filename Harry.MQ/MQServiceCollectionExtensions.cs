#if COREFX
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Harry.MQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MQServiceCollectionExtensions
    {
        public static IServiceCollection AddMQ(this IServiceCollection services, Action<IMQFactory> factoryAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddMQ().AddSingleton(_ =>
            {

                IMQFactory factory = _.GetService<IMQFactory>();
                if (factory == null)
                    factory = new MQFactory();

                factoryAction?.Invoke(factory);
                return factory;
            });
            return services;
        }

        public static IServiceCollection AddMQ(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IMQFactory, MQFactory>();

            return services;
        }
    }
}
#endif