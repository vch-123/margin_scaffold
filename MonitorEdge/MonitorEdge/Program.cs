using MonitorEdge.Service;
using MonitorEdge;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine("hello,Started");
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {

               Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug() // 设置全局最低日志级别为Debug
.WriteTo.Logger(lg => lg
.Filter.ByIncludingOnly(p => p.Level == Serilog.Events.LogEventLevel.Debug) // 只记录Debug级别的日志
.WriteTo.File("logs/ctrlANDmaindata__.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
.WriteTo.Logger(lg => lg
.Filter.ByIncludingOnly(p => p.Level == Serilog.Events.LogEventLevel.Information) // 只记录Information级别的日志
.WriteTo.File("logs/mqtt__.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
.WriteTo.Logger(lg => lg
.Filter.ByIncludingOnly(p => p.Level == Serilog.Events.LogEventLevel.Warning) // 只记录Warning级别的日志
.WriteTo.File("logs/heartbeat__.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
.WriteTo.Logger(lg => lg
.Filter.ByIncludingOnly(p => p.Level == Serilog.Events.LogEventLevel.Error) // 只记录Error级别的日志
.WriteTo.File("logs/error__.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
.WriteTo.Logger(lg => lg
.Filter.ByIncludingOnly(p => p.Level == Serilog.Events.LogEventLevel.Fatal) // 只记录Fatal级别的日志
.WriteTo.File("logs/plc__.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
.CreateLogger();


               // Build the configuration and add it to the services
               var environment = hostContext.HostingEnvironment;
               var builder = new ConfigurationBuilder()
                   .SetBasePath(environment.ContentRootPath)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

               var configuration = builder.Build();
               services.AddSingleton<ConfigService>();
               services.AddSingleton<CacheService>();
               services.AddSingleton<MqttClient>();
               
               services.AddHostedService<CraneSimuBackgroundService>();               
           });
    }
}