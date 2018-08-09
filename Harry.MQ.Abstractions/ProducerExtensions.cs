using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public static class ProducerExtensions
    {
        /// <summary>
        /// 使用指定编码格式发送消息(默认使用UTF8)
        /// </summary>
        /// <param name="producer"><see cref="IProducer"/></param>
        /// <param name="msg">消息内容</param>
        /// <param name="encoding">编码格式(默认使用UTF8)</param>
        /// <returns></returns>
        public static IProducer Publish(this IProducer producer, string msg, Encoding encoding=null)
        {
            if (producer == null)
                throw new ArgumentNullException(nameof(producer));

            if (string.IsNullOrEmpty(msg))
                return producer;

            encoding = encoding ?? Encoding.UTF8;

            Message message = new Message()
            {
                Body = encoding.GetBytes(msg)
            };
            producer.Publish(message);
            return producer;
        }
    }
}
