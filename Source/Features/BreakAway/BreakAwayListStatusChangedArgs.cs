using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;

namespace Harris.Automation.ADC.Services.ListService.Source.Features.BreakAway
{
    class BreakAwayListStatusChangedArgs: EventArgs
    {
        public BreakAwayListStatusChangedArgs(String server, Int32 list, BreakAwayListStatusDTO breakAwayListStatus)
        {
            Server = server;
            List = list;
            BreakAwayListStatus = breakAwayListStatus;
        }

        public String Server { get; private set; }
        public Int32 List { get; private set; }
        public BreakAwayListStatusDTO BreakAwayListStatus { get; private set; }
    }
}
