using Hs.Foundation.RabbitMQ.Event;

namespace KingMetal.Domains.Abstractions.Event
{
    #region 事件对象

    /// <summary>
    /// 事件对象
    /// </summary>
    public readonly struct EventObject
    {
        #region 属性

        /// <summary>
        /// 事件对象
        /// </summary>
        public IEvent Event { get; }

        /// <summary>
        /// 事件元数据
        /// </summary>
        public EventMetadata EventMetadata { get; }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="event">事件对象</param>
        /// <param name="eventMetadata">事件元数据</param>
        public EventObject(IEvent @event,EventMetadata eventMetadata)
        {
            Event = @event;
            EventMetadata = eventMetadata;
        }

        #endregion
    }

    #endregion
}
