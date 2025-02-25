using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributingToCenterControl.Model
{
    internal class TransferMainData
    {
        public string Time { get; set; }

        public int PlcOnline1 { get; set; }
        public int PlcOnline2 { get; set; }

        //布料流程状态
        public int DistributingMaterialProcessState1 { get; set; }
        public int DistributingMaterialProcessState2 { get; set; }

        //定量计量胶带输送机仪表料量累积
        public decimal BeltConveyorAccum1 { get; set; }
        public decimal BeltConveyorAccum2 { get; set; }
        public decimal BeltConveyorAccum3 { get; set; }
        public decimal BeltConveyorAccum4 { get; set; }
        public decimal BeltConveyorAccum5 { get; set; }
        public decimal BeltConveyorAccum6 { get; set; }
        public decimal BeltConveyorAccum7 { get; set; }
        public decimal BeltConveyorAccum8 { get; set; }
        public decimal BeltConveyorAccum9 { get; set; }
        public decimal BeltConveyorAccum10 { get; set; }
        public decimal BeltConveyorAccum11 { get; set; }
        public decimal BeltConveyorAccum12 { get; set; }
        public decimal BeltConveyorAccum13 { get; set; }

        //圆盘给料机电子皮带秤料量累积
        public decimal DiscFeederBeltScaleAccum1 {  get; set; }
        public decimal DiscFeederBeltScaleAccum2 { get; set; }

        //振动筛电子皮带秤料量累积
        public decimal VibScreenBeltScaleAccum1 { get; set; }
        public decimal VibScreenBeltScaleAccum2 { get; set; }

        //料仓称重量
        public decimal MatBinWeight1 {  get; set; }
        public decimal MatBinWeight2 { get; set; }
        public decimal MatBinWeight3 { get; set; }
        public decimal MatBinWeight4 { get; set; }
        public decimal MatBinWeight5 { get; set; }
        public decimal MatBinWeight6 { get; set; }
        public decimal MatBinWeight7 { get; set; }
        public decimal MatBinWeight8 { get; set; }
        public decimal MatBinWeight9 { get; set; }
        public decimal MatBinWeight10 { get; set; }
        public decimal MatBinWeight11 { get; set; }
        public decimal MatBinWeight12 { get; set; }
        public decimal MatBinWeight13 { get; set; }

        //圆盘给料机料仓称重量
        public decimal DiscFeederBinWeight1 { get; set; }
        public decimal DiscFeederBinWeight2 { get; set; }

        //振动筛料仓称重量
        public decimal VibScreenBinWeight1 { get; set; }
        public decimal VibScreenBinWeight2 { get; set; }

        //料仓下料速度
        public decimal MatBinDownSpeed1 { get; set; }
        public decimal MatBinDownSpeed2 { get; set; }
        public decimal MatBinDownSpeed3 { get; set; }
        public decimal MatBinDownSpeed4 { get; set; }
        public decimal MatBinDownSpeed5 { get; set; }
        public decimal MatBinDownSpeed6 { get; set; }
        public decimal MatBinDownSpeed7 { get; set; }
        public decimal MatBinDownSpeed8 { get; set; }
        public decimal MatBinDownSpeed9 { get; set; }
        public decimal MatBinDownSpeed10 { get; set; }
        public decimal MatBinDownSpeed11 { get; set; }
        public decimal MatBinDownSpeed12 { get; set; }
        public decimal MatBinDownSpeed13 { get; set; }

        //圆盘给料机料仓下料速度
        public decimal DiscFeederDischargeRate1 {  get; set; }
        public decimal DiscFeederDischargeRate2 { get; set; }

        //振动筛料仓下料速度
        public decimal VibScreenDischargeRate1 { get; set; }
        public decimal VibScreenDischargeRate2 { get; set; }

        //料仓物位
        public decimal MatBinObjectPoistion1 {  get; set; }
        public decimal MatBinObjectPoistion2 { get; set; }
        public decimal MatBinObjectPoistion3 { get; set; }
        public decimal MatBinObjectPoistion4 { get; set; }
        public decimal MatBinObjectPoistion5 { get; set; }
        public decimal MatBinObjectPoistion6 { get; set; }
        public decimal MatBinObjectPoistion7 { get; set; }
        public decimal MatBinObjectPoistion8 { get; set; }
        public decimal MatBinObjectPoistion9 { get; set; }
        public decimal MatBinObjectPoistion10 { get; set; }
        public decimal MatBinObjectPoistion11 { get; set; }
        public decimal MatBinObjectPoistion12 { get; set; }
        public decimal MatBinObjectPoistion13 { get; set; }

        //圆盘给料机料仓物位
        public decimal DiscFeederBinLevel1 { get;set; }
        public decimal DiscFeederBinLevel2 {  get;set; }

        //振动筛料仓物位
        public decimal VibScreenBinLevel1 { get;set; }
        public decimal VibScreenBinLevel2 { get; set; }

        //模拟量备用
        public decimal SimulationPreserve1 {  get;set; }
        public decimal SimulationPreserve2 { get; set; }
        public decimal SimulationPreserve3 { get; set; }
        public decimal SimulationPreserve4 { get; set; }
        public decimal SimulationPreserve5 { get; set; }
        public decimal SimulationPreserve6 { get; set; }
        public decimal SimulationPreserve7 { get; set; }
        public decimal SimulationPreserve8 { get; set; }
        public decimal SimulationPreserve9 { get; set; }
        public decimal SimulationPreserve10 { get; set; }
        public decimal SimulationPreserve11 { get; set; }
        public decimal SimulationPreserve12 { get; set; }
        public decimal SimulationPreserve13 { get; set; }
        public decimal SimulationPreserve14 { get; set; }
        public int SimulationPreserve15 { get; set; }
        public int SimulationPreserve16 { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
