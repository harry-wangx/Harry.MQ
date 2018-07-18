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
            services.TryAddSingleton(_ =>
            {
                IMQFactory factory = new MQFactory();
                factoryAction?.Invoke(factory);
                return factory;
            });
            return services;
        }
    }
}
#endif