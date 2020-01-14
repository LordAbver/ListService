using System;
using System.Collections.Generic;
using System.Linq;
using Harris.Automation.ADC.Types.Events;


namespace Harris.Automation.ADC.Services.ListService
{
    public enum EventStatus
    {
        Unknown,
        Done,
        Running,
        Cued,
        Error,
    }

    public class Extensions
    {
        public static EventStatus Map(IEnumerable<EventRunStatus> runStatus, EventEventIdPair @event)
        {
            if (runStatus == null || @event == null)
                throw new ArgumentNullException(runStatus == null ? "runStatus" : "event");
            
            if (runStatus.Any())
            {
                switch (runStatus.First())
                {
                    case EventRunStatus.Done:
                        //case EventRunStatus.NotPlayed:
                        //case EventRunStatus.Skipped:
                        return EventStatus.Done;

                    case EventRunStatus.PlayedNextVideo:
                    case EventRunStatus.Prepared:
                    case EventRunStatus.Prerolled:
                    case EventRunStatus.RollingNext:
                    case EventRunStatus.StandbyOn:  //<--
                        return EventStatus.Cued;

                    case EventRunStatus.Running:
                        return EventStatus.Running;

                    default:
                        return @event.ADCEvent.EmsIndex > 0 ? EventStatus.Unknown : EventStatus.Error;
                }
            }

            return @event.ADCEvent.EmsIndex <= 0 ? EventStatus.Error : EventStatus.Unknown;
        }
    }
}