using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Service.BackService
{
    public class CtrlHeartBeatBackgroundService : BackgroundService
    {
        private readonly CacheService _cacheService;

        public CtrlHeartBeatBackgroundService(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _cacheService.UpdateCtrlData(3, 0, true);
                await Task.Delay(1000);
                _cacheService.UpdateCtrlData(3, 0, false);
                await Task.Delay(1000);
            }
        }
    }
}
