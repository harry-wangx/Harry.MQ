using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// 消息生产者接口
    /// </summary>
    public interface IProducer : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        void Publish(Message msg);

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
