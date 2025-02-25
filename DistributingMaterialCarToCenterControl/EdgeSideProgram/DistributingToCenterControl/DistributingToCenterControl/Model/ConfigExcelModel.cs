using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Model
{
    internal class ConfigExcelModel
    {
        [ExcelColumnName("数据名称")]
        public string DataName { get; set; }

        [ExcelColumnName("数据类型")]
        public string DataType { get; set; }

        [ExcelColumnName("变量")]
        public string Variable { get; set; }

        [ExcelColumnName("数值")]
        public string Value { get; set; }

        [ExcelColumnName("变量注释")]
        public string VariableDescription { get; set; }
    }
}
