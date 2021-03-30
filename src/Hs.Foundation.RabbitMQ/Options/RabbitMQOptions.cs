using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace Hs.Foundation.RabbitMQ.Options
{
    #region RabbitMQ配置

    /// <summary>
    /// RabbitMQ配置
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get; set; } = "HsFoundation";

        /// <summary>
        /// 服务器多个HostName之间用逗号隔开
        /// </summary>
        public string HostNames { get; set; }

        /// <summary>
        /// 服务器端点集合
        /// </summary>
        internal List<AmqpTcpEndpoint> EndPoints => HostNames.Split(',').Select(m => AmqpTcpEndpoint.Parse(m)).ToList();

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// VirtualHost
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// 每个连接上拉取和推送消息的池的大小(就是RabittMQ的Model的队列大小)
        /// </summary>
        public int ChannelPoolSize { get; set; } = 200;

        /// <summary>
        /// 最大连接数，拉取和推送消息的最大最大连接数
        /// </summary>
        public int ConnectionPoolSize { get; set; } = 20;
    }

    #endregion RabbitMQ配置
}