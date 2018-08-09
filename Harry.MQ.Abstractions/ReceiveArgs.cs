using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// 消息接收参数
    /// </summary>
    public class ReceiveArgs : EventArgs
    {
        public ReceiveArgs(object oSender, object oArgs, Message message)
        {
            this.OriginSender = oSender;
            this.OriginArgs = oArgs;
            this.Message = message;
        }
        /// <summary>
        /// 消息
        /// </summary>
        public Message Message { get; private set; }

        /// <summary>
        /// 源发送者
        /// </summary>
        public object OriginSender { get; private set; }

        /// <summary>
        /// 源参数
        /// </summary>
        public object OriginArgs { get; private set; }
    }
}
