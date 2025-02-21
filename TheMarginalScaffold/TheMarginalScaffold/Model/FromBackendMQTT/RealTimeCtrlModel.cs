using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Enum;

namespace TheMarginalScaffold.Model.FromBackendMQTT
{
    public class RealTimeCtrlModel
    {
        public DateTime Time { get; set; }
        public CtrlEnum Ctrl { get; set; }
    }
}
