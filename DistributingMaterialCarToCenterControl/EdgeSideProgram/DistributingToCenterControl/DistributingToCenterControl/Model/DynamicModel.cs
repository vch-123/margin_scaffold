using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Model
{
    /// <summary>
    /// PLC的数据类型 
    /// </summary>
    internal class DynamicModel
    {
        public static string DINT = "DINT";     //双整数  4B
        public static string REAL = "REAL";     //浮点数  4B
        public static string BOOL = "BOOL";     //布尔    1b  1bit
        public static string DWORD = "DWORD";   //双字    待定
        public static string INT = "INT";       //整数    2B 
        public static string BYTE = "BYTE";     //字节    1B
    }
}
