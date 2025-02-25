using EdgeSideProgramScaffold.Service.FuncServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.BackgroundServices
{
    internal class DisplayMaindataBackgroundService : BackgroundService
    {
        private readonly CacheService _cacheService;
        public DisplayMaindataBackgroundService(CacheService cacheService)
        {
            _cacheService = cacheService;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                Type type = _cacheService.TransferMainData.GetType();
                PropertyInfo[] properties = type.GetProperties();

                Console.WriteLine($"Properties of {type.Name}:");
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        var value = property.GetValue(_cacheService.TransferMainData);
                        Console.WriteLine($"\t{property.Name} = {value}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\tError accessing {property.Name}: {ex.Message}");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                //await Task.Delay(_configService.Output_Interval);
                await Task.Delay(5000);            
            }
        }
    }
}
