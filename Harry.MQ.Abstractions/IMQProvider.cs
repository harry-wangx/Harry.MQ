using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ
{
    public interface IMQProvider : IDisposable
    {
        ///// <summary>
        ///// 创建发布者
        ///// </summary>
        ///// <param name="channelName">通道名称</param>
        ///// <returns></returns>
        //IProducer CreateProducer(string channelName);

        /// <summary>
        /// 创建发布者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播</param>
        /// <returns></returns>
        IProducer CreateProducer(string channelName, bool isBroadcast);

        /// <summary>
        /// 创建消息消费者
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="isBroadcast">是否广播</param>
        /// <returns></returns>
        IConsumer CreateConsumer(string channelName, bool isBroadcast);
    }
}
