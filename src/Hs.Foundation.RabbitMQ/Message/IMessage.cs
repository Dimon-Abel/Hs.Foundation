using Hs.Foundation.RabbitMQ.Event;

namespace Hs.Foundation.RabbitMQ.Message
{
    #region 消息

    /// <summary>
    /// 消息
    /// </summary>
    public interface IMessage
    {
        #region 属性

        /// <summary>
        /// 消息Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 事件对象
        /// </summary>
        public byte[] Event { get; }

        /// <summary>
        /// 事件元数据
        /// </summary>
        public EventMetadata EventMetadata { get; }

        #endregion 属性

        #region 方法

        /// <summary>
        /// 获取消息的字节数组
        /// </summary>
        /// <returns> </returns>
        byte[] GetBytes();

        #endregion 方法
    }

    #endregion 消息
}