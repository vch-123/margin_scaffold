using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.MQTTmessage
{
    internal class WalkObj
    {
        public DateTime Time { get; set; }
        public long Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

    }
}
