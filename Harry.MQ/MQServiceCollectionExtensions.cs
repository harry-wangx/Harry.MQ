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
        ///// <summary>
        ///// 添加MQ支持
        ///// </summary>
        ///// <param name="services"></param>
        ///// <param name="factoryAction"></param>
        ///// <returns></returns>
        //public static IServiceCollection AddMQ(this IServiceCollection services, Action<IMQFactory> factoryAction)
        //{
        //    if (services == null)
        //        throw new ArgumentNullException(nameof(services));

        //    services.AddMQ().AddSingleton(_ =>
        //    {
        //        IMQFactory factory = _.GetRequiredService<IMQFactory>();
        //        factoryAction?.Invoke(factory);
        //        return factory;
        //    });
        //    return services;
        //}

        /// <summary>
        /// 添加MQ支持
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
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