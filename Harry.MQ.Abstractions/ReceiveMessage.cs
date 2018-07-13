using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public class ReceiveMessage:Message
    {
        public ReceiveMessage(IConsumer sender,object oSender,object oArgs)
        {
            this.Sender = sender;
            this.OriginSender = oSender;
            this.OriginArgs = oArgs;
        }
        public IConsumer Sender { get; private set; }

        public object OriginSender { get; private set; }

        public object OriginArgs { get;private set; }
    }
}
