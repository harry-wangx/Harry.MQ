using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    /// <summary>
    /// 工厂接口
    /// </summary>
    public interface IMQFactory: IDisposable
    {
        /// <summary>
        /// 创建发布者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
        /// <returns></returns>
        IProducer CreateProducer(string channelName, bool isBroadcast);

        /// <summary>
        /// 创建消息消费者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播模式</param>
        /// <returns></returns>
        IConsumer CreateConsumer(string channelName, bool isBroadcast);

        /// <summary>
        /// 添加 <see cref="IMQProvider">IMQProvider</see>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        IMQFactory AddProvider(IMQProvider provider);
    }
}
