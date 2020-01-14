using Harris.Automation.ADC.Services.Common.Configuration;


namespace Harris.Automation.ADC.Services.ListService
{
    public sealed class Config : SingletonConfiguration<ListServiceConfiguration>
    {
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                }
                return _instance;
                }
            }

        private Config()
            : base()
            {

        }
    }
}
