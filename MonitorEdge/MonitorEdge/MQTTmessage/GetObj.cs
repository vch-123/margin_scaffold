using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.MQTTmessage
{
    internal class GetObj
    {
        public DateTime Time { get; set; }
        public long Id { get; set; }
        public int Z { get; set; }
        public int Mode1 { get; set; } = 1;
        public int Mode2 { get; set; } = 1;

    }
}
