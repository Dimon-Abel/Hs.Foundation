using Hs.Foundation.RabbitMQ.Options;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Hs.Foundation.RabbitMQ.Common
{
    #region RabbitMQ通道创建策略

    /// <summary>
    /// RabbitMQ通道创建策略
    /// </summary>
    public class ChannelPooledObjectPolicy : IPooledObjectPolicy<ChannelObject>
    {
        #region 私有变量

        /// <summary>
        /// 配置对象
        /// </summary>
        private readonly RabbitMQOptions rabbitMQOptions;

        /// <summary>
        /// 消费者配置对象
        /// </summary>
        private readonly ConsumerOptions consumerOptions;

        /// <summary>
        /// 生产者配置对象
        /// </summary>
        private readonly ProducerOptions producerOptions;

        /// <summary>
        /// 连接工厂
        /// </summary>
        private readonly ConnectionFactory connectionFactory;

        /// <summary>
        /// 连接对象集合
        /// </summary>
        private readonly List<ConnectionObject> connectionObjects;

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

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rabbitMQOptions">配置对象</param>
        /// <param name="consumerOptions">消费者配置</param>
        /// <param name="producerOptions">生产者配置</param>
        /// <param name="loggerFactory">日志工厂对象</param>
        public ChannelPooledObjectPolicy(IOptions<RabbitMQOptions> rabbitMQOptions, IOptions<ConsumerOptions> consumerOptions, IOptions<ProducerOptions> producerOptions, ILoggerFactory loggerFactory)
        {
            this.rabbitMQOptions = rabbitMQOptions?.Value ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
            this.producerOptions = producerOptions?.Value ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
            this.consumerOptions = consumerOptions?.Value ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            logger = this.loggerFactory.CreateLogger<ChannelPooledObjectPolicy>();
            connectionObjects = new List<ConnectionObject>();
            connectionFactory = new ConnectionFactory
            {
                UserName = this.rabbitMQOptions.UserName,
                Password = this.rabbitMQOptions.Password,
                VirtualHost = this.rabbitMQOptions.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                ClientProvidedName = this.rabbitMQOptions.ExchangeName,
            };
        }

        #endregion

        #region 创建通道对象

        /// <summary>
        /// 创建通道对象
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ChannelObject Create()
        {
            if (connectionObjects.Count < rabbitMQOptions.ConnectionPoolSize)
            {
                var connection = connectionFactory.CreateConnection(rabbitMQOptions.EndPoints);
                var connectionObject = new ConnectionObject(connection, rabbitMQOptions, consumerOptions, producerOptions, loggerFactory);
                connectionObjects.Add(connectionObject);
                return connectionObject.GetChannel();
            }

            if (++counter >= connectionObjects.Count)
                counter = 0;

            return connectionObjects[counter].GetChannel();
        }

        #endregion

        #region 归还对象

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Return(ChannelObject obj)
            => true;

        #endregion
    }

#endregion
}