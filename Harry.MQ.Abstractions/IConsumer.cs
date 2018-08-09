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
        /// 接收消息事件
        /// </summary>
        event EventHandler<ReceiveArgs> Received;

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="autoAck">是否自动发送回执</param>
        void Begin(bool autoAck);

        /// <summary>
        /// 发送回执
        /// </summary>
        void Ack(ReceiveArgs args);

        /// <summary>
        /// 是否广播模式
        /// </summary>
        bool IsBroadcast { get; }

        /// <summary>
        /// 是否已消毁
        /// </summary>
        /// <returns></returns>
        bool CheckDisposed();
    }
}
