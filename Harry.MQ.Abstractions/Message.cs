using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// 消息类
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public byte[] Body { get; set; }

        public override string ToString()
        {
            return this.GetString();
        }

        public string ToString(Encoding encoding)
        {
            return this.GetString(encoding);
        }
    }
}
