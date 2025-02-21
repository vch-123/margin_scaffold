using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Enum;

namespace TheMarginalScaffold.Model.CMDToPLC
{
    public class CmdMessage
    {
        public byte Head { get; set; } = 0xCA;
        
        /// <summary>
        /// 报文ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 命令码
        /// </summary>
        public CmdEnum Cmd { get; set; }

        /// <summary>
        /// X坐标4个字节
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标4个字节
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Z坐标4个字节
        /// </summary>
        public int Z { get; set; }
       
        //三个备用
        public int? Preserve1 {  get; set; }
        public byte? Preserve2 { get; set; }
        public byte? Preserve3 { get; set; }
    }
}
