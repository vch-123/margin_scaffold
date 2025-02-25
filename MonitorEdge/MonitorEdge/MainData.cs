using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge
{
    internal class MainData
    {

        public DateTime Time { get; set; }
        public int PlcOnline { get; set; } = 1;
        public string DeviceName { get; set; }
        public string CMD { get; set; }
        public long CommID { get; set; }
        public int CmdState { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z {  get; set; }
        public int XS { get; set; }
        public int YS { get; set; }
        public int ZS {  get; set; }
        public int RelaySwitchOK { get; set; } = 1;
        public int PowerOK { get; set; } = 1;
        public int AutoMode { get; set; } = 1;
        public int Paused { get; set; } = 0;
        public int ProcessStopped { get; set; } = 0;
        public int FaultReset { get; set; } = 0;
        public int EMStop { get; set; } = 0;
        public int SoftEMStop { get; set; } = 0;
        public int DeviceState { get; set; } = 0;

        public decimal Weight {  get; set; }
        public Dictionary<string, object> Data { get; set; }

        public int TargetX { get; set; } 
        public int TargetY { get; set; } 
        public int TargetZ { get; set; }
        public bool IsWalking { get; set; } 
        public bool IsGetting { get; set; }
        public bool IsPutting { get; set; }       

        public GetPutState GetPutState { get; set; }
        

    }

    internal enum GetPutState
    {
        Idle,
        MovingToTarget, // 移动到目标高度
        ReturningToInitial // 返回到初始高度
    }

}
