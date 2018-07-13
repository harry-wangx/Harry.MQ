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
    }
}
