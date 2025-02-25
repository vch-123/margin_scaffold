using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.Enums
{
    internal enum CtrlEnum
    {
        /// <summary>
        /// 启动软件模式
        /// </summary>
        SoftwareModeBoot = 0,

        /// <summary>
        /// 断开软件模式
        /// </summary>
        SoftwareModeDisconnect = 1,

        /// <summary>
        /// 暂停  
        /// </summary>
        Pause = 2,

        /// <summary>
        /// 暂停恢复
        /// </summary>
        PauseResume = 3,

        /// <summary>
        /// 软件急停
        /// </summary>
        SoftwareEMStop = 4,

        /// <summary>
        /// 软件急停恢复
        /// </summary>
        SoftwareEMStopResume = 5,

        /// <summary>
        /// 故障复位
        /// </summary>
        FaultReset = 6,

        /// <summary>
        /// 流程终止 
        /// </summary>
        ProcessTermination = 7,

        /// <summary>
        /// 皮带启动
        /// </summary>
        BeltBoot = 8,

        /// <summary>
        /// 皮带停止
        /// </summary>
        BeltStop = 9,

        /// <summary>
        /// 抬犁
        /// </summary>
        PlowUp = 10,

        /// <summary>
        /// 落犁
        /// </summary>
        PlowDown = 11,

        /// <summary>
        /// 单动模式
        /// </summary>
        SingleInstructionMode = 12,

        /// <summary>
        /// 联动模式
        /// </summary>
        LinkedInstructionMode = 13,

        TinyCarCome = 14

    }
}
