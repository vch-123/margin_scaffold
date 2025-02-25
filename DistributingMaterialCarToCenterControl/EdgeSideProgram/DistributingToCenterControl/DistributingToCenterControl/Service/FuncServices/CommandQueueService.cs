using DistributingToCenterControl.Model;
using EdgeSideProgramScaffold.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.FuncServices
{
    internal class CommandQueueService
    {
        private CacheService _cacheService;
        private ConfigService _configService;
        private ConcurrentQueue<BackendToEdgeData> _commandQueue = new();        
        private readonly CommandService _commandService;
        public CommandQueueService(CommandService commandService, ConfigService configService, CacheService cacheService)
        {
            _cacheService = cacheService;
            _commandService = commandService;
            _configService = configService;
        }

        /// <summary>
        /// 需要新增，后端发的状态
        /// </summary>
        /// <param name="walkMessage"></param>
        public void EnqueueCurrentState(string currentStateMessage)
        {
            ///把后端发的字符串反序列化为状态信息
            var currentState = JsonConvert.DeserializeObject<BackendToEdgeData>(currentStateMessage);
            if (currentState != null)
            {
                _cacheService.UpdateBackendToEdgeData(currentState);
                _commandQueue.Enqueue(currentState);
            }
        }
        
        public void EnqueueMaterialGrids(string materialGridsMessage)
        {           
            _cacheService.UpdateMaterialGrids(materialGridsMessage); //更新完了  剩下的一会一起讨论
            _commandQueue.Enqueue(_cacheService.GetBackendToEdgeData());
        }

        public void EnqueueDistributingCars(string distributingMaterialCarsMessage)
        {
            _cacheService.UpdateDistributingMaterialCars(distributingMaterialCarsMessage); //更新完了  剩下的一会一起讨论
            _commandQueue.Enqueue(_cacheService.GetBackendToEdgeData());
        }

        public void EnqueueDistributingCarsCmd(string distributingMaterialCarsMessage)
        {
            _cacheService.UpdateDistributingCarsCmd(distributingMaterialCarsMessage);
            _commandQueue.Enqueue(_cacheService.GetBackendToEdgeData());
        }

                   
        public async Task ExecuteState()
        {
            if (_commandQueue.TryDequeue(out var stateMessage))
            {               
                _commandService.SendStateData();               
            }
        }              
    }
}
