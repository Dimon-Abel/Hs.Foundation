namespace Hs.Foundation.RabbitMQ.Options
{
    #region 消费者配置

    /// <summary>
    /// 消费者配置
    /// </summary>
    public class ConsumerOptions
    {
        /// <summary>
        /// 是否自动Ack
        /// </summary>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// 消息消费失败是否重回队列(如果为false，则在本地不停重发给消费者消费)
        /// </summary>
        public bool ReturnQueue { get; set; } = true;

        /// <summary>
        /// 消费者批量处理每次处理的最大消息量(也就是客户端每次最大从消息服务器拉取的最大消息数量)
        /// </summary>
        public int CunsumerMaxBatchSize { get; set; } = 10000;
    }

    #endregion
}
