using MiniExcelLibs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheMarginalScaffold.Client;
using TheMarginalScaffold.Model;
using TheMarginalScaffold.Model.CraneState;

namespace TheMarginalScaffold.Service.FuncService
{
    public class DataConverterService
    {
        private readonly ConfigService _configService;
        private readonly CacheService _cacheService;
        public DataConverterService(ConfigService configService, CacheService cacheService)
        {
            _configService = configService;
            _cacheService = cacheService;
            _cacheService.ExcelListModel = GetInstance();
        }

        private List<PLCVariableExcelModel> GetInstance()
        {
            var path = _configService.PLC_Variables_Mapping_File_Path;//从配置文件获取excel data            
            var models = MiniExcel.Query<PLCVariableExcelModel>(path);  //moeel  excel表里的每一行和 ImportExcelModel对应
            var dic = models.ToList();
            return dic;
        }

        public static string GetTime()
        {
            DateTime now = DateTime.Now;
            // 获取当前时区偏移
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(now);
            // 格式化日期时间字符串
            return now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff") +
                (offset >= TimeSpan.Zero ? "+" : "-") +
                offset.ToString(@"hh\:mm");
        }

        private void LogBytes(byte[] bytes)
        {
            var logContent = new System.Text.StringBuilder();
            logContent.AppendLine(); // 在最开始添加一个换行符

            for (int i = 0; i < bytes.Length; i++)
            {
                logContent.AppendFormat("0x{0:X2}", bytes[i]); // 格式化为0x后跟两位十六进制数
                if (i < bytes.Length - 1) // 如果不是最后一个字节，则添加逗号
                {
                    logContent.Append(", ");
                }
                if ((i + 1) % 20 == 0) // 每20个字节后换行
                {
                    logContent.AppendLine();
                }
            }

            // 如果数组的最后一个字节不是20的倍数，添加一个换行符
            if (bytes.Length % 20 != 0 && bytes.Length > 0) // 确保数组不为空
            {
                logContent.AppendLine();
            }

            // 输出整个字节数组到一个日志条目
            Log.Fatal("收到PLC发送字节数: {0}  字节为: {1}", bytes.Length, logContent.ToString().TrimEnd());
        }

        public bool CheckByte(byte a, int index)
        {
            // 创建一个只在第index位有1的掩码
            byte mask = (byte)(1 << index);

            // 使用按位与操作检查a的第index位是否为1
            return (a & mask) != 0;
        }

        public async void OperateExcelModel(byte[] bytes)
        {
            //Console.WriteLine("23333333333333333333333");
            LogBytes(bytes);

            TransferMainData mainData = new TransferMainData() { Data = new Dictionary<string, object>() };
            mainData.Time = GetTime();        

            mainData.CMD = MqttClient.Cmd;
            mainData.CommID = bytes[2];
            mainData.CmdState = bytes[3];
            mainData.DeviceName = _configService.EqCode;
            
            mainData.PlcOnline = 1;
            string[] alarmMessages = { "AlarmMessage", "AlarmMessage2" };


            foreach (var model in _cacheService.ExcelListModel)
            {
                string UpStr = model.DataType.ToUpperInvariant();
                if (model.MainParaName == null)
                {
                    // 根据边缘测量地址和数据类型，从bytes中提取值，并尝试添加到mainData.data字典中
                    var returnValue = ReturnByType(model.EdgeMeasurementAddress, UpStr, bytes);
                    TryAdd(model.DataName, returnValue, mainData.Data);
                }

                else  // 如果主要字段名不为null，表示这个字段是MainData类中的一个属性
                {
                    PropertyInfo[] propertyInfo = typeof(TransferMainData).GetProperties();
                    // 获取MainData类的所有公共属性。
                    // 遍历所有属性，寻找名称与模型中的主要字段名匹配的属性。
                    //比如当前model是automode，那就让maindata里的属性滚一圈，碰到了就赋值
                    foreach (var item in propertyInfo)   //propertyInfo
                    {
                        if (item.Name == model.MainParaName)
                        {

                            if (alarmMessages.Contains(item.Name))
                            {

                                var returnValue = ReturnByType(model.EdgeMeasurementAddress, UpStr, bytes);

                                if (returnValue.GetType() == typeof(bool))
                                {

                                    //okokok
                                    // 如果是布尔类型，进行相应的操作
                                    int x = (bool)returnValue ? 1 : 0;


                                    if (x == 1 && item.Name == "AlarmMessage")
                                    {
                                        mainData.AlarmMessage += model.DataName + "||"; continue;
                                    }

                                    else if (x == 0 && item.Name == "AlarmMessage2")
                                    {
                                        if (model.EdgeMeasurementAddress == "182.4")
                                        {
                                            mainData.AlarmMessage += model.DataName + "||"; continue;
                                        }

                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                            }

                            else if (model.MainParaName == "ErrorCode")
                            {
                                var returnValue = ReturnByType(model.EdgeMeasurementAddress, UpStr, bytes);
                                byte firstByte;
                                byte secondByte;
                                string combinedHexString;

                                byte[] byteArray = returnValue as byte[];
                                if (byteArray != null)
                                {
                                    firstByte = byteArray[1];
                                    secondByte = byteArray[0];

                                    string firstByteHex = firstByte.ToString("X2");
                                    string secondByteHex = secondByte.ToString("X2");

                                    if (firstByteHex != "00" && secondByteHex != "00")
                                    {
                                        switch (model.EdgeMeasurementAddress)
                                        {
                                            case "76.0":  mainData.ErrorCode += "起升变频器故障代码:"; break;
                                            case "110.0": mainData.ErrorCode += "小车变频器故障代码:"; break;
                                            case "140.0": mainData.ErrorCode += "大车变频器故障代码:"; break;
                                            case "156.0": mainData.ErrorCode += "开闭变频器故障代码:"; break;
                                        }

                                        // 合并两个十六进制字符串
                                        combinedHexString = secondByteHex + firstByteHex; //原料库  字节顺序和尾矿库相反， 字节顺序反过来就可以
                                        mainData.ErrorCode += combinedHexString + "||";
                                    }
                                }

                                continue;

                            }

                            else if (UpStr == "REAL")
                            {
                                var returnValue = ReturnByType(model.EdgeMeasurementAddress, UpStr, bytes);
                                if (returnValue != null)
                                {
                                    item.SetValue(mainData, Convert.ToDecimal(returnValue));
                                }
                                else
                                {
                                    Log.Error(model.EdgeMeasurementAddress + "REAL Return Value is null, unable to convert to decimal.");
                                }
                                continue;
                            }

                            else
                            {
                                // 对于其他属性，直接设置从bytes中提取的值。
                                var returnValue = ReturnByType(model.EdgeMeasurementAddress, UpStr, bytes);

                                if (returnValue.GetType() == typeof(bool))
                                {

                                    //okokok
                                    // 如果是布尔类型，进行相应的操作
                                    int x = (bool)returnValue ? 1 : 0;

                                    item.SetValue(mainData, x);
                                    continue;
                                }

                                item.SetValue(mainData, returnValue);
                            }                           
                        }
                    }
                }
            }       

            if (CheckByte(_cacheService.CtrlData[2], 4) == true)
            {
                mainData.SoftEMStop = 1;
            }
            else
            {
                mainData.SoftEMStop = 0;
            }          
            mainData.AlarmMessage += mainData.ErrorCode;
             
            if (mainData.AlarmMessage != "")
            {
                Log.Error($"故障：{mainData.AlarmMessage}");
            }

            _cacheService.UpdateMainData(mainData);
        }


        public static string GetMainDataInfoString(TransferMainData mainData)
        {
            if (mainData == null)
            {
                throw new ArgumentNullException(nameof(mainData));
            }

            Type type = mainData.GetType();
            StringBuilder info = new StringBuilder();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(mainData);
                info.Append(property.Name).Append(": ").Append(value).Append(", ");
            }

            if (info.Length > 0)
            {
                info.Length -= 2; // Remove the last comma and space
            }

            return info.ToString();
        }

        /// <summary>
        /// 将传入的地址切割前一位是字节位，后一位是比特位
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private (int, int) CutAddresss(string address)
        {
            int byteLocation = Convert.ToInt32(address.Split('.')[0]);
            int bitLocation = Convert.ToInt32(address.Split('.')[1]);
            return (byteLocation, bitLocation);
        }


        /// <summary>
        /// 获取一个byte的中某一位的值
        /// </summary>
        /// <param name="value">byte值</param>
        /// <param name="address">位数</param>
        /// <returns></returns>
        private bool FindBitInByte(byte value, int address)
        {
            // 确保address在0到7之间，因为byte有8位
            if (address < 0 || address > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Address must be between 0 and 7.");
            }

            // 使用1左移address位，然后与value进行AND操作
            // 如果result不为0，说明在指定的address位置是1
            bool bitIsSet = (value & (1 << address)) != 0;
            return bitIsSet;
        }

        /// <summary>
        /// 返回类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        private object? ReturnByType(string address, string type, byte[] bytes)
        {
            if (type == PLCVariableType.BOOL)
            {
                var result = CutAddresss(address);
                var value = FindBitInByte(bytes[result.Item1], result.Item2);
                return value;
            }
            else if (type == PLCVariableType.INT)
            {
                //int addr = Convert.ToInt32(address);
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte[] shortArray = new byte[2];
                Buffer.BlockCopy(bytes, intValue, shortArray, 0, 2);
                short modelValue = BitConverter.ToInt16(shortArray, 0);
                return modelValue;
            }
            else if (type == PLCVariableType.DINT)
            {

                //int addr = Convert.ToInt32(address);
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte[] shortArray = new byte[4];
                Buffer.BlockCopy(bytes, intValue, shortArray, 0, 4);
                byte[] bytes1 = new byte[4];

                bytes1[0] = shortArray[2];
                bytes1[1] = shortArray[3];
                bytes1[2] = shortArray[0];
                bytes1[3] = shortArray[1];

                int value = bytes1[3];
                value = value + bytes1[2] * 256;
                value = value + bytes1[1] * 256 * 256;
                value = value + bytes1[0] * 256 * 256 * 256;
                //int modelValue = BitConverter.ToInt32(bytes1, 0);
                return value;
            }
            else if (type == PLCVariableType.REAL)
            {

                //int addr = Convert.ToInt32(address);
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte[] shortArray = new byte[4];
                Buffer.BlockCopy(bytes, intValue, shortArray, 0, 4);

                byte[] bytes1 = new byte[4];

                bytes1[0] = shortArray[2];
                bytes1[1] = shortArray[3];
                bytes1[2] = shortArray[0];
                bytes1[3] = shortArray[1];

                float modelValue = BitConverter.ToSingle(ReverseByteArray(bytes1), 0);

                return modelValue;



            }
            else if (type == PLCVariableType.DWORD)
            {
                //int addr = Convert.ToInt32(address);
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte[] shortArray = new byte[4];
                Buffer.BlockCopy(bytes, intValue, shortArray, 0, 4);
                uint modelValue = BitConverter.ToUInt32(shortArray, 0);
                return modelValue;
            }
            else if (type == PLCVariableType.BYTE)
            {
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte modelValue = bytes[intValue];
                return modelValue;
            }
            else if (type == PLCVariableType.WORD) // 处理WORD类型
            {

                if (address == "76.0")
                {
                    Console.WriteLine();
                }

                // 将address转换为整数，表示从bytes数组中的起始位置
                double number = double.Parse(address);
                // 然后将 double 转换为 int
                int intValue = (int)number;
                byte[] shortArray = new byte[2];
                Buffer.BlockCopy(bytes, intValue, shortArray, 0, 2);


                return shortArray;
                //ushort value = BitConverter.ToUInt16(shortArray, 0);
                //return value;
            }
            else
            {
                return null;
            }

        }

        private byte[] ReverseByteArray(byte[] array)
        {
            byte[] reversedArray = new byte[array.Length];
            int j = 0;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                reversedArray[j++] = array[i];
            }
            return reversedArray;
        }


        /// <summary>
        /// 尝试插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="valuePairs"></param>
        private void TryAdd(string key, object? value, Dictionary<string, object> valuePairs)
        {
            if (key == null)
                return;
            if (!valuePairs.ContainsKey(key))
            {
                valuePairs.Add(key, value);
            }
            else
            {
                valuePairs[key] = value;
            }
        }
    }
}
