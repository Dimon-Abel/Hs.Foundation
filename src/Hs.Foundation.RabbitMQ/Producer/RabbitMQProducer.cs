using Hs.Foundation.RabbitMQ.Abstractions;
using Hs.Foundation.RabbitMQ.Common;
using Hs.Foundation.RabbitMQ.Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Producer
{
    #region RabbitMQ消息生产者

    /// <summary>
    /// RabbitMQ消息生产者
    /// </summary>
    public class RabbitMQProducer : IProducer
    {
        #region 私有变量

        /// <summary>
        /// RabbitMQ客户端对象
        /// </summary>
        private readonly IRabbitMQClient rabbitMQClient;

        #endregion 私有变量

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rabbitMQClient"> </param>
        public RabbitMQProducer(IRabbitMQClient rabbitMQClient)
            => this.rabbitMQClient = rabbitMQClient;

        #endregion 初始化

        #region 发送消息

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">   要发送的消息 </param>
        /// <param name="messageId"> 消息标识 </param>
        /// <param name="topic">     消息主题 </param>
        /// <returns> </returns>
        public Task<bool> Publish(IMessage message, string topic, string messageId = null)
        {
            try
            {
                return rabbitMQClient.GetChannel().Publish(message.GetBytes(), topic, messageId);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="messageObject"> </param>
        /// <returns> </returns>
        public Task<bool> Publish(IEnumerable<PushMessageObject> messageObject)
        {
            try
            {
                return rabbitMQClient.GetChannel().Publish(messageObject);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        #endregion 发送消息
    }

    #endregion RabbitMQ消息生产者
}