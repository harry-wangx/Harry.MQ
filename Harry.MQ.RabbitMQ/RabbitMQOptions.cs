using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQOptions
    {
        public EventsOptions Events { get; set; } = new EventsOptions();

        public QueueDeclareOptions QueueDeclare { get; set; } = new QueueDeclareOptions();

        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public class QueueDeclareOptions
        {
            /// <summary>
            /// 是否持久化
            /// </summary>
            public bool Durable { get; set; } = true;

            /// <summary>
            /// 是否是排它性队列
            /// </summary>
            public bool Exclusive { get; set; }

            /// <summary>
            /// 当没有任何消费者使用时，是否自动删除该队列
            /// </summary>
            public bool AutoDelete { get; set; }

            public Dictionary<string, object> Arguments { get; set; }
        }

        public class EventsOptions
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
            /// 通道过滤器，是否支持当前通道名称,如果支持，返回true
            /// </summary>
            public Func<string, bool> ChannelFilter { get; set; } = _ => true;
        }

        public class ExchangeOptions
        {
            /// <summary>
            /// Exchange名称
            /// </summary>
            public string Name { get; set; } = "";

            public IBasicProperties BasicProperties { get; set; } =
                new BasicProperties()
                {
                    //消息类型
                    ContentType = "text/plain",
                    //1:nonpersistent 2:persistent 默认持久化
                    DeliveryMode = 2
                };
        }
    }
}
