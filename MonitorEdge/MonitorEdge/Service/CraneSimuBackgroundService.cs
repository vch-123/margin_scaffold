using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using MonitorEdge.MQTTmessage;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.Service
{
    internal class CraneSimuBackgroundService : BackgroundService
    {
        private ConfigService _configService;
        private MqttClient _mqttClient;
        private CacheService _cacheService;
     

        public CraneSimuBackgroundService(ConfigService configService,MqttClient mqttClient,CacheService cacheService)
        {
            _configService = configService;
            _mqttClient = mqttClient;                     
            _cacheService = cacheService;
        }

      
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var crane in _cacheService._cranes)
                {
                    crane.Time = DateTime.Now;
                    var topic = $"ICS/EQ_STATE/{crane.DeviceName}";
                    var message = JsonConvert.SerializeObject(crane);
                    await _mqttClient.PublishMessage(topic, message);                                      
                    Console.WriteLine($"Sent status for {crane.DeviceName}: {message}");
                    Console.WriteLine("\n");
                }

                await Task.Delay(200);
                Console.Clear();
            }
        }
    }
}
