using MonitorEdge.MQTTmessage;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEdge.Service
{
    internal class CacheService
    {
        public readonly List<MainData> _cranes;
        private const int WalkDuration = 6000; // 走行时间：6秒
        private const int GetDuration = 6000; // 取料时间：6秒
        private const int PutDuration = 6000; // 放料时间：6秒
        private const int UpdateInterval = 100; // 更新间隔：100ms
        private Timer _positionUpdateTimer; // 定时器，用于更新位置
        private Timer _cmdStateTimer; // 定时器，用于重置 CmdState

        public CacheService()
        {
            _cranes = new List<MainData>
            {
                new MainData { DeviceName = "ECrane01", X = 12000, Y = 200, Z = 13000, XS = 0, YS = 0, ZS = 0, Data = new(),Weight=8.5m },
                new MainData { DeviceName = "ECrane02", X = 12000, Y = 55000, Z = 13000, XS = 0, YS = 0, ZS = 0, Data = new(),Weight=8.5m },
                new MainData { DeviceName = "ECrane03", X = 12000, Y = 100000, Z = 13000, XS = 0, YS = 0, ZS = 0, Data = new(),Weight=8.5m },
                new MainData { DeviceName = "ECrane04", X = 12000, Y = 168000, Z = 13000, XS = 0, YS = 0, ZS = 0, Data = new(),Weight=8.5m }
            };

            _positionUpdateTimer = new Timer(UpdateCranePositionsAsync, null, 0, UpdateInterval);
            _cmdStateTimer = new Timer(UpdateCraneCmdStateAsync, null, 0, 3000); // 每3秒检查一次 CmdState
        }

        private void UpdateCraneCmdStateAsync(object? state)
        {
            foreach (var crane in _cranes)
            {
                if (!crane.IsWalking && !crane.IsGetting && !crane.IsPutting && crane.CmdState == 2)
                {
                    crane.CmdState = 0; // 重置 CmdState
                    Log.Information($"CmdState for {crane.DeviceName} reset to 0.");
                }
            }
        }

        // 处理走行命令
        public void HandleWalkCommand(string deviceName, string content)
        {
            var crane = _cranes.FirstOrDefault(c => c.DeviceName == deviceName);
            if (crane == null)
            {
                Log.Warning($"Crane {deviceName} not found.");
                return;
            }

            var walkObj = JsonConvert.DeserializeObject<WalkObj>(content);
            var targetX = walkObj.X;
            var targetY = walkObj.Y;
            var cmdID = walkObj.Id;

            // 计算速度
            var deltaX = targetX - crane.X;
            var deltaY = targetY - crane.Y;
            var steps = WalkDuration / UpdateInterval;

            crane.CommID = cmdID;
            crane.CMD = "walk";
            crane.XS = deltaX / steps;
            crane.YS = deltaY / steps;
            crane.TargetX = targetX;
            crane.TargetY = targetY;
            crane.IsWalking = true;
            crane.CmdState = 1;

            Log.Information($"Walk command for {crane.DeviceName}: Target (X: {targetX}, Y: {targetY}), Speed (XS: {crane.XS}, YS: {crane.YS})");
        }

        // 处理取料命令
        public void HandleGetCommand(string deviceName, string content)
        {
            var crane = _cranes.FirstOrDefault(c => c.DeviceName == deviceName);
            if (crane == null)
            {
                Log.Warning($"Crane {deviceName} not found.");
                return;
            }

            var getObj = JsonConvert.DeserializeObject<GetObj>(content);
            var targetZ = getObj.Z;
            var cmdID = getObj.Id;

            // 计算速度
            var deltaZ = targetZ - crane.Z;
            var steps = GetDuration / UpdateInterval;

            crane.CommID = cmdID;
            crane.CMD = "get";
            crane.ZS = deltaZ / steps;
            crane.TargetZ = targetZ;
            crane.IsGetting = true;
            crane.CmdState = 1;
            crane.GetPutState = GetPutState.MovingToTarget;

            Log.Information($"Get command for {crane.DeviceName}: Target Z: {targetZ}, Speed ZS: {crane.ZS}");
        }

        // 处理放料命令
        public void HandlePutCommand(string deviceName, string content)
        {
            var crane = _cranes.FirstOrDefault(c => c.DeviceName == deviceName);
            if (crane == null)
            {
                Log.Warning($"Crane {deviceName} not found.");
                return;
            }

            var putObj = JsonConvert.DeserializeObject<PutObj>(content);
            var targetZ = putObj.Z;
            var cmdID = putObj.Id;

            // 计算速度
            var deltaZ = targetZ - crane.Z;
            var steps = PutDuration / UpdateInterval;

            crane.CommID = cmdID;
            crane.CMD = "put";
            crane.ZS = deltaZ / steps;
            crane.TargetZ = targetZ;
            crane.IsPutting = true;
            crane.CmdState = 1;
            crane.GetPutState = GetPutState.MovingToTarget;

            Log.Information($"Put command for {crane.DeviceName}: Target Z: {targetZ}, Speed ZS: {crane.ZS}");
        }

        // 定时更新起重机位置
        private async void UpdateCranePositionsAsync(object state)
        {
            foreach (var crane in _cranes)
            {
                // 更新走行位置
                if (crane.IsWalking)
                {
                    crane.X += crane.XS;
                    crane.Y += crane.YS;

                    // 检查是否到达目标位置
                    if (Math.Abs(crane.X - crane.TargetX) <= Math.Abs(crane.XS) &&
                        Math.Abs(crane.Y - crane.TargetY) <= Math.Abs(crane.YS))
                    {
                        crane.X = crane.TargetX;
                        crane.Y = crane.TargetY;
                        crane.IsWalking = false;
                        crane.CmdState = 2;
                        crane.XS = 0;
                        crane.YS = 0;

                        Log.Information($"Crane {crane.DeviceName} reached target position (X: {crane.X}, Y: {crane.Y}).");
                    }
                }

                // 更新取料高度
                if (crane.IsGetting)
                {
                    crane.Z += crane.ZS;

                    // 检查是否到达目标高度
                    if ((Math.Abs(crane.Z - crane.TargetZ) <= Math.Abs(crane.ZS))&&crane.GetPutState==GetPutState.MovingToTarget)
                    {
                        crane.Z = crane.TargetZ;
                        crane.TargetZ = 13000;
                        var deltaZ = crane.TargetZ - crane.Z;
                        var steps = PutDuration / UpdateInterval;
                        crane.ZS = deltaZ / steps;
                        crane.GetPutState = GetPutState.ReturningToInitial;
                        crane.Weight = 18.52m;
                        Log.Information($"Crane {crane.DeviceName} reached target height (Z: {crane.Z}) for getting.");
                    }
                    if ((Math.Abs(crane.Z - crane.TargetZ) <= Math.Abs(crane.ZS)) && crane.GetPutState == GetPutState.ReturningToInitial)
                    {
                        crane.Z=crane.TargetZ;
                        crane.IsGetting = false;
                        crane.CmdState = 2;
                        crane.ZS = 0;
                        crane.GetPutState = GetPutState.Idle;

                        Log.Information($"Crane {crane.DeviceName} reached target height (Z: {crane.Z}) for getting.");
                    }

                }

                // 更新放料高度
                if (crane.IsPutting)
                {
                    crane.Z += crane.ZS;

                    // 检查是否到达目标高度
                    if ((Math.Abs(crane.Z - crane.TargetZ) <= Math.Abs(crane.ZS)) && crane.GetPutState == GetPutState.MovingToTarget)
                    {
                        crane.Z = crane.TargetZ;
                        crane.TargetZ = 13000;
                        var deltaZ = crane.TargetZ - crane.Z;
                        var steps = PutDuration / UpdateInterval;
                        crane.ZS = deltaZ / steps;
                        crane.GetPutState = GetPutState.ReturningToInitial;
                        crane.Weight = 8.5m;
                        Log.Information($"Crane {crane.DeviceName} reached target height (Z: {crane.Z}) for putting.");
                    }
                    if ((Math.Abs(crane.Z - crane.TargetZ) <= Math.Abs(crane.ZS)) && crane.GetPutState == GetPutState.ReturningToInitial)
                    {
                        crane.Z = crane.TargetZ;
                        crane.IsGetting = false;
                        crane.CmdState = 2;
                        crane.ZS = 0;
                        crane.GetPutState = GetPutState.Idle;

                        Log.Information($"Crane {crane.DeviceName} reached target height (Z: {crane.Z}) for putting.");
                    }

                }
            }
        }
    }
}