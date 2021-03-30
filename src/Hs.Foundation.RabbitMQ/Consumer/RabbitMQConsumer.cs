using Hs.Foundation.RabbitMQ.Abstractions;
using Hs.Foundation.RabbitMQ.Common;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Consumer
{
    #region RabbitMQ消费者

    /// <summary>
    /// RabbitMQ消费者
    /// </summary>
    public class RabbitMQConsumer : IConsumer
    {
        #region 私有变量

        /// <summary>
        /// RabbitMQ客户端对象
        /// </summary>
        private readonly IRabbitMQClient _rabbitMQClient;

        /// <summary>
        /// 订阅的通道集合
        /// </summary>
        private readonly ConcurrentDictionary<string, ChannelObject> channels;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rabbitMQClient"></param>
        public RabbitMQConsumer(IRabbitMQClient rabbitMQClient)
        {
            _rabbitMQClient = rabbitMQClient;
            channels = new ConcurrentDictionary<string, ChannelObject>();
        }

        #endregion

        #region 订阅消息

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="group">消息分组</param>
        /// <param name="handler">消息处理器</param>
        public void Subscribe(string topic, string group, Func<ReadOnlyMemory<byte>, ulong, Task> handler)
        {
            var channel = channels.GetOrAdd(group, _rabbitMQClient.GetChannel());
            channel.Subscribe(topic, group, handler);
        }

        #endregion

        #region 确认消息

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="group">组</param>
        /// <param name="deliveryTag">消息标识</param>
        /// <param name="multiple">是否批量确认，如果批量确认，则deliveryTag以下的消息都将被确认</param>
        public void Ack(string group, ulong deliveryTag, bool multiple)
            => channels[group].Ack(deliveryTag, multiple);

        #endregion

        #region 拒绝消息

        /// <summary>
        /// 拒绝消息
        /// </summary>
        /// <param name="group">组</param>
        /// <param name="deliveryTag">消息ID</param>
        /// <param name="multiple">是否批量拒绝</param>
        /// <param name="enqueue">是否重新入队</param>
        public void Nack(string group, ulong deliveryTag, bool multiple, bool enqueue)
            => channels[group].Nack(deliveryTag, multiple, enqueue);

        #endregion
    }

    #endregion
}
