using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMarginalScaffold.Enum
{
    public enum CtrlEnum
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
        /// 复位
        /// </summary>
        FaultReset = 6,

        /// <summary>
        /// 流程终止
        /// </summary>
        ProcessTermination = 7,

        /// <summary>
        /// 编码器清零
        /// </summary>
        EncoderReset=8,

        /// <summary>
        /// 照明开启
        /// </summary>
        LightOn=9,

        /// <summary>
        /// 照明关闭
        /// </summary>
        LightOff=10,

        /// <summary>
        /// 取消遥控模式
        /// </summary>
        CancelHandsetMode=11
    }
}
