using Hs.Foundation.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hs.Foundation.RabbitMQ.Common
{
    #region 连接对象

    /// <summary>
    /// 连接对象
    /// </summary>
    public class ConnectionObject
    {
        #region 私有变量

        /// <summary>
        /// 消息接收推送通道
        /// </summary>
        private readonly List<ChannelObject> channels;

        /// <summary>
        /// 连接对象
        /// </summary>
        private readonly IConnection connection;

        /// <summary>
        /// 计数器
        /// </summary>
        private int counter = -1;

        /// <summary>
        /// 日志工厂
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region 属性

        /// <summary>
        /// 配置对象
        /// </summary>
        public RabbitMQOptions RabbitMQOptions { get; }

        /// <summary>
        /// 消费者配置对象
        /// </summary>
        public ConsumerOptions ConsumerOptions { get; }

        /// <summary>
        /// 生产者配置
        /// </summary>
        public ProducerOptions ProducerOptions { get; }

        /// <summary>
        /// 管道总数
        /// </summary>
        public int ChannelCount => channels.Count;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="options">配置对象</param>
        /// <param name="consumerOptions">消费者配置对象</param>
        /// <param name="producerOptions">生产者配置对象</param>
        /// <param name="loggerFactory">日志工厂</param>
        public ConnectionObject(IConnection connection, RabbitMQOptions options, ConsumerOptions consumerOptions, ProducerOptions producerOptions, ILoggerFactory loggerFactory)
        {
            channels = new List<ChannelObject>();
            this.connection = connection;
            RabbitMQOptions = options;
            ConsumerOptions = consumerOptions;
            ProducerOptions = producerOptions;
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            logger = loggerFactory.CreateLogger<ConnectionObject>();
        }

        #endregion

        #region 获取Channel

        /// <summary>
        /// 获取通道对象
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ChannelObject GetChannel()
        {
            if (channels.Count < RabbitMQOptions.ChannelPoolSize)
            {
                var channelObject = new ChannelObject(this, connection.CreateModel(), loggerFactory.CreateLogger<ChannelObject>());
                channels.Add(channelObject);
                return channelObject;
            }


            if (++counter >= channels.Count())
                counter = 0;

            return channels[counter];
        }

        #endregion
    }

    #endregion
}