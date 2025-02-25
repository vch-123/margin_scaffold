using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Model
{


    //17个料仓 其中有13个普通料仓，2个圆盘给料机料仓，2个振动筛料仓料仓  料仓Name如下:
    //MatHouseA01  -  MatHouseA13  
    //MatHouseB01  -  MatHouseB02
    //MatHouseC01  -  MatHouseC02
    
    public class MaterialWarehouseObj
    {
       
        public string MatHouseName {  get; set; }

        //定量计算胶带输送机仪表料量累计
        public decimal BeltConveyorAccum { get; set; }

        //料仓称重量
        public decimal MatBinWeight { get; set; }

        //料仓下料速度
        public decimal MatBinDownSpeed { get; set; }

        //料仓物位
        public decimal MatBinObjectPosition { get; set; }
    }
    
    //布料机
    public class ECarObj
    {
        //设备名
        public string DeviceName { get; set; }

        //与PLC连接
        public int PlcOnline { get; set; }

        /// <summary>
        /// 布料流程状态"当中控收到ICS系统发的布料就位，状态发30，表示准备上料
        ///当 中控的皮带启动以后， 状态发40，表示正在上料
        ///当中控认为布料完成了，没有料量，并已停止流程，发50，表示布料完成，ICS系统可以结束布料任务"
        /// </summary>
        public int DistributingMaterialProcessState {  get; set; }
    }

    public class MainData
    {       
        //发送时时间
        public string Time { get; set; }

        //此位为0  所有数据不判断
        //此位为1  PLC发送真实数据
        public int GeneralPlcOnline { get; set; }

        // 两个布料机情况
        public List<ECarObj> ECars { get; set; }
    
        

        // 17个料仓情况
        public List<MaterialWarehouseObj> MaterialWarehouses { get; set; }



        //构造初始化
        public MainData()
        {
            ECars = new List<ECarObj>
            {
                new ECarObj(), 
                new ECarObj()  
            };

            MaterialWarehouses = new List<MaterialWarehouseObj>();
            for (int i = 0; i < 17; i++)
            {
                MaterialWarehouses.Add(new MaterialWarehouseObj());
            }
        }

    }
}

