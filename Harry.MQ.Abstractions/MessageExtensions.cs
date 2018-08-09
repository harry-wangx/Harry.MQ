using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public static class MessageExtensions
    {
        /// <summary>
        /// 获取消息的文本格式（默认使用UTF8）
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string GetString(this Message message, Encoding encoding = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message.Body == null)
                return null;

            encoding = encoding ?? Encoding.UTF8;

            return encoding.GetString(message.Body);
        }

    }
}
