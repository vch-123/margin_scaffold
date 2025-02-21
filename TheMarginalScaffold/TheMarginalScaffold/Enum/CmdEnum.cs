using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMarginalScaffold.Enum
{
    public enum CmdEnum
    {
        /// <summary>
        /// 走行
        /// </summary>
        Walk = 0x11,

        /// <summary>
        /// 抓料
        /// </summary>
        Get = 0x12,

        /// <summary>
        /// 放料
        /// </summary>
        Put = 0x13
    }
}
