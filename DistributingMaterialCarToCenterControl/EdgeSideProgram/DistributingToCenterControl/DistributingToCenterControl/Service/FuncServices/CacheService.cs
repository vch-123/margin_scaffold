using DistributingToCenterControl.Model;
using EdgeSideProgramScaffold.Model;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.FuncServices
{
    internal class CacheService
    {
        private static Timer? _timer = new Timer(CheckHeartBeat, null, 0, 500);
        public static bool _heartBeat = true; // 默认HeartBeat标志为true  起重机PLC数据的心跳
        private static DateTime lastCalledTime;

        public static bool _heartBeatDCC = true; //DCC命令的后端心跳
        private static DateTime lastCalledTimeDCC;
        public static bool _heartBeatICC = true; //ICC命令的后端心跳
        private static DateTime lastCalledTimeICC;

        public  TransferMainData TransferMainData { get; set; }=new TransferMainData();
        private MainData _mainData = new();
        

        private static BackendToEdgeData _backendToEdgeData = new() { CellStockage = new int[21] };//储存后端发的数据 定期发给PLC
        
        private List<ImportExcelModel> _models = new List<ImportExcelModel>(); 
        private readonly ConfigService _configService;

        public CacheService(ConfigService configService)
        {
            _configService = configService;
        }

        public static void CheckHeartBeat(object? state)
        {

            TimeSpan difference1 = DateTime.Now - lastCalledTime;
            if (difference1.TotalSeconds > 3)
            {
                // 如果3秒内没有调用OperateExcelModel，将HeartBeat标志位置为0
                _heartBeat = false;
                Log.Warning("超过3秒没有接收到PLC数据和执行UpdateMainData方法，PlcOnline置为0，最后一次接收数据时间为:" + lastCalledTime);
            }
            else
            {
                _heartBeat = true;
            }


            TimeSpan differenceDCC = DateTime.Now - lastCalledTimeDCC;
            if (differenceDCC.TotalSeconds > 3)
            {
                // 如果3秒内没有调用OperateExcelModel，将HeartBeat标志位置为0
                _heartBeatDCC = false;
                _backendToEdgeData.RequestStopDCC = false;
                _backendToEdgeData.FabricReadyDCC = false;

            }
            else
            {
                _heartBeatDCC = true;
            }


            TimeSpan differenceICC = DateTime.Now - lastCalledTimeICC;
            if (differenceICC.TotalSeconds > 3)
            {
                // 如果3秒内没有调用OperateExcelModel，将HeartBeat标志位置为0
                _heartBeatICC = false;
                _backendToEdgeData.RequestStopICC = false;
                _backendToEdgeData.FabricReadyICC = false;

            }
            else
            {
                _heartBeatICC = true;
            }
        }
       
        public void UpdateTransferData(TransferMainData transferData)
        {
            this.TransferMainData = transferData;
        }

        public void UpdateMainData(TransferMainData mainData)
        {
            lastCalledTime = DateTime.Now;

            //时间
            _mainData.Time = mainData.Time;

            //设备名
            _mainData.ECars[0].DeviceName = "ECar_DCC";
            _mainData.ECars[1].DeviceName = "ECar_ICC";

            //online
            _mainData.ECars[0].PlcOnline = mainData.PlcOnline2;
            _mainData.ECars[1].PlcOnline = mainData.PlcOnline1;

            //布料状态
            _mainData.ECars[0].DistributingMaterialProcessState = mainData.DistributingMaterialProcessState2;
            _mainData.ECars[1].DistributingMaterialProcessState = mainData.DistributingMaterialProcessState1;

            //定量计算胶带输送机仪表料量累计
            _mainData.MaterialWarehouses[0].MatHouseName = "MatHouseA01";
            _mainData.MaterialWarehouses[0].BeltConveyorAccum = mainData.BeltConveyorAccum1;
            _mainData.MaterialWarehouses[0].MatBinWeight = mainData.MatBinWeight1;
            _mainData.MaterialWarehouses[0].MatBinDownSpeed= mainData.MatBinDownSpeed1;
            _mainData.MaterialWarehouses[0].MatBinObjectPosition = mainData.MatBinObjectPoistion1;

            _mainData.MaterialWarehouses[1].MatHouseName = "MatHouseA02";
            _mainData.MaterialWarehouses[1].BeltConveyorAccum = mainData.BeltConveyorAccum2;
            _mainData.MaterialWarehouses[1].MatBinWeight = mainData.MatBinWeight2;
            _mainData.MaterialWarehouses[1].MatBinDownSpeed = mainData.MatBinDownSpeed2;
            _mainData.MaterialWarehouses[1].MatBinObjectPosition = mainData.MatBinObjectPoistion2;

            _mainData.MaterialWarehouses[2].MatHouseName = "MatHouseA03";
            _mainData.MaterialWarehouses[2].BeltConveyorAccum = mainData.BeltConveyorAccum3;
            _mainData.MaterialWarehouses[2].MatBinWeight = mainData.MatBinWeight3;
            _mainData.MaterialWarehouses[2].MatBinDownSpeed = mainData.MatBinDownSpeed3;
            _mainData.MaterialWarehouses[2].MatBinObjectPosition = mainData.MatBinObjectPoistion3;

            _mainData.MaterialWarehouses[3].MatHouseName = "MatHouseA04";
            _mainData.MaterialWarehouses[3].BeltConveyorAccum = mainData.BeltConveyorAccum4;
            _mainData.MaterialWarehouses[3].MatBinWeight = mainData.MatBinWeight4;
            _mainData.MaterialWarehouses[3].MatBinDownSpeed = mainData.MatBinDownSpeed4;
            _mainData.MaterialWarehouses[3].MatBinObjectPosition = mainData.MatBinObjectPoistion4;

            _mainData.MaterialWarehouses[4].MatHouseName = "MatHouseA05";
            _mainData.MaterialWarehouses[4].BeltConveyorAccum = mainData.BeltConveyorAccum5;
            _mainData.MaterialWarehouses[4].MatBinWeight = mainData.MatBinWeight5;
            _mainData.MaterialWarehouses[4].MatBinDownSpeed = mainData.MatBinDownSpeed5;
            _mainData.MaterialWarehouses[4].MatBinObjectPosition = mainData.MatBinObjectPoistion5;

            _mainData.MaterialWarehouses[5].MatHouseName = "MatHouseA06";
            _mainData.MaterialWarehouses[5].BeltConveyorAccum = mainData.BeltConveyorAccum6;
            _mainData.MaterialWarehouses[5].MatBinWeight = mainData.MatBinWeight6;
            _mainData.MaterialWarehouses[5].MatBinDownSpeed = mainData.MatBinDownSpeed6;
            _mainData.MaterialWarehouses[5].MatBinObjectPosition = mainData.MatBinObjectPoistion6;

            _mainData.MaterialWarehouses[6].MatHouseName = "MatHouseA07";
            _mainData.MaterialWarehouses[6].BeltConveyorAccum = mainData.BeltConveyorAccum7;
            _mainData.MaterialWarehouses[6].MatBinWeight = mainData.MatBinWeight7;
            _mainData.MaterialWarehouses[6].MatBinDownSpeed = mainData.MatBinDownSpeed7;
            _mainData.MaterialWarehouses[6].MatBinObjectPosition = mainData.MatBinObjectPoistion7;

            _mainData.MaterialWarehouses[7].MatHouseName = "MatHouseA08";
            _mainData.MaterialWarehouses[7].BeltConveyorAccum = mainData.BeltConveyorAccum8;
            _mainData.MaterialWarehouses[7].MatBinWeight = mainData.MatBinWeight8;
            _mainData.MaterialWarehouses[7].MatBinDownSpeed = mainData.MatBinDownSpeed8;
            _mainData.MaterialWarehouses[7].MatBinObjectPosition = mainData.MatBinObjectPoistion8;

            _mainData.MaterialWarehouses[8].MatHouseName = "MatHouseA09";
            _mainData.MaterialWarehouses[8].BeltConveyorAccum = mainData.BeltConveyorAccum9;
            _mainData.MaterialWarehouses[8].MatBinWeight = mainData.MatBinWeight9;
            _mainData.MaterialWarehouses[8].MatBinDownSpeed = mainData.MatBinDownSpeed9;
            _mainData.MaterialWarehouses[8].MatBinObjectPosition = mainData.MatBinObjectPoistion9;

            _mainData.MaterialWarehouses[9].MatHouseName = "MatHouseA10";
            _mainData.MaterialWarehouses[9].BeltConveyorAccum = mainData.BeltConveyorAccum10;
            _mainData.MaterialWarehouses[9].MatBinWeight = mainData.MatBinWeight10;
            _mainData.MaterialWarehouses[9].MatBinDownSpeed = mainData.MatBinDownSpeed10;
            _mainData.MaterialWarehouses[9].MatBinObjectPosition = mainData.MatBinObjectPoistion10;

            _mainData.MaterialWarehouses[10].MatHouseName = "MatHouseA11";
            _mainData.MaterialWarehouses[10].BeltConveyorAccum = mainData.BeltConveyorAccum11;
            _mainData.MaterialWarehouses[10].MatBinWeight = mainData.MatBinWeight11;
            _mainData.MaterialWarehouses[10].MatBinDownSpeed = mainData.MatBinDownSpeed11;
            _mainData.MaterialWarehouses[10].MatBinObjectPosition = mainData.MatBinObjectPoistion11;

            _mainData.MaterialWarehouses[11].MatHouseName = "MatHouseA12";
            _mainData.MaterialWarehouses[11].BeltConveyorAccum = mainData.BeltConveyorAccum12;
            _mainData.MaterialWarehouses[11].MatBinWeight = mainData.MatBinWeight12;
            _mainData.MaterialWarehouses[11].MatBinDownSpeed = mainData.MatBinDownSpeed12;
            _mainData.MaterialWarehouses[11].MatBinObjectPosition = mainData.MatBinObjectPoistion12;

            _mainData.MaterialWarehouses[12].MatHouseName = "MatHouseA13";
            _mainData.MaterialWarehouses[12].BeltConveyorAccum = mainData.BeltConveyorAccum13;
            _mainData.MaterialWarehouses[12].MatBinWeight = mainData.MatBinWeight13;
            _mainData.MaterialWarehouses[12].MatBinDownSpeed = mainData.MatBinDownSpeed13;
            _mainData.MaterialWarehouses[12].MatBinObjectPosition = mainData.MatBinObjectPoistion13;


            _mainData.MaterialWarehouses[13].MatHouseName = "MatHouseB01";
            _mainData.MaterialWarehouses[13].BeltConveyorAccum = mainData.DiscFeederBeltScaleAccum1;
            _mainData.MaterialWarehouses[13].MatBinWeight = mainData.DiscFeederBinWeight1;
            _mainData.MaterialWarehouses[13].MatBinDownSpeed = mainData.DiscFeederDischargeRate1;
            _mainData.MaterialWarehouses[13].MatBinObjectPosition = mainData.DiscFeederBinLevel1;

            _mainData.MaterialWarehouses[14].MatHouseName = "MatHouseB02";
            _mainData.MaterialWarehouses[14].BeltConveyorAccum = mainData.DiscFeederBeltScaleAccum2;
            _mainData.MaterialWarehouses[14].MatBinWeight = mainData.DiscFeederBinWeight2;
            _mainData.MaterialWarehouses[14].MatBinDownSpeed = mainData.DiscFeederDischargeRate2;
            _mainData.MaterialWarehouses[14].MatBinObjectPosition = mainData.DiscFeederBinLevel2;

            _mainData.MaterialWarehouses[15].MatHouseName = "MatHouseC01";
            _mainData.MaterialWarehouses[15].BeltConveyorAccum = mainData.VibScreenBeltScaleAccum1;
            _mainData.MaterialWarehouses[15].MatBinWeight = mainData.VibScreenBinWeight1;
            _mainData.MaterialWarehouses[15].MatBinDownSpeed = mainData.VibScreenDischargeRate1;
            _mainData.MaterialWarehouses[15].MatBinObjectPosition = mainData.VibScreenBinLevel1;

            _mainData.MaterialWarehouses[16].MatHouseName = "MatHouseC02";
            _mainData.MaterialWarehouses[16].BeltConveyorAccum = mainData.VibScreenBeltScaleAccum2;
            _mainData.MaterialWarehouses[16].MatBinWeight = mainData.VibScreenBinWeight2;
            _mainData.MaterialWarehouses[16].MatBinDownSpeed = mainData.VibScreenDischargeRate2;
            _mainData.MaterialWarehouses[16].MatBinObjectPosition = mainData.VibScreenBinLevel2;

           
        }

        public MainData MainData
        {
            get { return _mainData; }
        }

        public List<ImportExcelModel> ExcelListModel
        {
            get { return _models; }
            set { _models = value; }
        }
                 
        public void UpdateBackendToEdgeData(BackendToEdgeData backendToEdgeData)
        {
           _backendToEdgeData = backendToEdgeData;
        }

        public BackendToEdgeData GetBackendToEdgeData()
        {
            return _backendToEdgeData;
        }


        public void UpdateMaterialGrids(string materialGridsMessage)
        {
            var materialGrids = JsonConvert.DeserializeObject<MaterialGrids>(materialGridsMessage);
            int count=materialGrids.Info.Count;
            for (int i = 0; i < count; i++)
            {
                int MaterialGridNum = (int)materialGrids.Info[i].Weight;
                _backendToEdgeData.CellStockage[i]= MaterialGridNum;
            }
            //到这里就算转化完成了
        }

        public void UpdateDistributingMaterialCars(string distributingMaterialCarsMessage)
        {
            var distributingMaterialCar = JsonConvert.DeserializeObject<DistributingMaterialCarModel>(distributingMaterialCarsMessage);
            //这只是一个车的信息，通过EqCode是 ECar_ICC 或者是 ECar_DCC 分辨    其中ICC是进口矿布料机   DCC是国产矿布料机
            string EqCode = distributingMaterialCar.EqCode;
            switch(EqCode)
            {
                case "ECar_DCC":
                    int materialGridNo;
                    if (distributingMaterialCar.SpanAreaCode == null)
                    {
                        materialGridNo = 0;
                    }
                     materialGridNo = int.Parse(distributingMaterialCar.SpanAreaCode.Substring(1));

                    int carY = distributingMaterialCar.Y;
                    _backendToEdgeData.LocationDCC=carY; ;
                    _backendToEdgeData.CellDCC = materialGridNo;
                    break;
                case "ECar_ICC":
                    int materialGridNo1;
                    if (distributingMaterialCar.SpanAreaCode == null)
                    {
                        materialGridNo1 = 0;
                    }
                    materialGridNo1 = int.Parse(distributingMaterialCar.SpanAreaCode.Substring(1));

                    int carYY = distributingMaterialCar.Y;
                    _backendToEdgeData.LocationDCC = carYY; ;
                    _backendToEdgeData.CellDCC = materialGridNo1;
                    break;
                default:
                    Log.Error("Eqcode不是ECar_DCC或者是ECar_ICC");
                    break;
            }
            //这里更新完了两个布料机信息
        }

        public void UpdateDistributingCarsCmd(string distributingMaterialCarsMessage)
        {
            var distributingMaterialCarCmd = JsonConvert.DeserializeObject<DistributingMaterialCarCmdModel>(distributingMaterialCarsMessage);
            switch (distributingMaterialCarCmd.EqCode)
            {
                case "ECar_DCC":
                    if (distributingMaterialCarCmd.Request == 0)//0申请停止  1布料就位
                    {
                        _backendToEdgeData.RequestStopDCC = true;
                        lastCalledTimeDCC = DateTime.Now;
                    }
                    else if (distributingMaterialCarCmd.Request == 1)//0申请停止  1布料就位
                    {
                        _backendToEdgeData.FabricReadyDCC = true;
                        lastCalledTimeDCC = DateTime.Now;
                    }
                    break;
                case "ECar_ICC":
                    if (distributingMaterialCarCmd.Request == 0)//0申请停止  1布料就位
                    {
                        _backendToEdgeData.RequestStopICC = true;
                        lastCalledTimeICC = DateTime.Now;
                    }
                    else if (distributingMaterialCarCmd.Request == 1)//0申请停止  1布料就位
                    {
                        _backendToEdgeData.FabricReadyICC = true;
                        lastCalledTimeICC = DateTime.Now;
                    }
                    break;
                default:
                    Log.Error("<CMD>Eqcode不是ECar_DCC或者是ECar_ICC");
                    break;
            }
        }

    }
}
