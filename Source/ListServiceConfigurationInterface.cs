using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Harris.Automation.ADC.Services.Common.Configuration;

namespace Harris.Automation.ADC.Services.ListService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ListServiceConfigurationInterface : ConfigurationInterface
    {
        public ListServiceConfigurationInterface()
            : base(Config.Instance)
        {
            Config.Instance.Load(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) +
                ConfigurationManager.AppSettings["ConfigFilePath"]);
        }
    }
}
