using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorEdge.Service;

namespace MonitorEdge
{
    internal class MqttClient
    {
        public IManagedMqttClient? client;
        private readonly ConfigService _configService;
        private CacheService _cacheService;

        public MqttClient(ConfigService configService,CacheService cacheService)
        {
            _configService = configService;           
            _cacheService = cacheService;
            Initialize();
        }

        // 一个异步方法，用于向指定的MQTT主题发布消息。
        public async Task PublishMessage(string topic, string message)
        {
            await client.EnqueueAsync(topic, message); // 将消息入队到MQTT客户端
        }

        private void Initialize()
        {
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_configService.MQTT_IP, _configService.MQTT_PORT)
                .WithClientId(_configService.MQTT_ID)
                .Build();

            // 托管MQTT客户端
            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            #region log
            // 订阅客户端订阅
            client.ConnectedAsync += AddTopicSubscription;
            // 订阅连接失败事件。
            client.ConnectingFailedAsync += LogConnectingFailed;
            // 订阅连接状态变化事件。
            client.ConnectionStateChangedAsync += LogConnectionStateChanged;
            #endregion

            // 订阅应用程序消息接收事件
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            client.StartAsync(managedMqttClientOptions); // 托管客户端 启动
            Log.Information($"mqtt客户端:id:{_configService.MQTT_ID},addr:{_configService.MQTT_IP},port:{_configService.MQTT_PORT}");
        }

        /// <summary>
        /// 接收调度发来的MQTT CMD和CTRL
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var content = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
            var topic = arg.ApplicationMessage.Topic;

            // 根据主题类型分发到不同的处理方法
            if (IsCtrlTopic(topic))
            {
                //var deviceName = topic.Split('/').Last();
                //_cacheService.HandleWalkCommand(deviceName, content);
            }
            else if (IsCmdWalkTopic(topic))
            {
                await HandleCmdWalkMessageAsync(topic, content);
            }
            else if (IsCmdGetTopic(topic))
            {
                await HandleCmdGetMessageAsync(topic, content);
            }
            else if (IsCmdPutTopic(topic))
            {
                await HandleCmdPutMessageAsync(topic, content);
            }
            else
            {
                Log.Error($"mqtt收到无效消息：{topic}, {content}");
            }
        }

        private bool IsCtrlTopic(string topic)
        {
            return topic.StartsWith("ICS/CTRL/");
        }

        private bool IsCmdWalkTopic(string topic)
        {
            return topic.StartsWith("ICS/CMD/WALK/");
        }

        private bool IsCmdGetTopic(string topic)
        {
            return topic.StartsWith("ICS/CMD/GET/");
        }

        private bool IsCmdPutTopic(string topic)
        {
            return topic.StartsWith("ICS/CMD/PUT/");
        }

        private async Task HandleCtrlMessageAsync(string topic, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                //_commandQueueService.ExecuteRealTime(topic,content); 
                Log.Information($"mqtt 收到Ctrl消息：{topic}, {content}");
            }
        }

        private async Task HandleCmdWalkMessageAsync(string topic, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var deviceName = topic.Split('/').Last();
                _cacheService.HandleWalkCommand(deviceName, content);
                Log.Information($"mqtt 收到CmdWalk消息：{topic}, {content}");               
            }
        }

        private async Task HandleCmdGetMessageAsync(string topic, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var deviceName = topic.Split('/').Last();
                _cacheService.HandleGetCommand(deviceName, content);
                Log.Information($"mqtt 收到CmdGet消息：{topic}, {content}");
            }
        }

        private async Task HandleCmdPutMessageAsync(string topic, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var deviceName = topic.Split('/').Last();
                _cacheService.HandlePutCommand(deviceName, content);
                Log.Information($"mqtt 收到CmdPut消息：{topic}, {content}");
            }
        }

        private Task LogConnectionStateChanged(EventArgs arg)
        {
            Log.Information($"mqtt {client}连接状态发生变化: {client.IsConnected}");
            return Task.CompletedTask;
        }

        private Task LogConnectingFailed(ConnectingFailedEventArgs arg)
        {
            Log.Error($"mqtt {client}连接失败: {arg.Exception}");
            return Task.CompletedTask;
        }

        private Task AddTopicSubscription(MqttClientConnectedEventArgs arg)
        {
            client.SubscribeAsync(_configService.TopicCtrlCrane01);
            client.SubscribeAsync(_configService.TopicCtrlCrane02);
            client.SubscribeAsync(_configService.TopicCtrlCrane03);
            client.SubscribeAsync(_configService.TopicCtrlCrane04);

            client.SubscribeAsync(_configService.TopicCmdWalkCrane01);
            client.SubscribeAsync(_configService.TopicCmdWalkCrane02);
            client.SubscribeAsync(_configService.TopicCmdWalkCrane03);
            client.SubscribeAsync(_configService.TopicCmdWalkCrane04);

            client.SubscribeAsync(_configService.TopicCmdGetCrane01);
            client.SubscribeAsync(_configService.TopicCmdGetCrane02);
            client.SubscribeAsync(_configService.TopicCmdGetCrane03);
            client.SubscribeAsync(_configService.TopicCmdGetCrane04);

            client.SubscribeAsync(_configService.TopicCmdPutCrane01);
            client.SubscribeAsync(_configService.TopicCmdPutCrane02);
            client.SubscribeAsync(_configService.TopicCmdPutCrane03);
            client.SubscribeAsync(_configService.TopicCmdPutCrane04);

            return Task.CompletedTask;
        }
    }
}