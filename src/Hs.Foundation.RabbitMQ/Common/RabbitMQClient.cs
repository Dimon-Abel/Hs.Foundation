using Hs.Foundation.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Hs.Foundation.RabbitMQ.Common
{
    #region RabbitMQ客户端

    /// <summary>
    /// RabbitMQ客户端
    /// </summary>
    public class RabbitMQClient : IRabbitMQClient
    {
        #region 私有变量

        /// <summary>
        /// 通道对象池
        /// </summary>
        private readonly DefaultObjectPool<ChannelObject> connectionPool;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rabbitMQOptions">配置对象</param>
        /// <param name="consumerOptions">消费者配置对象</param>
        /// <param name="producerOptions">生产者配置对象</param>
        /// <param name="loggerFactory">日志工厂</param>
        public RabbitMQClient(IOptions<RabbitMQOptions> rabbitMQOptions, IOptions<ConsumerOptions> consumerOptions, IOptions<ProducerOptions> producerOptions, ILoggerFactory loggerFactory)
            => connectionPool = new DefaultObjectPool<ChannelObject>(new ChannelPooledObjectPolicy(rabbitMQOptions, consumerOptions, producerOptions, loggerFactory));

        #endregion

        #region 获取通道对象

        /// <summary>
        /// 获取通道对象
        /// </summary>
        /// <returns></returns>
        public ChannelObject GetChannel()
            => connectionPool.Get();

        #endregion
    }

    #endregion
}
