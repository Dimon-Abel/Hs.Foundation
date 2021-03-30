using Hs.Foundation.RabbitMQ.Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Abstractions
{
    #region 消息生产者

    /// <summary>
    /// 消息生产者
    /// </summary>
    public interface IProducer
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">   要发布的消息 </param>
        /// <param name="topic">     消息主题 </param>
        /// <param name="messageId"> 消息ID </param>
        /// <returns> </returns>
        Task<bool> Publish(IMessage message, string topic, string messageId = null);

        /// <summary>
        /// 批量发布消息
        /// </summary>
        /// <param name="messageObject"> </param>
        /// <returns> </returns>
        Task<bool> Publish(IEnumerable<PushMessageObject> messageObject);
    }

    #endregion 消息生产者
}