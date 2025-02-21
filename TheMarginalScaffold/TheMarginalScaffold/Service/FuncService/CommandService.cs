using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Client;
using TheMarginalScaffold.Enum;
using TheMarginalScaffold.Model.CMDToPLC;
using TheMarginalScaffold.Model.FromBackendMQTT;

namespace TheMarginalScaffold.Service.FuncService
{
    public class CommandService
    {
        private readonly CacheService _cacheService;
        private readonly UDPClient _udpClient;
        private byte cycleTime;

        public CommandService(CacheService cacheService, UDPClient udpClient)
        {
            _cacheService = cacheService;
            _udpClient = udpClient;
            cycleTime = 0;
        }

        public async void ExecuteCommand(CmdMessage cmdMessage)   //发送RMessage到PLC
        {
            try
            {
                byte[] buffer = new byte[37];
                buffer[0] = 0xCA;
                buffer[1] = Convert.ToByte(cmdMessage.ID);
                buffer[2] = Convert.ToByte(cmdMessage.Cmd);

                AnyToBytes(cmdMessage.Y, 3, buffer);
                byte temp1 = buffer[3]; //调完写一个方法
                buffer[3] = buffer[4];
                buffer[4] = temp1;
                temp1 = buffer[5];
                buffer[5] = buffer[6];
                buffer[6] = temp1;

                AnyToBytes(cmdMessage.X, 7, buffer);
                temp1 = buffer[7];
                buffer[7] = buffer[8];
                buffer[8] = temp1;
                temp1 = buffer[9];
                buffer[9] = buffer[10];
                buffer[10] = temp1;


                AnyToBytes(Convert.ToInt16(cmdMessage.Z), 11, buffer);
                AnyToBytes(cmdMessage.Z, 11, buffer);
                temp1 = buffer[11];
                buffer[11] = buffer[12];
                buffer[12] = temp1;
                temp1 = buffer[13];
                buffer[13] = buffer[14];
                buffer[14] = temp1;



                AnyToBytes(24, 15, buffer);

                temp1 = buffer[15];
                buffer[15] = buffer[16];
                buffer[16] = temp1;
                temp1 = buffer[17];
                buffer[17] = buffer[18];
                buffer[18] = temp1;





                Log.Information($"udp 发送指令坐标为:x:{cmdMessage.X}--y:{cmdMessage.Y}--z:{cmdMessage.Z}");
                await _udpClient.SendCmdData(buffer);
            }
            catch
            {
                Log.Error($"RMessage转化出现问题！请检查 发送指令坐标为:x:{cmdMessage.X}--y:{cmdMessage.Y}--z:{cmdMessage.Z}");
            }
        }
        private byte SetBits(byte b, bool[] bitValues)  //合并字节
        {
            byte result = b;
            for (int i = 0; i < bitValues.Length; i++)
            {
                // 如果bitValues[i]为true，则设置对应位为1
                if (bitValues[i])
                {
                    result |= (byte)(1 << i);
                }
                // 如果bitValues[i]为false，则设置对应位为0
                else
                {
                    result &= (byte)(~(1 << i));
                }
            }
            return result;
        }

        private void AnyToBytes(dynamic value, int start, byte[] bytes)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Copy(buffer, 0, bytes, start, buffer.Length); // 从buffer[0]开始，复制buffer.Length个元素到bytes[start]开始的位置 
        }

        // 执行实时模型的操作
        public void ExecuteRealTime(RealTimeCtrlModel realTimeCtrlModel)
        {
            switch (realTimeCtrlModel.Ctrl)
            {
                case CtrlEnum.SoftwareModeBoot:
                    _cacheService.UpdateCtrlData(3, 3, true);
                    _cacheService.UpdateCtrlData(2, 0, true);
                    break;
                case CtrlEnum.SoftwareModeDisconnect:
                    _cacheService.UpdateCtrlData(3, 3, false);
                    _cacheService.UpdateCtrlData(2, 1, true);
                    break;
                case CtrlEnum.Pause:
                    _cacheService.UpdateCtrlData(2, 3, true);
                    break;
                case CtrlEnum.PauseResume:
                    _cacheService.UpdateCtrlData(2, 5, true);
                    break;
                case CtrlEnum.SoftwareEMStop:
                    _cacheService.UpdateCtrlData(2, 4, true);
                    break;
                case CtrlEnum.SoftwareEMStopResume:
                    _cacheService.UpdateCtrlData(2, 4, false);
                    break;
                case CtrlEnum.FaultReset:
                    _cacheService.UpdateCtrlData(2, 2, true);
                    break;
                case CtrlEnum.ProcessTermination:
                    _cacheService.UpdateCtrlData(2, 7, true);
                    break;
                case CtrlEnum.EncoderReset:
                    _cacheService.UpdateCtrlData(2,6,true);
                    break;
                case CtrlEnum.LightOn:
                    _cacheService.UpdateCtrlData(3,1,true);
                    break;
                case CtrlEnum.LightOff:
                    _cacheService.UpdateCtrlData(3,1,false);
                    break;
                case CtrlEnum.CancelHandsetMode:
                    _cacheService.UpdateCtrlData(3,5,true);
                    break;
            }
        }        
    }
}
