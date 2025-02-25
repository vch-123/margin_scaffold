using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributingToCenterControl.Model
{

    internal class BackendToEdgeData
    {
        //BC5 东面->  ICC  BC6西面->  DCC
        public string Time { get; set; }

        /// <summary>
        /// 料格存料量 
        /// </summary>
        public int[] CellStockage { get; set; }

        /// <summary>
        /// BC6布料机大车位置
        /// </summary>
        public int LocationDCC { get; set; }

        /// <summary>
        /// BC6所在料格
        /// </summary>
        public int CellDCC { get; set; }

        /// <summary>
        /// BC6申请停止
        /// </summary>
        public bool RequestStopDCC { get; set; }

        /// <summary>
        /// BC6布料就位
        /// </summary>
        public bool FabricReadyDCC { get; set; }

        /// <summary>
        /// BC5布料机大车位置
        /// </summary>
        public int LocationICC { get; set; }

        /// <summary>
        /// BC5所在料格
        /// </summary>
        public int CellICC { get; set; }

        /// <summary>
        /// BC5申请停止
        /// </summary>
        public bool RequestStopICC { get; set; }

        /// <summary>
        /// BC5布料就位
        /// </summary>
        public bool FabricReadyICC { get; set; }
    }
}
