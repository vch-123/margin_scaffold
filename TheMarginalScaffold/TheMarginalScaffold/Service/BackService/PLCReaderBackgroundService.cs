using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Client;
using TheMarginalScaffold.Service.FuncService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TheMarginalScaffold.Service.BackService
{
    public class PLCReaderBackgroundService:BackgroundService
    {
        private readonly UDPClient _udpClient;
        private readonly CacheService _cacheService;
        private readonly DataConverterService _dataConverterService;
        private readonly ConfigService _configService;
        public PLCReaderBackgroundService(UDPClient uDPClient, CacheService cacheService, DataConverterService dataConverterService, ConfigService configService)
        {
            _udpClient = uDPClient;
            _cacheService = cacheService;
            _dataConverterService = dataConverterService;
            _configService = configService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                byte[] data = {
                              0xCA,//引导符
                              0x23,//CtrlID
                              0x45,//Cmd指令ID
                              0x00,//Cmd指令执行状态
                              0x00,0x00,0x00,
                              0x00,0x00,0x00,0x00,//小车
                              0x00,0x00,0x00,0x00,//大车
                              0x00,0x00,0x00,0x00,//吊具
                              0x00,0x00,//小车电机转速
                              0x00,0x00,//大车电机转速
                              0x00,0x00,//起升电机转速
                              0x00,0x00,//开闭电机转速
                              0x00,0x00,0x00,0x00,//起重量
                              0x00,0x00,0x00,0x00,//抓斗开闭度
                              0x00,0x00,//故障
                              0x00,0x00,//小车变频器故障代码
                              0x00,0x00,//大车变频器故障代码
                              0x00,0x00,//起升变频器故障代码
                              0x00,0x00,//开闭变频器故障代码
                            };
                _dataConverterService.OperateExcelModel(data);
                await Task.Delay(200);
            }


            _udpClient.SetCallback((s, e) =>
            {
                try
                {
                    _dataConverterService.OperateExcelModel(s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex}");
                }                
            });
        }
    }
}
