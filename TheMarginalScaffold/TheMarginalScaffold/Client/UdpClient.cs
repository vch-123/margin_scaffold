using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Service.FuncService;

namespace TheMarginalScaffold.Client
{
    public class UDPClient
    {
        private readonly ConfigService _configService;
        private UdpClient _udpClient;
        private Action<byte[], IPEndPoint?>? Callback;
        public UDPClient(ConfigService configService)
        {
            _configService = configService;
            Init();
        }

        public void Init()
        {
            _udpClient = new UdpClient(_configService.PLC_Local_Port);
            Log.Information($"udp 绑定端口:{_configService.PLC_Local_Port}");
            _udpClient.BeginReceive(ReciveCallback, null);
        }

        // SetCallback方法设置当接收到UDP数据时调用的回调函数。
        public void SetCallback(Action<byte[], IPEndPoint?> act)
        {
            Callback = act;
        }

        // ReciveCallback是接收到UDP数据时调用的回调方法。
        private void ReciveCallback(IAsyncResult ar)
        {
            // 创建一个IPEndPoint实例，用于存储发送方的网络端点信息。
            IPEndPoint? ipe = new IPEndPoint(IPAddress.Any, 0);
            //IPAddress.Any 表示接收任何IP地址发送的数据，端口号 0 表示使用动态分配的端口。
            try
            {
                // 从UdpClient接收数据，并更新ipe为发送方的终点。
                var data = _udpClient?.EndReceive(ar, ref ipe);
                // 如果接收到数据且设置了回调函数，则调用回调函数。
                if (data != null && Callback != null)
                {
                    // 调用回调函数，传递接收到的数据和发送方的终点。
                    Callback?.Invoke(data, ipe);
                }
            }
            catch (ObjectDisposedException)
            {
                // 如果捕获到UDP客户端已关闭的异常，记录警告日志。
                Log.Error("UDP client已关闭，无法接收数据。");
            }
            catch (SocketException ex)
            {
                // 如果捕获到Socket异常，记录错误日志。
                Log.Error($"Socket exception: {ex.Message}");
            }
            // 继续异步接收数据。
            _udpClient?.BeginReceive(ReciveCallback, null);
        }

        /// <summary>
        /// 向起重机推送实时状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>       
        public async Task SendCtrlData(byte[] data)
        {
            // 解析PLC的IP地址。
            IPAddress ipAddress = IPAddress.Parse(_configService.PLC_IP);
            // 获取PLC控制端口。
            int port = _configService.PLC_Ctrl_Port;
            // 创建IPEndPoint实例，表示目标网络端点。
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            // 异步发送数据到指定的网络端点。
            _udpClient?.SendAsync(data, data.Length, endPoint);
            // 记录发送数据的日志。
            Log.Debug($"udp 向{_configService.PLC_IP}:{_configService.PLC_Ctrl_Port}发送:{BitConverter.ToString(data)}");
        }

        // SendCmdData方法用于向PLC发送命令数据。
        public async Task SendCmdData(byte[] data)
        {
            // 解析PLC的IP地址。
            IPAddress ipAddress = IPAddress.Parse(_configService.PLC_IP);
            // 获取PLC命令端口。
            int port = _configService.PLC_Cmd_Port;
            // 创建IPEndPoint实例，表示目标网络端点。
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            // 异步发送数据到指定的网络端点。
            _udpClient?.SendAsync(data, data.Length, endPoint);
            // 记录发送数据的日志。
            Log.Information($"udp 向{_configService.PLC_IP}:{_configService.PLC_Cmd_Port}发送:{BitConverter.ToString(data)}");
        }
    }
}

