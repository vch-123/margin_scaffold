using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Enum;
using TheMarginalScaffold.Model.CMDToPLC;
using TheMarginalScaffold.Model.FromBackendMQTT;

namespace TheMarginalScaffold.Service.FuncService
{
    public class CommandQueueService
    {
        private CacheService _cacheService;
        private ConfigService _configService;
        private ConcurrentQueue<CmdMessage> _commandQueue = new();
        private ConcurrentQueue<RealTimeCtrlModel> realTimeQueue = new();
        private readonly CommandService _commandService;
        public CommandQueueService(CommandService commandService, ConfigService configService, CacheService cacheService)
        {
            _cacheService = cacheService;
            _commandService = commandService;
            _configService = configService;
        }

        //命令和实时消息CMD入队的位置
        public void EnqueueWalkCommand(string walkMessage)//走行入队
        {
            var walkCmdModel = JsonConvert.DeserializeObject<WalkCmdModel>(walkMessage);
            if (walkCmdModel != null && _cacheService.CanSendCmd())
            {
                CmdMessage cmdMessage = new CmdMessage()
                {
                    ID = walkCmdModel.Id,
                    Cmd = CmdEnum.Walk,

                    X = walkCmdModel.X,
                    Y = walkCmdModel.Y,
                    
                };
                _commandQueue.Enqueue(cmdMessage); //如果不为空，那么命令入队
            }
            else
            {
                Log.Error($"mqtt在CmdState非0/2时收到了新CMD消息★：{walkCmdModel.Id}  CMDSTATE:{_cacheService.MainData.CmdState}");
            }
        }

        public void EnqueueGetCommand(string getMessage)//取料入队
        {
            var getCmdModel = JsonConvert.DeserializeObject<GetCmdModel>(getMessage);
            if (getCmdModel != null && _cacheService.CanSendCmd())
            {
                CmdMessage cmdMessage = new CmdMessage()
                {
                    ID = getCmdModel.Id,
                    Cmd = CmdEnum.Get,

                    Z = getCmdModel.Z,
                    
                };
                _commandQueue.Enqueue(cmdMessage); //如果不为空，那么命令入队
            }
            else
            {
                Log.Error($"mqtt在CmdState非0/2时收到了新CMD消息★：{getCmdModel.Id}  CMDSTATE:{_cacheService.MainData.CmdState}");
            }
        }

        public void EnqueuePutCommand(string putMessage)//放料入队
        {
            var putCmdModel = JsonConvert.DeserializeObject<PutCmdModel>(putMessage);
            if (putCmdModel != null && _cacheService.CanSendCmd())
            {
                CmdMessage cmdMessage = new CmdMessage()
                {
                    ID = putCmdModel.Id,
                    Cmd = CmdEnum.Put,

                    Z = putCmdModel.Z,

                };
                _commandQueue.Enqueue(cmdMessage); //如果不为空，那么命令入队
            }
            else
            {
                Log.Error($"mqtt在CmdState非0/2时收到了新CMD消息★：{putCmdModel.Id}  CMDSTATE:{_cacheService.MainData.CmdState}");
            }
        }

        public void EnqueueRealTimeCtrl(string realTimeCtrl)
        {
            var realTimeCtrlModel = JsonConvert.DeserializeObject<RealTimeCtrlModel>(realTimeCtrl);
            if (realTimeCtrlModel != null)
            {
                realTimeQueue.Enqueue(realTimeCtrlModel);
            }
        }

        public async Task ExecuteCommand()
        {
            if (_commandQueue.TryDequeue(out var cmdMessage))
            {
                switch (cmdMessage.Cmd)
                {
                    case CmdEnum.Walk:
                        cmdMessage.Z = _configService.SafeHeight;
                        break;

                    case CmdEnum.Get:

                        cmdMessage.X = _cacheService.MainData.X;
                        cmdMessage.Y = _cacheService.MainData.Y;
                        break;

                    case CmdEnum.Put:

                        cmdMessage.X = _cacheService.MainData.X;
                        cmdMessage.Y = _cacheService.MainData.Y;
                        break;
                }
                _commandService.ExecuteCommand(cmdMessage);                         
            }
        }

        public async Task ExecuteRealTimeCtrl()
        {
            if (realTimeQueue.TryDequeue(out var rMessage))
            {
                _commandService.ExecuteRealTime(rMessage);
            }
        }
               
    }
}
