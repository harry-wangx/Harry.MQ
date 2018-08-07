using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    public interface IConsumer : IDisposable
    {
        /// <summary>
        /// 接收到消息事件
        /// </summary>
        event EventHandler<ReceiveArgs> Received;

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="autoAck">是否自动发送回执(广播模式下此参数不起作用)</param>
        void Begin(bool autoAck);

        /// <summary>
        /// 发送回执（广播模式下此方法不执行任何操作）
        /// </summary>
        void Ack(ReceiveArgs args);
    }
}
