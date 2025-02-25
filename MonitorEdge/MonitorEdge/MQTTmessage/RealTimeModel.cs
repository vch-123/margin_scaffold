using MonitorEdge.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.MQTTmessage
{
    internal class RealTimeModel
    {
        public DateTime Time { get; set; }

        /// <summary>
        /// Ctrl指令枚举
        /// </summary>
        public CtrlEnum Ctrl { get; set; }

    }
}
