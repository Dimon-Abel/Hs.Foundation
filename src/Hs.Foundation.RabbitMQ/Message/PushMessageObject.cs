using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Message
{
    #region 要推送的消息对象

    /// <summary>
    /// 要推送的消息对象
    /// </summary>
    public readonly struct PushMessageObject
    {
        #region 属性

        /// <summary>
        /// 要发送的消息
        /// </summary>
        public byte[] Message { get; }

        /// <summary>
        /// 消息主题
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Persistent { get; }

        /// <summary>
        /// 消息是否发送成功的任务
        /// </summary>
        public TaskCompletionSource<bool> ResultTask { get; }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="message">要发送的消息</param>
        /// <param name="topic">消息主题</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="persistent">是否持久化</param>
        public PushMessageObject(byte[] message, string topic, string messageId, bool persistent)
        {
            Message = message;
            Topic = topic;
            Persistent = persistent;
            MessageId = messageId;
            ResultTask = new TaskCompletionSource<bool>();
        }

        #endregion
    }

    #endregion
}

