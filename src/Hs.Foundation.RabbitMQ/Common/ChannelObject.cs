using Hs.Foundation.RabbitMQ.Message;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Hs.Foundation.RabbitMQ.Common
{
    #region 消息通道对象

    /// <summary>
    /// 消息通道对象
    /// </summary>
    public class ChannelObject : IDisposable
    {
        #region 私有变量

        /// <summary>
        /// 连接对象
        /// </summary>
        private readonly ConnectionObject connectionObject;

        /// <summary>
        /// 通道
        /// </summary>
        private readonly IModel channel;

        /// <summary>
        /// 持久化属性
        /// </summary>
        private readonly IBasicProperties persistentProperties;

        /// <summary>
        /// 非持久化属性
        /// </summary>
        private readonly IBasicProperties unPersistentProperties;

        /// <summary>
        /// 消息发送处理器
        /// </summary>
        private ActionBlock<PushMessageObject> sendAction;

        /// <summary>
        /// 批量发送消息的缓存
        /// </summary>
        private readonly List<PushMessageObject> messages;

        /// <summary>
        /// 日志对象
        /// </summary>
        private ILogger logger;

        #endregion 私有变量

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionObject"> 连接对象 </param>
        /// <param name="channel">          管道 </param>
        /// <param name="logger">           日志对象 </param>
        public ChannelObject(ConnectionObject connectionObject, IModel channel, ILogger<ChannelObject> logger)
        {
            this.channel = channel;
            this.connectionObject = connectionObject;
            messages = new List<PushMessageObject>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            persistentProperties = channel.CreateBasicProperties();
            persistentProperties.Persistent = true;

            unPersistentProperties = channel.CreateBasicProperties();
            unPersistentProperties.Persistent = false;

            //定义交换机
            channel.ExchangeDeclare(connectionObject.RabbitMQOptions.ExchangeName, "topic", true, false);
            BuilderSendAction();
        }

        #endregion 初始化

        #region 析构函数

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ChannelObject()
            => Dispose(false);

        #endregion 析构函数

        #region 构建消息批量发送

        /// <summary>
        /// 构建消息批量发送
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BuilderSendAction()
        {
            sendAction = new ActionBlock<PushMessageObject>(m =>
            {
                messages.Add(m);
                if (messages.Count >= connectionObject.ProducerOptions.MaxPublishMessages || sendAction.InputCount == 0)
                {
                    var result = BatchPublish(messages);
                    messages.ForEach(m => m.ResultTask.TrySetResult(result));
                    messages.Clear();
                }
            });
        }

        #endregion 构建消息批量发送

        #region 批量发送消息

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="messages"> </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool BatchPublish(IEnumerable<PushMessageObject> messages)
        {
            var batchMessage = channel.CreateBasicPublishBatch();
            foreach (var m in messages)
            {
                var basicProperties = m.Persistent ? persistentProperties : unPersistentProperties;
                if (string.IsNullOrEmpty(m.MessageId))
                    basicProperties.MessageId = m.MessageId;

                batchMessage.Add(connectionObject.RabbitMQOptions.ExchangeName, m.Topic, true, basicProperties, m.Message.AsMemory());
            }

            try
            {
                batchMessage.Publish();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 批量发送消息

        #region 发送消息

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">        消息内容 </param>
        /// <param name="topic">      消息主题 </param>
        /// <param name="messageId">  消息ID </param>
        /// <param name="persistent"> 是否持久化 </param>
        /// <returns> </returns>
        public Task<bool> Publish(byte[] msg, string topic, string messageId = null, bool persistent = true)
        {
            return Task.Run(async () =>
            {
                PushMessageObject messageObject = new PushMessageObject(msg, topic, messageId, persistent);
                if (!sendAction.Post(messageObject))
                    await sendAction.SendAsync(messageObject);

                return await messageObject.ResultTask.Task;
            });
        }

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="messageObjects"> </param>
        /// <returns> </returns>
        public Task<bool> Publish(IEnumerable<PushMessageObject> messageObjects)
            => Task.FromResult(BatchPublish(messageObjects));

        #endregion 发送消息

        #region 订阅消息

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic">   消息Topic </param>
        /// <param name="group">   消息组 </param>
        /// <param name="handler"> 事件处理器 </param>
        public void Subscribe(string topic, string group, Func<ReadOnlyMemory<byte>, ulong, Task> handler)
        {
            channel.QueueDeclare(group, true, false, false);
            channel.QueueBind(group, connectionObject.RabbitMQOptions.ExchangeName, topic);
            channel.BasicQos(0, (ushort)connectionObject.ConsumerOptions.CunsumerMaxBatchSize, true);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                if (handler != null)
                {
                    try
                    {
                        await handler(args.Body, args.DeliveryTag);
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical($"消息处理失败：{ex.Message}");
                        channel.BasicNack(args.DeliveryTag, false, connectionObject.ConsumerOptions.ReturnQueue);
                    }
                }
                else
                    channel.BasicNack(args.DeliveryTag, false, connectionObject.ConsumerOptions.ReturnQueue);
            };
            channel.BasicConsume(group, connectionObject.ConsumerOptions.AutoAck, consumer);
        }

        #endregion 订阅消息

        #region 确认消息

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="deliveryTag"> 消息标识 </param>
        /// <param name="multiple">    是否一次确认多个消息 </param>
        public void Ack(ulong deliveryTag, bool multiple)
            => channel.BasicAck(deliveryTag, multiple);

        #endregion 确认消息

        #region 拒绝消息

        /// <summary>
        /// 拒绝消息
        /// </summary>
        /// <param name="deliveryTag"> 消息标识 </param>
        /// <param name="multiple">    是否一次拒绝多个消息 </param>
        /// <param name="requeue">     是否重回队列 </param>
        public void Nack(ulong deliveryTag, bool multiple, bool requeue)
            => channel.BasicNack(deliveryTag, multiple, requeue);

        #endregion 拒绝消息

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"> </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                sendAction.Complete();
                sendAction.Completion.Wait();
            }
        }

        #endregion 释放资源
    }

    #endregion 消息通道对象
}