using EdgeSideProgramScaffold.Model;
using MiniExcelLibs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSideProgramScaffold.Service.FuncServices
{
    internal class ConfigService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private List<ConfigExcelModel> _models = new List<ConfigExcelModel>();
        public ConfigService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
            var Config_File_Path = "ConfigExcelModel.xlsx";
            var path = Config_File_Path;//从配置文件获取excel data            
            var models = MiniExcel.Query<ConfigExcelModel>(path);  //moeel  excel表里的每一行和 ConfigExcellModel对应
            _models = models.ToList();
            AutoConfig();
            PrintProperties(this);
        }

        //自动配置
        public void AutoConfig()
        {
            foreach (var model in _models)
            {
                if (model.DataType == "INT")
                {
                    SetIntMapValue(model.Variable, model.Value);
                }
                else if (model.DataType == "String")
                {
                    SetStringMapValue(model.Variable, model.Value);
                }
                else { Log.Information("ConfigExcelModel数据类型错误"); }
            }
        }

        private void PrintProperties(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            Console.WriteLine($"Properties of {type.Name}:");
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    var value = property.GetValue(obj);
                    Console.WriteLine($"\t{property.Name} = {value}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\tError accessing {property.Name}: {ex.Message}");
                }
            }
        }

        //设置对应的值
        public void SetIntMapValue(string variable, string excel_value)
        {
            int value = int.Parse(excel_value);
            // 获取当前类的类型信息
            Type type = this.GetType();

            // 尝试获取名为variable的属性
            PropertyInfo propertyInfo = type.GetProperty(variable);

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(int))
            {
                // 如果属性存在且类型为int，则设置其值
                propertyInfo.SetValue(this, value);
            }
            else
            {
                // 属性不存在或类型不匹配时的处理
                Console.WriteLine($"Property '{variable}' not found or not of type int.");
            }
        }

        public void SetStringMapValue(string variable, string excel_value)
        {
            // 获取当前类的类型信息
            Type type = this.GetType();

            // 尝试获取名为variable的属性
            PropertyInfo propertyInfo = type.GetProperty(variable);


            if (propertyInfo != null && propertyInfo.PropertyType == typeof(string))
            {
                if (propertyInfo.Name == "EqCode")
                {
                    propertyInfo.SetValue(this, excel_value);
                    this.MQTT_EQ_STATE_TOPIC = $"ICS/EQ_STATE/{EqCode}";
                    this.MQTT_CURRENTSTATE_TOPIC = $"ICS/BACK/{EqCode}";
                }
                // 如果属性存在且类型为int，则设置其值
                propertyInfo.SetValue(this, excel_value);
            }
            else
            {
                // 属性不存在或类型不匹配时的处理
                Console.WriteLine($"Property '{variable}' not found or not of type int.");
            }
        }

        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; }

        public string PLC_Variables_Mapping_File_Path => _configuration["PLC_Variables_Mapping_File_Path"];
        public string Config_File_Path => _configuration["Config_File_Path"];

        public int BackendToPLCoffsetY { get; set; }
        public string EqCode { get; set; }
        public int LoadTime { get; set; }
              

        public string PLC_IP { get; set; }
        public int PLC_Local_Port { get; set; }   
        public int PLC_State_Port { get; set; }


        public string MQTT_ID { get; set; }   //自己的mqtt id
        public string MQTT_IP { get; set; }
        public int MQTT_Port { get; set; }

        public string MQTT_EQ_STATE_TOPIC { get; set; }        
        public string MQTT_CURRENTSTATE_TOPIC { get; set; }
        public int PrintSuccess { get; set; }

        public string MQTT_BACKEND_MaterialGrids { get; set; } = "ICS/SpanAreaInfoToDCS";
        public string MQTT_BACKEND_DistributingMaterialCars { get; set; } = "ICS/CarInfoToDCS";
        public string MQTT_BACKEND_DistributingMaterialCarsCmd { get; set; } = "ICS/CarRequest";
    }
}
