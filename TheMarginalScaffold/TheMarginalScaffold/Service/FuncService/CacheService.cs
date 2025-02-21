using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Model.CraneState;

namespace TheMarginalScaffold.Service.FuncService
{
    public class CacheService
    {
        private static Timer? _timer = new Timer(CheckHeartBeat, null, 0, 500);
        public static bool _heartBeat = true; // 默认HeartBeat标志为true
        private static DateTime lastCalledTime;

        private TransferMainData _transferMainData = new TransferMainData();
        private MainData _mainData = new();
        private List<PLCVariableExcelModel> _models = new List<PLCVariableExcelModel>();
        private byte[] _ctrlData = new byte[6] { 0xCA, 0, 0, 0, 0, 0 };//发给起重机8000port   
        private readonly ConfigService _configService;

        public CacheService(ConfigService configService)
        {
            _configService = configService;

        }

        public TransferMainData TransferMainData
        {
            get { return _transferMainData; }
        }

        public static void CheckHeartBeat(object? state)
        {

            TimeSpan difference = DateTime.Now - lastCalledTime;
            if (difference.TotalSeconds > 3)
            {
                // 如果3秒内没有调用OperateExcelModel，将HeartBeat标志位置为0
                _heartBeat = false;
                Log.Warning("超过3秒没有接收到PLC数据和执行UpdateMainData方法，PlcOnline置为0，最后一次接收数据时间为:" + lastCalledTime);
            }
            else
            {
                _heartBeat = true;
            }

        }

        /// <summary>
        /// 更新所有plc数据
        /// </summary>
        /// <param name="pLCData"></param>
        /// 
        public void UpdateMainData(TransferMainData transferMainData)
        {
            _transferMainData = transferMainData;


            // 更新上次调用时间
            lastCalledTime = DateTime.Now;

            _mainData.Time = transferMainData.Time;
            _mainData.PlcOnline = transferMainData.PlcOnline;
            _mainData.DeviceName = transferMainData.DeviceName;
            _mainData.CMD = transferMainData.CMD;
            _mainData.CommID = transferMainData.CommID;
            _mainData.CmdState = transferMainData.CmdState;
            _mainData.X = transferMainData.X;
            _mainData.Y = transferMainData.Y;
            _mainData.Z = transferMainData.Z;
            _mainData.XS = transferMainData.XS;
            _mainData.YS = transferMainData.YS;
            _mainData.ZS = transferMainData.ZS;
            _mainData.AutoModeKey = _mainData.AutoModeKey;
            _mainData.AutoMode = transferMainData.AutoMode;
            _mainData.Paused = transferMainData.Paused;
            _mainData.SoftEMStop = transferMainData.SoftEMStop;
            _mainData.RelaySwitchOK = transferMainData.RelaySwitchOK;
            _mainData.PowerOK = transferMainData.PowerOK;
            _mainData.ManModeHandset = transferMainData.ManModeHandset;
            _mainData.ManModeDriver = transferMainData.ManModeDriver;
            _mainData.ManModeRemoteControl = transferMainData.ManModeRemoteControl;

            _mainData.Weight = transferMainData.Weight;
            _mainData.GrabAngle = transferMainData.GrabAngle;                        
            _mainData.Data = transferMainData.Data;

            _mainData.AlarmMessage = transferMainData.AlarmMessage;
        }



        /// <summary>
        /// 更新ctrl数据
        /// </summary>
        /// <param name="byteLocation"></param>
        /// <param name="bitLocation"></param>
        /// <param name="sign"></param>
        public void UpdateCtrlData(int byteLocation, int bitLocation, bool sign)
        {
            byte item = SetBit(_ctrlData[byteLocation], bitLocation, sign);
            _ctrlData[byteLocation] = item; //设置第几位置  true  代表什么状态
        }

        public MainData MainData
        {
            get { return _mainData; }
        }

        public List<PLCVariableExcelModel> ExcelListModel
        {
            get { return _models; }
            set { _models = value; }
        }

        public byte[] CtrlData
        {
            get { return _ctrlData; }
        }

        private byte SetBit(byte b, int position, bool value)
        {
            if (value)
            {
                // 设置为 1
                return (byte)(b | (1 << position));
            }
            else
            {
                // 设置为 0
                return (byte)(b & ~(1 << position));
            }
        }

        //循环自我增量
        public void CycleSelfIncrement(byte byteValue)
        {
            //用来控制plc心跳
            _ctrlData[1] = byteValue;
        }

        public bool CanSendCmd()
        {
            if(MainData.CmdState==0 || MainData.CmdState == 2)
            {
                return true;
            }
            return false;
        }
    }
}
