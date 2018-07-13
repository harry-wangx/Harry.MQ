using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public static class MessageExtensions
    {
        public static string GetString(this Message message, Encoding encoding)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (message.Body == null)
                return null;

            return encoding.GetString(message.Body);
        }

        public static string GetString(this Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return message.GetString(MQDefaults.DefaultEncoding);
        }
    }
}
