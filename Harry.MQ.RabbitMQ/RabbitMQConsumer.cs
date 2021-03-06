﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.MQ.RabbitMQ
{
    public class RabbitMQConsumer : IConsumer
    {
        private readonly IModel channel;
        private volatile bool _disposed;
        private readonly EventingBasicConsumer consumer;
        private readonly RabbitMQOptions _options;
        private string queueName;

        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event EventHandler<ReceiveArgs> Received;

        /// <summary>
        /// 是否广播模式
        /// </summary>
        public bool IsBroadcast { get; private set; }

        public RabbitMQConsumer(IModel channel, string channelName, RabbitMQOptions options, bool isBroadcast)
        {
            this.channel = channel;
            this._options = options;
            this.IsBroadcast = isBroadcast;

            //默认使用通道名称作为Exchange名称
            string exchange = _options.Exchange?.Name ?? channelName;

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;

            //如果Queue不存在，定义Queue;如果已存在，则忽略此操作。此操作是幂等的。
            if (isBroadcast)
            {
                //广播模式下，默认让系统自动生成队列名称
                queueName = channel.QueueDeclare(queue: _options.QueueDeclare?.Name ?? "",
                         //广播模式下默认不持久化
                         durable: _options.QueueDeclare?.Durable == null ? false : _options.QueueDeclare.Durable.Value,
                         //广播模式下默认使用专属queue
                         exclusive: _options.QueueDeclare?.Exclusive == null ? true : _options.QueueDeclare.Exclusive.Value,
                         //广播模式下默认自动删除queue
                         autoDelete: _options.QueueDeclare?.AutoDelete == null ? true : _options.QueueDeclare.AutoDelete.Value,
                         arguments: _options.QueueDeclare.Arguments).QueueName;
            }
            else
            {
                //非广播模式下，默认使用通道名称作为队列名称
                queueName = channel.QueueDeclare(queue: _options.QueueDeclare?.Name ?? channelName,
                         //非广播模式下默认持久化
                         durable: _options.QueueDeclare?.Durable == null ? true : _options.QueueDeclare.Durable.Value,
                         //非广播模式下默认不使用专属queue
                         exclusive: _options.QueueDeclare?.Exclusive == null ? false : _options.QueueDeclare.Exclusive.Value,
                         //非广播模式下默认不自动删除queue
                         autoDelete: _options.QueueDeclare?.AutoDelete == null ? false : _options.QueueDeclare.AutoDelete.Value,
                         arguments: _options.QueueDeclare.Arguments).QueueName;
            }

            //广播模式下，默认使用空字符串作为routingKey；否则使用队列名称
            var routingKey = _options.Exchange?.RoutingKey ?? (isBroadcast ? "" : queueName);

            channel.QueueBind(queueName, exchange, routingKey);
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            if (CheckDisposed()) return;

            Message message = new Message()
            {
                Body = e.Body
            };
            Received?.Invoke(this, new ReceiveArgs(sender, e, message));
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="autoAck">是否自动发送回执(广播模式下此参数不起作用)</param>
        public void Begin(bool autoAck)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(RabbitMQConsumer));
            }

            //每次只向一个消费者发送一个消息
            channel.BasicQos(0, 1, false);

            //广播模式下不需要发送回执
#if NET451 || COREFX
            channel.BasicConsume(queue: queueName,
                     autoAck: IsBroadcast ? true : autoAck,
                     consumer: consumer);
#elif NET45
            channel.BasicConsume(queueName, (IsBroadcast ? true : autoAck), consumer);
#else
            //防止有新增加的.net版本，且未正确实现当前方法
            //throw new NotImplementedException();
            throw
#endif
        }

        /// <summary>
        /// 发送回执（广播模式下此方法不执行任何操作）
        /// </summary>
        public void Ack(ReceiveArgs args)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(RabbitMQConsumer));
            }

            //广播模式下不需要发送回执
            if (IsBroadcast) return;

            if (args == null)
                throw new ArgumentNullException(nameof(args));

            BasicDeliverEventArgs e = args.OriginArgs as BasicDeliverEventArgs;
            if (e == null)
                throw new ArgumentNullException(nameof(args.OriginArgs));

            channel.BasicAck(e.DeliveryTag, false);
        }

        /// <summary>
        /// 是否已消毁
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckDisposed() => _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    Received = null;
                    consumer.Received -= OnReceived;
                }
                catch { }

                try
                {
                    channel?.Dispose();
                }
                catch { }
            }
        }
    }
}
