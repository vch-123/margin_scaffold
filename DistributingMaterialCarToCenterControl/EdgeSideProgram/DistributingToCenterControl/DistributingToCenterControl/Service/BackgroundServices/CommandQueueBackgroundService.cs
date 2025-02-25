using EdgeSideProgramScaffold.Service.FuncServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.BackgroundServices
{
    internal class CommandQueueBackgroundService : BackgroundService
    {

        private readonly CommandQueueService _commandQueueService;
        public CommandQueueBackgroundService(CommandQueueService commandQueueService)
        {
            _commandQueueService = commandQueueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _commandQueueService.ExecuteState();   //这里就是给PLC发RMessage
                await Task.Delay(200, stoppingToken);
            }
        }
    }
}
