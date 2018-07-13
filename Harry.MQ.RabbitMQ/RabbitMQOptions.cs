using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQOptions 
    {
        /// <summary>
        /// 创建ConnectionFactory 时会尝试调用此函数
        /// </summary>
        public Func<ConnectionFactory, IConnectionFactory> OnCreateConnectionFactory { get; set; } = _ => _;

        /// <summary>
        /// 创建IConnection的时候会尝试调用此函数
        /// </summary>
        public Func<IConnectionFactory, IConnection> OnCreateConnection { get; set; }

        /// <summary>
        /// 通道过滤器，是否支持当前通道名称
        /// </summary>
        public Func<string, bool> ChannelFilter { get; set; } = _ => true;
    }
}
