using Hs.Foundation.RabbitMQ.Abstractions;
using Hs.Foundation.RabbitMQ.Common;
using Hs.Foundation.RabbitMQ.Consumer;
using Hs.Foundation.RabbitMQ.Options;
using Hs.Foundation.RabbitMQ.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hs.Foundation.RabbitMQ.Extensions
{
    #region ServiceCollection扩展方法

    /// <summary>
    /// ServiceCollection扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region 添加RabbitMQ消息总线

        /// <summary>
        /// 添加RabbitMQ消息总线
        /// </summary>
        /// <param name="services">        IServiceCollection </param>
        /// <param name="rabbitMQOptions"> 设置配置信息委托 </param>
        /// <returns> </returns>
        public static IServiceCollection AddRabbitMQMessagBus(this IServiceCollection services, Action<RabbitMQOptions> rabbitMQOptions)
            => services.Configure(rabbitMQOptions)
                .AddSingleton<IRabbitMQClient, RabbitMQClient>()
                .AddSingleton<IProducer, RabbitMQProducer>()
                .AddSingleton<IConsumer, RabbitMQConsumer>();

        /// <summary>
        /// 添加RabbitMQ消息总线
        /// </summary>
        /// <param name="services">      IServiceCollection </param>
        /// <param name="configuration"> 配置文件对象(RabbitMQOptions) </param>
        /// <returns> </returns>
        public static IServiceCollection AddRabbitMQMessagBus(this IServiceCollection services, IConfiguration configuration)
            => AddRabbitMQMessagBus(services, config =>
            {
                var rabbitMQOptions = new RabbitMQOptions();
                ConfigurationHelper.GetConfiguration(configuration, nameof(RabbitMQOptions)).Bind(rabbitMQOptions);

                config.UserName = rabbitMQOptions.UserName;
                config.Password = rabbitMQOptions.Password;
                config.VirtualHost = rabbitMQOptions.VirtualHost;
                config.ChannelPoolSize = rabbitMQOptions.ChannelPoolSize;
                config.ConnectionPoolSize = rabbitMQOptions.ConnectionPoolSize;
                config.HostNames = rabbitMQOptions.HostNames;
                config.ExchangeName = rabbitMQOptions.ExchangeName;
            });

        #endregion 添加RabbitMQ消息总线
    }

    #endregion ServiceCollection扩展方法
}