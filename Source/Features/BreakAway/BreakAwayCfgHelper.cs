using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Harris.Automation.ADC.Logger;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;

namespace Harris.Automation.ADC.Services.ListService.Source.Features.BreakAway
{
    class BreakAwayCfgHelper
    {
        #region BreakAway Feature
        private String _breakAwayCfgFileName;

        public BreakAwayCfgHelper()
        {
            _breakAwayCfgFileName = String.Empty;
            InitBreakAwayConfig();
        }

        #region Public methods
        public BreakAwayConfigurationDTO GetConfiguration()
        {
            var breakAwayConfig = new BreakAwayConfigurationDTO();
            Type[] personTypes = { typeof(BreakAwayGroupDTO), typeof(ChannelDescription) };
            var serializer = new XmlSerializer(typeof(BreakAwayConfigurationDTO), personTypes);
            try
            {
                using (var fs = new FileStream(_breakAwayCfgFileName, FileMode.Open))
                {
                    breakAwayConfig = (BreakAwayConfigurationDTO)serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.ErrorFormat("Unable to get the Break Away configuration from file {0},reason:{1}", _breakAwayCfgFileName, ex);
            }

            return breakAwayConfig;
        }

        public Boolean SetConfiguration(BreakAwayConfigurationDTO breakAwayCfg)
        {
            Boolean res = true;
            try
            {
                Type[] personTypes = { typeof(BreakAwayGroupDTO), typeof(ChannelDescription) };
                var serializer = new XmlSerializer(typeof(BreakAwayConfigurationDTO), personTypes);
                var fs = new FileStream(_breakAwayCfgFileName, FileMode.Create);
                serializer.Serialize(fs, breakAwayCfg);
                fs.Close();
            }
            catch (Exception ex)
            {
                ServiceLogger.ErrorFormat("Unable to write the Break Away configuration to file {0},reason:{1}", _breakAwayCfgFileName, ex);
                res = false;
            }
            return res;
        }
        #endregion

        #region Private methods
        private void InitBreakAwayConfig()
        {
            const String defaultFilePath = ".\\config\\BreakAwayConfig.xml";
            String filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            String directory = Path.GetDirectoryName(filePath);
            String path = ConfigurationManager.AppSettings["BreakAwayConfigFilePath"];
            if (string.IsNullOrEmpty(path))
            {
                path = defaultFilePath;
            }
            if (directory != null)
                _breakAwayCfgFileName = Path.Combine(directory, path);
            if (!File.Exists(_breakAwayCfgFileName))
            {
                ServiceLogger.InformationalFormat("BreakAway configuration {0} was not found. ", _breakAwayCfgFileName);
                SetDefaultBreakAwayConfig();
            }
        }

        private void SetDefaultBreakAwayConfig()
        {
            var res = new BreakAwayConfigurationDTO();
            SetConfiguration(res.ToDefault());
        }

        #endregion

        public void Dispose()
        {
            _breakAwayCfgFileName = null;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
