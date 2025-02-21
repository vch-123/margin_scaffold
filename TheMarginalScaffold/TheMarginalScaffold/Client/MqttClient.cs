using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Client
{
    public class MqttClient
    {
        public static string Cmd;

        private readonly CacheService _cacheService;
        public IManagedMqttClient? client;
        private readonly ConfigService _configService;
        private readonly CommandQueueService _commandQueueService;

        public MqttClient(ConfigService configService, CommandQueueService commandQueueService, CacheService cacheService)
        {
            _configService = configService;
            _commandQueueService = commandQueueService;
            _cacheService = cacheService;
            Initialize();
        }

        // 一个异步方法，用于向指定的MQTT主题发布消息。
        public async Task PublishMessage(string topic, string message)
        {
            await client.EnqueueAsync(topic, message);//将消息入队到MQTT客户端
        }

        private void Initialize()
        {
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_configService.MQTT_IP, _configService.MQTT_Port)
                .WithClientId(_configService.MQTT_ID)
                .Build();

            //托管MQTT客户端
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

            //订阅应用程序消息接收事件
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            client.StartAsync(managedMqttClientOptions); //托管客户端  启动           
            Log.Information($"mqtt客户端:id:{_configService.MQTT_ID},addr:{_configService.MQTT_IP},port:{_configService.MQTT_Port}");
        }

        /// <summary>
        /// 接收调度发来的MQTT CMD和CTRL
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {

            var content = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
            if (arg.ApplicationMessage.Topic == $"{_configService.MQTT_CTRL_TOPIC}")
            {
                if (!string.IsNullOrEmpty(content))
                {
                    _commandQueueService.EnqueueRealTimeCtrl(content);  //实时命令入队
                    Log.Information($"mqtt 收到Ctrl消息：{arg.ApplicationMessage.Topic},{content}");
                }
            }

            else
            {
                if (_cacheService.CanSendCmd())
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        Log.Information($"mqtt 收到Cmd原始消息：{arg.ApplicationMessage.Topic},{content}需要经过offset转换 ");
                        switch (arg.ApplicationMessage.Topic)
                        {
                            case string n when n == $"{_configService.MQTT_WALK_TOPIC}":
                                MqttClient.Cmd = "walk";
                                _commandQueueService.EnqueueWalkCommand(content);
                                break;

                            case string n when n == $"{_configService.MQTT_GET_TOPIC}":
                                MqttClient.Cmd = "get";
                                _commandQueueService.EnqueueGetCommand(content);
                                break;

                            case string n when n == $"{_configService.MQTT_PUT_TOPIC}":
                                MqttClient.Cmd = "put";
                                _commandQueueService.EnqueuePutCommand(content);
                                break;
                            default:
                                Log.Error($"mqtt收到了无效的CMD消息(非walk/get/put)：{arg.ApplicationMessage.Topic},{content}");
                                break;
                        }
                    }
                }
                else
                {
                    Log.Error($"mqtt在CmdState非0/2时收到了新CMD消息：{arg.ApplicationMessage.Topic},{content}  CMDSTATE:{_cacheService.MainData.CmdState}");

                }
            }

            return Task.CompletedTask;
        }
        private Task LogConnectionStateChanged(EventArgs arg)
        {
            Log.Information($"mqtt {client}连接状态发生变化:{client.IsConnected}");
            return Task.CompletedTask;
        }

        private Task LogConnectingFailed(ConnectingFailedEventArgs arg)
        {
            Log.Error($"mqtt {client}连接失败:{arg.Exception}");
            return Task.CompletedTask;
        }

        private Task AddTopicSubscription(MqttClientConnectedEventArgs arg)
        {

            client.SubscribeAsync(_configService.MQTT_CTRL_TOPIC);
            client.SubscribeAsync(_configService.MQTT_WALK_TOPIC);
            client.SubscribeAsync(_configService.MQTT_GET_TOPIC);
            client.SubscribeAsync(_configService.MQTT_PUT_TOPIC);

            Log.Information($"{_configService.MQTT_CTRL_TOPIC}已订阅");
            Log.Information($"{_configService.MQTT_WALK_TOPIC}已订阅");
            Log.Information($"{_configService.MQTT_GET_TOPIC}已订阅");
            Log.Information($"{_configService.MQTT_PUT_TOPIC}已订阅");

            return Task.CompletedTask;
        }

    }
}
