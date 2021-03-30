using Hs.Foundation.RabbitMQ.Message;
using System;
using System.Text;
#nullable disable
namespace Hs.Foundation.RabbitMQ.Event
{
    #region 事件元数据

    /// <summary>
    /// 事件元数据
    /// </summary>
    public readonly struct EventMetadata
    {
        #region 属性

        /// <summary>
        /// 事件ID
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// 事件版本
        /// </summary>
        public long Version { get; }

        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTimeOffset DateTime { get; }
        
        /// <summary>
        /// 触发事件的命令ID
        /// </summary>
        public string CommandId { get; }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="datetime">时间</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="commandId">命令ID</param>
        private EventMetadata(long version, DateTimeOffset datetime, string eventId, string commandId)
        {
            if (version < 0)
                throw new ArgumentException(nameof(version));

            EventId = eventId;
            Version = version;
            DateTime = datetime;
            CommandId = commandId;
        }

        #endregion

        #region 创建事件元数据

        /// <summary>
        /// 创建事件元数据
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="datetime">时间</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="commandId">命令ID</param>
        /// <returns></returns>
        public static EventMetadata Create(long version, DateTimeOffset datetime, string eventId = null, string commandId = null)
            => new EventMetadata(version, datetime, string.IsNullOrEmpty(eventId) ? $"{version}_{datetime.ToUnixTimeMilliseconds()}" : eventId, commandId ?? string.Empty);

        #endregion

        #region 获取字节数组

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            //字节顺序为表示EventId的字节长度的字节数组,commandId的字节长度的字节数组，EventId字节数组,commandId字节数组，版本号字节数组，时间字节数组
            MessageMemoryStream messageStream = new MessageMemoryStream();
            var eventIdBytes = Encoding.UTF8.GetBytes(EventId);
            var eventIdLengthBytes = BitConverter.GetBytes(eventIdBytes.Length);
            var commandIdBytes = Encoding.UTF8.GetBytes(CommandId);
            var commandIdLengthBytes = BitConverter.GetBytes(commandIdBytes.Length);
            var versionBytes = BitConverter.GetBytes(Version);
            var dateTimeBytes = BitConverter.GetBytes(DateTime.ToUnixTimeMilliseconds());

            messageStream.Write(eventIdLengthBytes);
            messageStream.Write(commandIdLengthBytes);
            messageStream.Write(eventIdBytes);
            messageStream.Write(commandIdBytes);
            messageStream.Write(versionBytes);
            messageStream.Write(dateTimeBytes);

            return messageStream.ToArray();
        }

        #endregion

        #region 从字节数组转元数据对象

        /// <summary>
        /// 从字节数组转元数据对象
        /// </summary>
        /// <param name="eventMetadataBytes">元数据字节数组</param>
        /// <param name="eventMetadata">元数据对象</param>
        /// <returns></returns>
        public static bool TryFromBytes(Span<byte> eventMetadataBytes, out EventMetadata eventMetadata)
        {
            eventMetadata = default;
            if (eventMetadataBytes == null || eventMetadataBytes.Length < sizeof(int) + sizeof(long) + sizeof(long))
                return false;
            else
            {
                var eventIdLengthBytes = eventMetadataBytes.Slice(0, sizeof(int));
                var commandIdLengthBytes = eventMetadataBytes.Slice(eventIdLengthBytes.Length, sizeof(int));
                var eventIdLength = BitConverter.ToInt32(eventIdLengthBytes);
                var commandIdLength = BitConverter.ToInt32(commandIdLengthBytes);

                var eventId = Encoding.UTF8.GetString(eventMetadataBytes.Slice(sizeof(int) * 2, eventIdLength));
                var commandId = Encoding.UTF8.GetString(eventMetadataBytes.Slice(sizeof(int) * 2 + eventIdLength, commandIdLength));

                var versionBytes = eventMetadataBytes.Slice(sizeof(int) * 2 + eventIdLength + commandIdLength, sizeof(long));
                var metadataBytess = eventMetadataBytes.Slice(sizeof(int) * 2 + eventIdLength + commandIdLength + sizeof(long), sizeof(long));
                eventMetadata = Create(BitConverter.ToInt64(versionBytes), DateTimeOffset.UtcNow, eventId, commandId);

                return true;
            }
        }

        #endregion
    }

    #endregion
}
