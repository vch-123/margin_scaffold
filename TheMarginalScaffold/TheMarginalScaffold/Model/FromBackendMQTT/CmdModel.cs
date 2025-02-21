using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMarginalScaffold.Model.FromBackendMQTT
{
    public class CmdBase
    {
        public DateTime Time { get; set; }
        public long Id { get; set; }
    }

    public class WalkCmdModel : CmdBase
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class GetCmdModel : CmdBase
    {
        public int Z { get; set; }
        public int Mode1 { get; set; }
        public int Mode2 { get; set; }
    }

    public class PutCmdModel : CmdBase
    {
        public int Z { get; set; }
    }
}
