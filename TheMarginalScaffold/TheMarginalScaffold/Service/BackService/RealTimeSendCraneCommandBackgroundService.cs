using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Service.BackService
{
    public class RealTimeSendCraneCommandBackgroundService : BackgroundService
    {
        private readonly CommandQueueService _commandQueueService;
        public RealTimeSendCraneCommandBackgroundService(CommandQueueService commandQueueService)
        {
            _commandQueueService = commandQueueService;
        }

        // 重写BackgroundService类的ExecuteAsync方法，提供后台服务的执行逻辑。
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 使用while循环，只要没有接收到停止请求，就持续执行。
            while (!stoppingToken.IsCancellationRequested)
            {
                // 调用_commandQueueService的ExecuteRealTime方法，执行实时命令。
                await _commandQueueService.ExecuteRealTimeCtrl();    //消费   也是在MQTT里入队
                // 等待一段指定的时间（这里是100毫秒），同时检查是否有停止请求。
                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
