﻿using System;
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
        event EventHandler<ReceiveMessage> Received;

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="autoAck">是否自动发送回执</param>
        void Begin(bool autoAck);

        /// <summary>
        /// 发送回执
        /// </summary>
        void Ack(ReceiveMessage message);
    }
}