using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Service.BackService
{
    public class DisplayMainDataBackgroundService : BackgroundService
    {
        private readonly CacheService _cacheService;
        public DisplayMainDataBackgroundService(CacheService cacheService)
        {
            _cacheService = cacheService;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                Console.WriteLine(CacheService._heartBeat);
                string hexString = String.Join(" ", _cacheService.CtrlData.Select(b => $"0x{b:X2}"));
                Console.WriteLine(hexString);

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
                await Task.Delay(1000);
                Console.Clear();
            }
        }
    }
}
