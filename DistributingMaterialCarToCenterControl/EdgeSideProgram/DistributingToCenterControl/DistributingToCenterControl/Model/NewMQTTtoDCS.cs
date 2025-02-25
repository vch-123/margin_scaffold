using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributingToCenterControl.Model
{
    //后端->边缘测 料格信息
    //频率：5分钟 / 次   ICS/SpanAreaInfoToDCS
    internal class MaterialGrids
    {
        public DateTime Time { get; set; }
        public List<MaterialGridModel> Info { get; set; }
    }

    internal class MaterialGridModel
    {
        public string Code;
        public decimal Weight;
    }


    //后端->边缘测 布料机信息
    //频率：
    //有数据时：5秒/次
    //无数据时：不发送
    //ICS/CarInfoToDCS
    internal class DistributingMaterialCarModel
    {
        public DateTime Time { get; set; }
        public string EqCode { get; set; }
        public bool HasInfo { get; set; }
        public int Y { get; set; }
        public string? SpanAreaCode { get; set; }
    }


    //后端->边缘测 布料机申请停止 / 申请就位
    //频率：
    //需要时 500ms发送一次；不需要时不发送
    //ICS/CarRequest
    internal class DistributingMaterialCarCmdModel
    {
        public DateTime Time { get; set; }
        public string EqCode { get; set; }
        public int Request { get; set; }
    }
}