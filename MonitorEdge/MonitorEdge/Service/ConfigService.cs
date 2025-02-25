using MiniExcelLibs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.Service
{
    internal class ConfigService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        //以下是接收
        public string TopicCtrlCrane01 { get; set; } = "ICS/CTRL/ECrane01";
        public string TopicCtrlCrane02 { get; set; } = "ICS/CTRL/ECrane02";
        public string TopicCtrlCrane03 { get; set; } = "ICS/CTRL/ECrane03";
        public string TopicCtrlCrane04 { get; set; } = "ICS/CTRL/ECrane04";
        
        public string TopicCmdWalkCrane01 { get; set; } = "ICS/CMD/WALK/ECrane01";
        public string TopicCmdWalkCrane02 { get; set; } = "ICS/CMD/WALK/ECrane02";
        public string TopicCmdWalkCrane03 { get; set; } = "ICS/CMD/WALK/ECrane03";
        public string TopicCmdWalkCrane04 { get; set; } = "ICS/CMD/WALK/ECrane04";

        public string TopicCmdGetCrane01 { get; set; } = "ICS/CMD/GET/ECrane01";
        public string TopicCmdGetCrane02 { get; set; } = "ICS/CMD/GET/ECrane02";
        public string TopicCmdGetCrane03 { get; set; } = "ICS/CMD/GET/ECrane03";
        public string TopicCmdGetCrane04 { get; set; } = "ICS/CMD/GET/ECrane04";

        public string TopicCmdPutCrane01 { get; set; } = "ICS/CMD/PUT/ECrane01";
        public string TopicCmdPutCrane02 { get; set; } = "ICS/CMD/PUT/ECrane02";
        public string TopicCmdPutCrane03 { get; set; } = "ICS/CMD/PUT/ECrane03";
        public string TopicCmdPutCrane04 { get; set; } = "ICS/CMD/PUT/ECrane04";
        
        
        //以下是发送
        public string TopicMainDataCrane01 { get; set; } = "ICS/EQ_STATE/ECrane01";
        public string TopicMainDataCrane02 { get; set; } = "ICS/EQ_STATE/ECrane02";
        public string TopicMainDataCrane03 { get; set; } = "ICS/EQ_STATE/ECrane03";
        public string TopicMainDataCrane04 { get; set; } = "ICS/EQ_STATE/ECrane04";

        public string MQTT_ID = "TEST";
        public string MQTT_IP = "localhost";
        public int MQTT_PORT = 1883;
    }

}
