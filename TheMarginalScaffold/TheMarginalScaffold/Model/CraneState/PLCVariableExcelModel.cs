using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMarginalScaffold.Model.CraneState
{
    public class PLCVariableExcelModel
    {
        [ExcelColumnName("数据名称")]
        public string DataName { get; set; }


        [ExcelColumnName("数据类型")]
        public string DataType { get; set; }


        [ExcelColumnName("数据PLC地址")]
        public string DataPlcAddress { get; set; }


        [ExcelColumnName("边缘侧地址")]
        public string EdgeMeasurementAddress { get; set; }


        [ExcelColumnName("机构名称")]
        public string InstitutionName { get; set; }


        [ExcelColumnName("是否显示")]
        public string IsShow { get; set; }


        [ExcelColumnName("是否保存")]
        public string IsSave { get; set; }


        [ExcelColumnName("是否报警")]
        public string IsAlarm { get; set; }


        [ExcelColumnName("报警值")]
        public string AlarmValue { get; set; }


        [ExcelColumnName("主要字段名")]
        public string MainParaName { get; set; }


        [ExcelColumnName("禁用")]
        public string IsEnable { get; set; }


        [ExcelColumnName("故障")]
        public string IsFault { get; set; }
    }
}
