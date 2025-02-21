using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMarginalScaffold.Model.CraneState
{
    public class MainData
    {
        /// <summary>
        /// 时间戳  "2024-02-04T17:10:56.2121661+08:00",
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// PLC与边缘测是否通畅  1在线  0断连
        /// </summary>
        public int PlcOnline { get; set; }

        /// <summary>
        /// 设备名称 string
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 当前Cmd名称  走行  取料  放料
        /// </summary>
        public string CMD { get; set; }

        /// <summary>
        /// CmdId
        /// </summary>
        public long CommID { get; set; }

        /// <summary>
        /// 未执行  正在执行  执行正常完成  执行异常完成
        /// </summary>       
        public int CmdState { get; set; }

        /// <summary>
        /// 是否合闸
        /// </summary>
        public int RelaySwitchOK { get; set; }

        /// <summary>
        /// 是否上电
        /// </summary>
        public int PowerOK { get; set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public int Paused { get; set; }

        /// <summary>
        /// 是否软件发送急停
        /// </summary>
        public int SoftEMStop { get; set; }

        /// <summary>
        /// 是否流程终止
        /// </summary>
        public int ProcessStopped { get; set; }

        /// <summary>
        /// 智能钥匙是否插好
        /// </summary>
        public int AutoModeKey { get; set; }

        /// <summary>
        /// 是否进入自动模式
        /// </summary>
        public int AutoMode { get; set; }

        /// <summary>
        /// 操作地是否是遥控器
        /// </summary>
        public int ManModeHandset { get; set; }

        /// <summary>
        /// 操作地是否是司机室
        /// </summary>
        public int ManModeDriver { get; set; }

        /// <summary>
        /// 操作地是否为远程操作台
        /// </summary>
        public int ManModeRemoteControl { get; set; }

        /// <summary>
        /// 小车及小车电机转速
        /// </summary>
        public int X { get; set; }
        public int XS { get; set; }

        /// <summary>
        /// 大车及大车电机转速
        /// </summary>
        public int Y { get; set; }
        public int YS { get; set; }

        /// <summary>
        /// 吊具及吊具电机转数速
        /// </summary>
        public int Z { get; set; }
        public int ZS { get; set; }

        /// <summary>
        /// 起重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 抓斗开合度
        /// </summary>
        public decimal GrabAngle { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        public string? AlarmMessage { get; set; }

        /// <summary>
        /// 其他数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; }
    }
}
