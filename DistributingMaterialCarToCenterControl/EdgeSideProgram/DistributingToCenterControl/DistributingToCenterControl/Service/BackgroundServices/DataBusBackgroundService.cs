using EdgeSideProgramScaffold.Client;
using EdgeSideProgramScaffold.Model;
using EdgeSideProgramScaffold.Service.FuncServices;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.BackgroundServices
{
    internal class DataBusBackgroundService : BackgroundService
    {

        private const int PulseInterval = 150; // 定义脉冲间隔常量
        private const int PulseCount = 5; // 定义脉冲次数常量

        private readonly CacheService _cacheService;
        private readonly Client.MqttClient _mqttClient;
        private readonly ConfigService _configService;
        private readonly UDPClient _udpClient;
        private byte cycleTime;
        public DataBusBackgroundService(CacheService cacheService, Client.MqttClient mqttClient,
                                        ConfigService configService, UDPClient uDPClient)
        {
            _cacheService = cacheService;
            _mqttClient = mqttClient;
            _configService = configService;
            _udpClient = uDPClient;
            cycleTime = 0;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {              
                await PublishCraneData();
                await Task.Delay(_configService.LoadTime, stoppingToken);
            }
        }


        // 回传给上位机的状态数据
        private async Task PublishCraneData() //这就是发MainData给后端了
        {
            
            _cacheService.MainData.Time=TagOperator.GetTime();
            if (CacheService._heartBeat == false)
            {
                _cacheService.MainData.GeneralPlcOnline = 0;
                _cacheService.MainData.ECars[0].PlcOnline = 0;
                _cacheService.MainData.ECars[1].PlcOnline = 0;
                Log.Error("发送的时候检测到心跳为false");
            }
            else if(CacheService._heartBeat == true)
            {
                _cacheService.MainData.GeneralPlcOnline = 1;
            }
            //Log.Information("主数据发送: {@MainData}", _cacheService.MainData);
            await _mqttClient.PublishMessage(_configService.MQTT_EQ_STATE_TOPIC, 
                                            JsonConvert.SerializeObject(_cacheService.MainData));         
        }
    }
}
