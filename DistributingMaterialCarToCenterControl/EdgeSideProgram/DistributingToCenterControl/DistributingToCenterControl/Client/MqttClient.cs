using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using Serilog;
using System.Text;
using EdgeSideProgramScaffold.Service.FuncServices;

namespace EdgeSideProgramScaffold.Client
{
    internal class MqttClient
    {              
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
            
            // 订阅客户端订阅
            client.ConnectedAsync += AddTopicSubscription;
            // 订阅连接失败事件。
            client.ConnectingFailedAsync += LogConnectingFailed;
            // 订阅连接状态变化事件。
            client.ConnectionStateChangedAsync += LogConnectionStateChanged;
            
            //订阅应用程序消息接收事件
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            client.StartAsync(managedMqttClientOptions); //托管客户端  启动           
            Log.Information($"mqtt客户端:id:{_configService.MQTT_ID},addr:{_configService.MQTT_IP},port:{_configService.MQTT_Port}");
        }

        /// <summary>
        /// 接收调度发来的MQTT State
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {

            var content = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
            if (arg.ApplicationMessage.Topic == $"{_configService.MQTT_CURRENTSTATE_TOPIC}")
            {
                Log.Debug($"mqtt 收到State消息：{arg.ApplicationMessage.Topic},{content}");
                if (!string.IsNullOrEmpty(content))
                {
                    _commandQueueService.EnqueueCurrentState(content);  //实时命令入队
                }
            }
            else if (arg.ApplicationMessage.Topic == $"{_configService.MQTT_BACKEND_MaterialGrids}")
            {
                Log.Debug($"mqtt 收到MaterialGrids消息：{arg.ApplicationMessage.Topic},{content}");
                if (!string.IsNullOrEmpty(content))
                {
                    _commandQueueService.EnqueueMaterialGrids(content);  //实时命令入队
                }
            }
            else if (arg.ApplicationMessage.Topic == $"{_configService.MQTT_BACKEND_DistributingMaterialCars}")
            {
                Log.Debug($"mqtt 收到DistributingCars消息：{arg.ApplicationMessage.Topic},{content}");
                if (!string.IsNullOrEmpty(content))
                {
                    _commandQueueService.EnqueueDistributingCars(content);  //实时命令入队
                }
            }
            else if (arg.ApplicationMessage.Topic == $"{_configService.MQTT_BACKEND_DistributingMaterialCarsCmd}")
            {
                Log.Debug($"mqtt 收到DistributingCarsCmd消息：{arg.ApplicationMessage.Topic},{content}");
                if (!string.IsNullOrEmpty(content))
                {
                    _commandQueueService.EnqueueDistributingCarsCmd(content);  //实时命令入队
                }
            }
            else
            {
                Log.Error("MQTT接收到了异常来源数据，并非是MQTT_CURRENTSTATE_TOPIC主题");
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
            client.SubscribeAsync(_configService.MQTT_CURRENTSTATE_TOPIC);
            Log.Information($"{_configService.MQTT_CURRENTSTATE_TOPIC}已订阅");

            client.SubscribeAsync(_configService.MQTT_BACKEND_MaterialGrids);
            Log.Information($"{_configService.MQTT_BACKEND_MaterialGrids}已订阅");

            client.SubscribeAsync(_configService.MQTT_BACKEND_DistributingMaterialCars);
            Log.Information($"{_configService.MQTT_BACKEND_DistributingMaterialCars}已订阅");

            client.SubscribeAsync(_configService.MQTT_BACKEND_DistributingMaterialCarsCmd);
            Log.Information($"{_configService.MQTT_BACKEND_DistributingMaterialCarsCmd}已订阅");

            return Task.CompletedTask;
        }
    }

}

