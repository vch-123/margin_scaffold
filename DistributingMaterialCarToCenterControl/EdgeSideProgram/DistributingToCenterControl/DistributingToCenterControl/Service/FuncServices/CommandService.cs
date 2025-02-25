using EdgeSideProgramScaffold.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeSideProgramScaffold.Client;
using DistributingToCenterControl.Model;

namespace EdgeSideProgramScaffold.Service.FuncServices
{
    internal class CommandService
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

        public async void SendStateData()
        {
            //拿到最新的后端状态
            BackendToEdgeData backendToEdgeData = _cacheService.GetBackendToEdgeData();

            //21个料格 INT类型  总共42个字节
            //BC5/BC6  位置4个 所在料格2个(申请停止, 布料就位)1个 合计发7个BC6 两车共14个 
            //总共发56个

            //从BackendToEdgeData类转换为发送给PLC字节数组
            byte[] backendStateBytes = new byte[58];

            for (int i = 0; i < backendToEdgeData.CellStockage.Length; i++)
            {
                if (i == 0)
                {
                    AnyToBytes((short)backendToEdgeData.CellStockage[i], i, backendStateBytes);
                }
                else
                {
                    AnyToBytes((short)backendToEdgeData.CellStockage[i], 2*i, backendStateBytes);
                }
            }

            AnyToBytes(backendToEdgeData.LocationICC, 42, backendStateBytes);
            AnyToBytes(backendToEdgeData.CellICC, 46, backendStateBytes);

            bool[] boolArray1 =
            {
                backendToEdgeData.RequestStopICC,
                backendToEdgeData.FabricReadyICC,
                false, false, false, false, false,false
            };
            backendStateBytes[48] = SetBits(backendStateBytes[48], boolArray1);

            AnyToBytes(backendToEdgeData.LocationDCC, 49, backendStateBytes);
            AnyToBytes((short)backendToEdgeData.CellDCC, 53, backendStateBytes);

            bool[] boolArray2 =
            {
                backendToEdgeData.RequestStopDCC,
                backendToEdgeData.FabricReadyDCC,
                false, false, false, false, false,false
            };
            backendStateBytes[55] = SetBits(backendStateBytes[55], boolArray2);
            backendStateBytes[56] = 15;
            backendStateBytes[57] = 15;

            await _udpClient.SendBackendStateData(backendStateBytes);
            
            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            }

            Log.Information($"以下三行是边缘测接收到后端的数据后向PLC发送的数据:");

            byte[] bb1 = new byte[20];
            //for (int i = 0; i < 20; i++)
            //{
            //    bb1[i]=backendStateBytes[i];
            //}

            //byte[] bb2 = new byte[20];
            //for (int i = 20; i < 40; i++)
            //{
            //    bb2[i-20] = backendStateBytes[i];
            //}

            //byte[] bb3 = new byte[16];
            //for (int i = 40; i < 57; i++)
            //{
            //    bb3[i-40] = backendStateBytes[i];
            //}

            //string hexString1 = String.Join(" ", bb1.Select(b => $"0x{b:X2}"));
            //string hexString2 = String.Join(" ", bb2.Select(b => $"0x{b:X2}"));
            //string hexString3 = String.Join(" ", bb3.Select(b => $"0x{b:X2}"));

           
            //Log.Information(hexString1);
            //Log.Information(hexString2);
            //Log.Information(hexString3);
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
    }
}
