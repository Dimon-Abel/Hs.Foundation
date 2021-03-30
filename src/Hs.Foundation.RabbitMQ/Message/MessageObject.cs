using Hs.Foundation.RabbitMQ.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Foundation.RabbitMQ.Message
{
    #region 事件包装对象

    /// <summary>
    /// 事件包装对象
    /// </summary>
    public readonly struct MessageObject
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
        /// <param name="event"></param>
        /// <param name="eventMetadata"></param>
        public MessageObject(IEvent @event, EventMetadata eventMetadata)
        {
            Event = @event;
            EventMetadata = eventMetadata;
        }

        #endregion
    }

    #endregion
}

