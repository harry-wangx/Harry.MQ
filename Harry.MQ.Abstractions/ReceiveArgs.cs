using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
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

        public object OriginSender { get; private set; }

        public object OriginArgs { get; private set; }
    }
}
