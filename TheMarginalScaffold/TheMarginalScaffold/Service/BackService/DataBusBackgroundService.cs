using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Client;
using TheMarginalScaffold.Model.CraneState;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Service.BackService
{
    public class DataBusBackgroundService : BackgroundService
    {
        private const int PulseInterval = 150; // 定义脉冲间隔常量
        private const int PulseCount = 5; // 定义脉冲次数常量  

        private readonly CacheService _cacheService;
        private readonly MqttClient _mqttClient;
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

        public bool CheckByte(byte a, int index)
        {
            byte mask = (byte)(1 << index);
            return (a & mask) != 0;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                WriteCraneData();

                await PublishCraneData();

                if (IsPulseRequired(2, 0))   //停止工作 脉冲 
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 0, false);
                }
                if (IsPulseRequired(2, 1))   //停止工作 脉冲 
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 1, false);
                }
                else if (IsPulseRequired(2, 2))  //故障复位  脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 2, false);
                }
                else if (IsPulseRequired(2, 3))  //暂停  脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 3, false);
                }
                else if (IsPulseRequired(2, 5))  //暂停恢复  脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 5, false);
                }
                else if (IsPulseRequired(2, 6))  //编码器清零 脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 6, false);
                }
                else if (IsPulseRequired(2, 7))  //流程中止  脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(2, 7, false);
                }
                else if (IsPulseRequired(3, 5))  //取消遥控器模式  脉冲
                {
                    await SendPulse(PulseCount, stoppingToken);
                    _cacheService.UpdateCtrlData(3, 5, false);
                }


                await Task.Delay(_configService.LoadTime, stoppingToken);
            }
        }

        private bool IsPulseRequired(int byte_index, int index)
        {
            return CheckByte(_cacheService.CtrlData[byte_index], index);
        }

        private async Task SendPulse(int count, CancellationToken token)
        {
            for (int i = 0; i < count; i++)
            {
                WriteCraneData();
                await Task.Delay(PulseInterval, token); // 等待脉冲间隔
            }
        }
       
        private async Task PublishCraneData() //这就是发MainData给后端了
        {           
            _cacheService.MainData.Time = DataConverterService.GetTime();

            if (CacheService._heartBeat == false)
            {
                _cacheService.MainData.PlcOnline = 0;
            }
            if ((_cacheService.MainData.AutoMode == 1) && (_cacheService.MainData.CmdState == 3))
            {
                Log.Error("在软件模式发送的时候检测到CMD_STATE为3" + "此时心跳:" + CacheService._heartBeat + " 数据:" + GetMainDataInfoString(_cacheService.MainData));
            }
            await _mqttClient.PublishMessage(_configService.MQTT_EQ_STATE_TOPIC, JsonConvert.SerializeObject(_cacheService.MainData)); //tagOperator的帮助 否则_cacheService.MainData里的数据就是空
            Log.Debug("主数据发送:" + GetMainDataInfoString(_cacheService.MainData));

        }

        public static string GetMainDataInfoString(MainData mainData)
        {
            if (mainData == null)
            {
                throw new ArgumentNullException(nameof(mainData));
            }

            Type type = mainData.GetType();
            StringBuilder info = new StringBuilder();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(mainData);
                info.Append(property.Name).Append(": ").Append(value).Append(", ");
            }

            if (info.Length > 0)
            {
                info.Length -= 2; // Remove the last comma and space
            }

            return info.ToString();
        }

        private async Task WriteCraneData()
        {
            _cacheService.CycleSelfIncrement(cycleTime);
            cycleTime++;
            await _udpClient.SendCtrlData(_cacheService.CtrlData);
        }
    }
}
