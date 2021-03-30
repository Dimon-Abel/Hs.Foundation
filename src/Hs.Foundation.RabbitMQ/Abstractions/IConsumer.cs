using System;
using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Abstractions
{
    #region 消息消费者

    /// <summary>
    /// 消息消费者
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="group">消息分组</param>
        /// <param name="handler">消息处理器</param>
        void Subscribe(string topic, string group, Func<ReadOnlyMemory<byte>, ulong, Task> handler);

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="group">消息组</param>
        /// <param name="deliveryTag">消息标识</param>
        /// <param name="multiple">是否批量确认，如果批量确认，则deliveryTag以下的消息都将被确认</param>
        void Ack(string group, ulong deliveryTag, bool multiple);

        /// <summary>
        /// 拒绝消息
        /// </summary>
        /// <param name="group">消息组</param>
        /// <param name="deliveryTag">消息ID</param>
        /// <param name="multiple">是否批量拒绝</param>
        /// <param name="enqueue">是否重新入队</param>
        void Nack(string group, ulong deliveryTag, bool multiple, bool enqueue);
    }

    #endregion
}
