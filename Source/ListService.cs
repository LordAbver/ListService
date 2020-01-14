using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;
using Harris.Automation.ADC.Services.Common.ParameterInspection;
using Harris.Automation.ADC.Services.Common.DataTransferObjects;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Events;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Timecode;
using Harris.Automation.ADC.Services.Common.Source;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;
using Harris.Automation.ADC.Services.ListService.Source.Features.BreakAway;
using Harris.Automation.ADC.Types;
using Harris.Automation.ADC.Types.Events;
using Harris.Automation.ADC.Types.ServiceErrors;
using Harris.Automation.ADC.Services.Common;
using Harris.Automation.ADC.Logger;
using Timer = System.Timers.Timer;

namespace Harris.Automation.ADC.Services.ListService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ErrorBehavior(typeof(ServicesErrorHandler))]
    public class ListService : BusinessService, IListService
    {
        #region Private fields

        private DeviceServerManager _adcSrv;
        private List<AsyncListServiceClient> _subscribers = new List<AsyncListServiceClient>();
        private BreakAwayManager _breakAwayManager;
        private Object _subscribersLock = new Object();
        private readonly Func<IListServiceClient> _callbackCreator;

#warning Replace Timer with Thread!
        private Timer _clientsMonitor;

        private DataServiceManager _dataServiceManager;

        private Boolean _isStopped;

        #endregion

        #region Public Properties


        #endregion

        #region Construction

        public ListService(DeviceServerManager adcConnect)
        {
            Config.Instance.SubscribeToAllConfigurationChanges(ConfigPropertyChanged);
            InitializeListService(adcConnect);
            _callbackCreator = () => OperationContext.Current.GetCallbackChannel<IListServiceClient>();
            _dataServiceManager = new DataServiceManager();
            _dataServiceManager.DataBaseFieldsRetrieved += OnDatabaseFieldsRetrieve;
            _breakAwayManager=new BreakAwayManager(this);
            _breakAwayManager.OnBreakAwayListStatusChanged += OnBreakAwayStatusChanged;
        }

#if (DEBUG)
        public ListService(DeviceServerManager adcConnect, Func<IListServiceClient> callbackCreator)
        {
            if (callbackCreator == null)
            {
                throw new ArgumentNullException("callbackCreator");
            }
            Config.Instance.SubscribeToAllConfigurationChanges(ConfigPropertyChanged);
            InitializeListService(adcConnect);
            _callbackCreator = callbackCreator;
        }
#endif

        public ListService()
        {
            //Configuration cfg = ConfigurationManager.OpenExeConfiguration((new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath);
            Config.Instance.Load(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) +
                ConfigurationManager.AppSettings["ConfigFilePath"]);            // Subscribe to config file changes made by config object
            Config.Instance.SubscribeToAllConfigurationChanges(ConfigPropertyChanged);

            var appName = Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName;
            if (String.IsNullOrEmpty(appName) || String.IsNullOrWhiteSpace(appName))
            {
                Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName = "LstSrvc";
                Config.Instance.Save();
            }
            // Create Device Server manager
            var dsManager = new DeviceServerManager();
            // Congure the Device Servers
            dsManager.ServersCreate(Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToInitialize);

            // Connect to the specified Device Servers
            foreach (var dsName in Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToConnect)
            {
                dsManager.ServerStart(dsName);
            }

            InitializeListService(dsManager);
            _callbackCreator = () => OperationContext.Current.GetCallbackChannel<IListServiceClient>();
        }

        public void Stop()
        {
            if (!_isStopped)
            {
                _adcSrv.Dispose();
                _clientsMonitor.Dispose();
                _dataServiceManager.DataBaseFieldsRetrieved -= OnDatabaseFieldsRetrieve;
                _breakAwayManager.OnBreakAwayListStatusChanged -= OnBreakAwayStatusChanged;
                _dataServiceManager.Dispose();
                _breakAwayManager.Dispose();
                lock (_subscribersLock)
                {
                    foreach (var asyncListServiceClient in _subscribers)
                    {
                        asyncListServiceClient.Dispose();
                    }
                    _subscribers.Clear(); 
                }
                _isStopped = true;
            }
        }

        #endregion

        #region Private methods

        private DeviceServerManager GetDeviceServerManager()
        {
            var appName = Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName;
            if (String.IsNullOrEmpty(appName) || String.IsNullOrWhiteSpace(appName))
            {
                Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName = "ListSrvc";
                Config.Instance.Save();
            }
            // Create Device Server manager
            var dsManager = new DeviceServerManager();
            // Congure the Device Servers
            dsManager.ServersCreate(Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToInitialize);

            // Connect to the specified Device Servers
            foreach (String dsName in Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToConnect)
            {
                dsManager.ServerStart(dsName);
            }
            return dsManager;
        }

        /// <summary>
        /// Set Device Server Manager to use with this service,
        /// set up event handlers and fault converters
        /// </summary>
        /// <param name="adcConnect">Device Server Manager</param>
        private void InitializeListService(DeviceServerManager adcConnect)
        {
            _isStopped = false;
            _adcSrv = adcConnect;
            _adcSrv.ListChanged += OnListChanged;
            _adcSrv.EventsAdded += OnEventsAdded;
            _adcSrv.EventsDeleted += OnEventsDeleted;
            _adcSrv.EventsUpdated += OnEventsUpdated;
            _adcSrv.EventsMoved += OnEventsMoved;
            _adcSrv.ServerStatusChanged += OnServerStatusChanged;
            ExceptionToFaultConverter.RegisterConverter<ServerNotExistException, DeviceServerNotCreatedError>(ex => new DeviceServerNotCreatedError(ex.Source, ex.Message));
            ExceptionToFaultConverter.RegisterConverter<ServerNotRunningException, DeviceServerNotRunningError>(ex => new DeviceServerNotRunningError(ex.Source, ex.Message));
            ExceptionToFaultConverter.RegisterConverter<ServiceParametersValidationException, ServiceParametersValidationError>(ex => new ServiceParametersValidationError(ex.Source, ex.Message));
            ExceptionToFaultConverter.RegisterConverter<ListServiceListLockedException, ListLockedError>(ex => new ListLockedError(ex.Source, ex.Message));
            ExceptionToFaultConverter.RegisterConverter<ListServiceListNotEnabledException, ListServiceError>(ex => new ListServiceError(ex.Source, ex.Message));
            ExceptionToFaultConverter.RegisterConverter<ServiceException, ListServiceError>(ex => new ListServiceError(ex.Source, ex.Message));
            _clientsMonitor = new Timer(60000) { AutoReset = false };
            _clientsMonitor.Elapsed += CheckClients;
            _clientsMonitor.Enabled = true;
        }

        private void CheckClients(object state, ElapsedEventArgs elapsedEventArgs)
        {
           Task.Factory.StartNew(RemoveDisconnectedClients); 
        }

        private void RemoveDisconnectedClients()
        {
            lock (_subscribersLock)
            {
                foreach (var client in _subscribers)
                {
                    client.CheckAvailability();
                }
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _clientsMonitor.Enabled = true;
            }
        }

        private void ConfigPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Some setting is changed, need to configure threads
            if (e.PropertyName == @"ConfigXML")
            {
                if (_adcSrv.AdcAppName == Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName)
                {
                    _adcSrv.ServersUpdate(Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToInitialize);
                    _adcSrv.SetActiveServers(Config.Instance.ConfigObject.ADCConnectionOptions.DeviceServersToConnect);
                }
                else
                {
                    _adcSrv.Dispose();
                    InitializeListService(GetDeviceServerManager());
                }
            }
        }

        private Boolean IsListLocked(Channel list)
        {
            var proxy = _callbackCreator();
            lock (_subscribersLock)
            {
                return _subscribers.Exists(client => client.IsListLockedBy(list) && client.IsCurrentCallback(proxy));
            }
        }

        private void CheckListLock(String server, Int32 list)
        {
            if (!IsListLocked(new Channel(server, list)))
            {
                throw new ListServiceListLockedException(String.Format(@"You need to lock the list #{0} before editing! (server '{1}')", list, server));
            }
        }

        private void CheckParametersForInsertMethod(InsertPlaceType place, Guid position)
        {
            if (place == InsertPlaceType.AfterGuid && position == Guid.Empty)
            {
                throw new ServiceParametersValidationException(@"Guid can't be Empty. Check the parameter 'Guid position'");
            }
        }

        private void CheckParametersForMoveMethod(List<EventEventIdPair> list, Int32 lookAhead, List<Guid> events, Guid destination)
        {
            if (events.Any(item => item == Guid.Empty))
            {
                throw new ServiceParametersValidationException(@"Guid can't be Empty. Check the Guid's of source events");
            }

            var listOfIndex = list.FindAll(ev => events.Contains(ev.ADCEventId)).Select(ev => list.IndexOf(ev));

            if (listOfIndex.Any(item => item >= lookAhead))
            {
                throw new ServiceParametersValidationException(@"One of the source events is out of look ahead");
            }

            var destinationEvent = list.Find(ev => ev.ADCEventId == destination);

            if (destinationEvent != null)
            {
                if (list.IndexOf(destinationEvent) >= lookAhead)
                {
                    throw new ServiceParametersValidationException(
                        @"Wrong destination Guid. Destination event is out of look ahead");
                }
            }
            else if(destination != Guid.Empty)
            {
                throw new ServiceParametersValidationException(
                    @"Wrong destination Guid. Destination event was not found");
            }
        }

        private void ClearDoneEventsBeforeInsert(IEnumerable<EventEventIdPair> events)
        {
            foreach (var ev in events)
            {
                ev.Undone();
            }
        }

        private void OnDatabaseFieldsRetrieve(object sender, EventsDBFieldsEventArgs e)
        {
            _adcSrv.SetDatabaseFields(e.ServerName, e.ListNumber, e.Events);
        }

        private void ClearDoneEventsAfterMove(LoginSession session, String server, Int32 list, IEnumerable<EventEventIdPair> events)
        {
            var clearDoneEvent = events.Where(item => item.ADCEvent.EventStatus.Contains(EventRunStatus.Done)).Select(item => item.ADCEventId).ToList();
            PerformEventsClearDone(session, server, list, clearDoneEvent);
        }

        #endregion

        #region Straight methods

        public Int32 GetEventsCount(LoginSession session, String server, Int32 list)
        {
            return _adcSrv.GetEventsCount(server, list);
        }

        public IEnumerable<EventDTO> GetList(LoginSession session, String server, int list)
        {
            return _adcSrv.GetEventsList(server, list).Select(pair => pair.ToDTO()).ToList();
        }

        public EventDTO GetFirstHardStartEvent(LoginSession session, String server, int list)
        {
            var tmpList = _adcSrv.GetEventsList(server, list).ToList();
            var result = tmpList.FirstOrDefault(pair => pair.IsHardStart).ToDTO();
            return result;
        }

        public EventDTO GetFirstMissingMediaEvent(LoginSession session, String server, int list)
        {
            var tmpList = _adcSrv.GetEventsList(server, list).ToList();
            var result = tmpList.FirstOrDefault(pair => pair.IsMissing).ToDTO();
            return result;
        }

        public EventDTO GetOnAirEvent(LoginSession session, String server, Int32 list)
        {
            var tmpList = _adcSrv.GetEventsList(server, list);
            return tmpList.Where(pair => pair.ADCEvent.EventStatus.Contains(EventRunStatus.Running)).FirstOrDefault()
                        .ToDTO();
        }

        public string GetListName(LoginSession session, String server, Int32 list)
        {
            string result = _adcSrv.GetListName(server, list);
            return result;
        }

        public TypeOfList GetListType(LoginSession session, String server, Int32 list)
        {
            var result = _adcSrv.GetListType(server, list);
            return result;
        }

        public string GetExtListName(LoginSession session, String server, Int32 list)
        {
            string result = _adcSrv.GetExtListName(server, list);
            return result;
        }

        public void SetExtListName(LoginSession session, String server, Int32 list, String filename)
        {
            if (filename == null)
                throw new ServiceException("Parameter 'filename' for 'SetExtListName' method can not be null");
            _adcSrv.SetExtListName(server, list, filename);
        }


        public int GetLookahead(LoginSession session, String server, Int32 list)
        {
            int result = _adcSrv.GetLookAhead(server, list);
            return result;
        }

        public IEnumerable<ListStateValue> GetListState(LoginSession session, String server, Int32 list)
        {
            IEnumerable<ListStateValue> result = _adcSrv.GetListState(server, list);
            return result;
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public EventProblemInformationDTO GetGapProblemInformation(LoginSession session, String server, int list)
        {
            var tmpList = _adcSrv.GetEventsList(server, list).ToList();
            Boolean isGap = false;
            Boolean isMissing = false;

            // hardstart
            var gapProblemEvent = new EventProblemInformationDTO();
            var index = tmpList.FindIndex(pair => pair.IsHardStart && !pair.ADCEvent.EventStatus.Contains(EventRunStatus.Done));
            if (index > 0)
            {
                var hardStartEvent = tmpList[index].ToDTO();// _dataServiceManager.AddDataBaseFields(tmpList[index].ToDTO());
                index--;
            for (; index > -1; index--)
            {
                // this is last event before hardstart 
                if (tmpList[index].IsPrimary)
                    break;
            }

            // primary event index
            if (index > 0)
            {
                var endEvent = tmpList[index].ToDTO();// _dataServiceManager.AddDataBaseFields(tmpList[index].ToDTO());               
                var startTimeGap = (endEvent.OnAirTime.ToTimeCode() + endEvent.Duration.ToTimeCode());
                var gapTimeInterval = hardStartEvent.OnAirTime.ToTimeCode() - startTimeGap;

                    gapProblemEvent.Duration = new TimeCodeDTO(gapTimeInterval);
                    gapProblemEvent.InsertId = endEvent.AdcEventId;
                    gapProblemEvent.StartTime = new TimeCodeDTO(startTimeGap);
                    gapProblemEvent.DeleteId = Guid.Empty;
                    gapProblemEvent.IdDeletedEvent = hardStartEvent.ID;
                    isGap = true;
                }
            }

            //missing media
            var missingProblemEvent = new EventProblemInformationDTO();
            index = tmpList.FindIndex(pair => pair.IsMissing);
            if (index > 0)
            {
                var missingMedia = tmpList[index].ToDTO();// _dataServiceManager.AddDataBaseFields(tmpList[index].ToDTO());
                index--;
                for (; index > -1; index--)
                {
                    // this is last event before hardstart 
                    if (tmpList[index].IsPrimary)
                        break;
                }

                // primary event index
                if (index > 0)
                {
                    var endEvent = tmpList[index].ToDTO();// _dataServiceManager.AddDataBaseFields(tmpList[index].ToDTO());

                    var startTimeMissing = (endEvent.OnAirTime.ToTimeCode() + endEvent.Duration.ToTimeCode());

                    missingProblemEvent.Duration = missingMedia.Duration;
                    missingProblemEvent.DeleteId = missingMedia.AdcEventId;
                    missingProblemEvent.InsertId = endEvent.AdcEventId;
                    missingProblemEvent.StartTime = new TimeCodeDTO(startTimeMissing);
                    missingProblemEvent.IdDeletedEvent = missingMedia.ID;

                    if (isGap)
                    {
                        if (missingProblemEvent.StartTime.ToTimeCode() > gapProblemEvent.StartTime.ToTimeCode())
                            return gapProblemEvent;

                        return missingProblemEvent;
                    }

                    isMissing = true;
                }
            }

            if (isGap)
                    return gapProblemEvent;

            if (isMissing)
                return missingProblemEvent;

            return null;

        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public EventDTO InsertEvent(LoginSession session, String server, Int32 list, EventDTO ev, InsertPlaceType place, Guid position)
        {
            if (ev == null)
                return null;
            CheckListLock(server, list);
            CheckParametersForInsertMethod(place, position);
            var tmpPair = ev.ToPair();
            return _adcSrv.InsertEvent(server, list, tmpPair, place, position).ToDTO();
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public IEnumerable<EventDTO> InsertEventList(LoginSession session, String server, Int32 list, IEnumerable<EventDTO> events, InsertPlaceType place, Guid position)
        {
            if (events == null || !events.Any())
                return new List<EventDTO>();

            CheckListLock(server, list);
            CheckParametersForInsertMethod(place, position);
            var tmpList = events.Select(pair => pair.ToPair()).ToList();
            return _adcSrv.InsertEventList(server, list, tmpList, place, position).Select(pair => pair.ToDTO());
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public Boolean ModifyEvent(LoginSession session, String server, Int32 list, EventDTO ev, Guid position)
        {
            if (ev == null)
                return false;
            CheckListLock(server, list);
            var tmpPair = ev.ToPair();
            _dataServiceManager.CreateTaskOnModifyEvent(server, list, tmpPair);
            return _adcSrv.ModifyEvent(server, list, tmpPair, position);
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public Boolean ModifyEventList(LoginSession session, String server, Int32 list, IEnumerable<EventDTO> events)
        {
            if (events != null && events.Any())
            {
                CheckListLock(server, list);
                var tmpList = events.Select(item => item.ToPair()).ToList();
                return _adcSrv.ModifyEventList(server, list, tmpList);
            }
            return false;
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public Boolean MoveEvent(LoginSession session, String server, Int32 list, Guid eventId, Guid destination)
        {
            CheckListLock(server, list);
            List<EventEventIdPair> tmpList = _adcSrv.GetEventsList(server, list).ToList();
            Int32 lookAhead = _adcSrv.GetLookAhead(server, list);
            CheckParametersForMoveMethod(tmpList, lookAhead, new List<Guid> {eventId}, destination);
            var res = _adcSrv.MoveEvent(server, list, eventId, destination);
            ClearDoneEventsAfterMove(session, server, list, tmpList.Where(item => item.ADCEventId == eventId).ToList());
            return res;
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public Boolean MoveEvents(LoginSession session, String server, Int32 list, IEnumerable<Guid> events, Guid destination)
        {
            if (events == null || !events.Any())
                return false;
            events = events.ToList();
            CheckListLock(server, list);
            List<EventEventIdPair> tmpList = _adcSrv.GetEventsList(server, list).ToList();
            Int32 lookAhead = _adcSrv.GetLookAhead(server, list);
            CheckParametersForMoveMethod(tmpList, lookAhead, events.ToList(), destination);
            var res = _adcSrv.MoveEvents(server, list, events, destination);
            ClearDoneEventsAfterMove(session, server, list, tmpList.Where(item => events.Contains(item.ADCEventId)).ToList());
            return res;
        }

        public Boolean DeleteEvent(LoginSession session, String server, Int32 list, Guid position)
        {
            CheckListLock(server, list);
            return _adcSrv.DeleteEvents(server, list, position, 1);
        }

        public Boolean DeleteEvents(LoginSession session, String server, Int32 list, Guid position, Int32 numberOfEvents)
        {
            if (numberOfEvents < 1) return false;
            CheckListLock(server, list);
            return _adcSrv.DeleteEvents(server, list, position, numberOfEvents);
        }

        public void DeleteEventsByGuid(LoginSession session, String server, Int32 list, IEnumerable<Guid> positions)
        {
            CheckListLock(server, list);

            var blocks = new Dictionary<Guid, Int32>();
            var events = GetList(session, server, list);
            var curIndex = 0;
            var prevIndex = 0;
            var curGuid = Guid.Empty;

            foreach (var ev in events)
            {
                if (positions.Contains(ev.AdcEventId))
                {
                    if (curIndex == prevIndex + 1 && curGuid != Guid.Empty)
                    {
                        blocks[curGuid] = blocks[curGuid] + 1;
                    }
                    else
                    {
                        blocks.Add(ev.AdcEventId, 1);
                        curGuid = ev.AdcEventId;
                    }
                    prevIndex = curIndex;
                }
                curIndex++;
            }

            foreach (var block in blocks)
            {
                _adcSrv.DeleteEvents(server, list, block.Key, block.Value);
            }
        }

        public void DeleteAllEvents(LoginSession session, String server, Int32 list)
        {
            CheckListLock(server, list);
            _adcSrv.DeleteAllEvents(server, list);
        }

        public void GangHold(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangHold(server, lists);
        }

        public void GangAirProtect(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangAirProtect(server, lists);
        }

        public void GangTensionRelease(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangTensionRelease(server, lists);
        }

        public void GangUnthread(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangUnthread(server, lists);
        }

        public void GangPlus1Sec(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangPlus1Sec(server, lists);
        }

        public void GangPlayGang(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangPlayGang(server, lists);
        }

        public void GangRoll(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangRoll(server, lists);
        }

        public void GangThread(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangThread(server, lists);
        }

        public void GangReady(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangReady(server, lists);
        }

        public void GangMinus1Sec(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangMinus1Sec(server, lists);
        }

        public void GangFreeze(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangFreeze(server, lists);
        }

        public void GangPlay(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangPlay(server, lists);
        }

        public void GangRecue(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangRecue(server, lists);
        }

        public void GangSkip(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangSkip(server, lists);
        }

        public void GangPlaySecondary(LoginSession session, String server, IEnumerable<Int32> lists)
        {
            _adcSrv.GangPlaySecondary(server, lists);
        }

        public void PerformListThread(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListThread(server, list);
        }

        public void PerformListUnthread(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListUnthread(server, list);
        }

        public void PerformListPlay(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListPlay(server, list);
        }

        public void PerformListHold(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListHold(server, list);
        }

        public void PerformListSkip(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListSkip(server, list);
        }

        public void PerformListFreeze(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListFreeze(server, list);
        }

        public void PerformListRoll(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListRoll(server, list);
        }

        public void PerformListRecue(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListRecue(server, list);
        }

        public void PerformListPlus1Sec(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListPlus1Sec(server, list);
        }

        public void PerformListMinus1Sec(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListMinus1Sec(server, list);
        }

        public void PerformListReady(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListReady(server, list);
        }

        public void PerformListProtect(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListProtect(server, list);
        }

        public void PerformAirProtect(LoginSession session, String server, Int32 list, IEnumerable<Guid> position)
        {
            _adcSrv.PerformAirProtect(server, list, position);
        }

        public void PerformListClearDone(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformListClearDone(server, list);
        }

        public void PerformEventClearDone(LoginSession session, String server, Int32 list, Guid position)
        {
            _adcSrv.PerformEventClearDone(server, list, position);
        }

        public void PerformEventsClearDone(LoginSession session, string server, int list, IEnumerable<Guid> position)
        {
            foreach (var guid in position)
            {
                _adcSrv.PerformEventClearDone(server, list, guid);
            }
        }

        public void PerformEventThread(LoginSession session, String server, Int32 list, Guid position)
        {
            _adcSrv.PerformEventThread(server, list, position);
        }

        public void PerformEventUnthread(LoginSession session, String server, Int32 list, Guid position)
        {
            _adcSrv.PerformEventUnthread(server, list, position);
        }

        public void PerformEventRecue(LoginSession session, String server, Int32 list, Guid position)
        {
            _adcSrv.PerformEventRecue(server, list, position);
        }

        public void PerformEventPreview(LoginSession session, String server, Int32 list, Guid position)
        {
            _adcSrv.PerformEventPreview(server, list, position);
        }

        public void ToggleLookahead(LoginSession session, String server, Int32 list)
        {
            _adcSrv.ToggleLookahead(server, list);
        }

        public void SetLookahead(LoginSession session, String server, Int32 list, Int32 lookahead)
        {
            _adcSrv.SetLookahead(server, list, lookahead);
        }

        public void PerformCutNext(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformCutNext(server, list);
        }

        public void PerformLetRoll(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformLetRoll(server, list);
        }

        public void PerformRollSecondary(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformRollSecondary(server, list);
        }

        public void PerformTensionRelease(LoginSession session, String server, Int32 list)
        {
            _adcSrv.PerformTensionRelease(server, list);
        }

        /// <summary>
        /// Get the list of currently connected Device Servers
        /// </summary>
        /// <param name="session">User session</param>
        /// <returns>The list of Device Server names of configured and is connected</returns>
        public IEnumerable<string> GetAvailableDeviceServers(LoginSession session)
        {
            return _adcSrv.GetAvailableDeviceServers();
        }

        /// <param name="session">User session</param>
        /// <returns>The list of all Device Server names of configured</returns>
        public IEnumerable<string> GetAllConfiguredServers(LoginSession session)
        {
            return _adcSrv.GetAllConfiguredServers();
        }

        /// <summary>
        /// Get List Count
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <returns>List Count on specified server</returns>
        public int GetListCount(String server)
        {
            return _adcSrv.GetListCount(server);
        }

        public IEnumerable<EventDTO> GetListPartial(LoginSession session, String server, Int32 list, Guid start, Int32 count)
        {
            return _adcSrv.GetListPartial(server, list, start, count).Select(pair => pair.ToDTO()).ToList();
        }

        public bool LockList(LoginSession session, String server, Int32 list, String clientName)
        {
            _adcSrv.CheckListAvailability(server, list);
            if (IsListAvailable(session, server, list))
            {
                var proxy = _callbackCreator();
                if (!IsListLocked(new Channel(server, list)))
                {
                    lock (_subscribers)
                    {
                        if (!_subscribers.Exists(client => client.IsCurrentCallback(proxy)))
                        {
                            _subscribers.Add(new AsyncListServiceClient(proxy));
                        }
                        _subscribers.Find(client => client.IsCurrentCallback(proxy)).LockList(new Channel(server, list), clientName);
                    }
                    OnListLocked(new Channel(server, list), clientName);
                }
                return true;
            }
            return false;
        }

        public void UnlockList(LoginSession session, String server, Int32 list, String clientName)
        {
            if (IsListLocked(new Channel(server, list)))
            {
                var proxy = _callbackCreator();
                OnListUnlocked(new Channel(server, list), clientName);
                lock (_subscribersLock)
                {
                    _subscribers.Find(client => client.IsCurrentCallback(proxy)).UnlockList(new Channel(server, list));
                }
            }
            else if (!IsListAvailable(session, server, list))
            {
                throw new ListServiceListLockedException(String.Format(@"List #{0} on server {1} is locked by another user.", list, server));
            }
        }

        public Boolean IsListAvailable(LoginSession session, String server, Int32 list)
        {
            _adcSrv.CheckListAvailability(server, list);
            var proxy = _callbackCreator();
            lock (_subscribersLock)
            {
                return
                    !this._subscribers.Exists(
                        client => client.IsListLockedBy(new Channel(server, list)) && !client.IsCurrentCallback(proxy));
            }
        }

        public Boolean CheckListCapacity(LoginSession session, String server, Int32 list, Int32 numberOfEvents)
        {
            var max = _adcSrv.GetMaxEventsInList(server, list);
            var evListCount = _adcSrv.GetEventsCount(server, list);
            return evListCount + numberOfEvents <= max;
        }

        public Int32 GetMaxEventsInList(LoginSession session, String server, Int32 list)
        {
            return _adcSrv.GetMaxEventsInList(server, list);
        }

        public IEnumerable<EventDTO> GetListPage(LoginSession session, string server, int listIndex, int offset, int count)
        {
            return _adcSrv.GetListPage(server, listIndex, offset, count).Select(pair => pair.ToDTO()).ToList();
        }

        public IEnumerable<EventDTO> GetListFiltered(LoginSession session, string server, int listIndex, int doneCount, int onAirCount, int nextCount, int errorCount)
        {
            return
                _adcSrv.GetListFiltered(server, listIndex, doneCount, onAirCount, nextCount, errorCount).Select(
                    pair => pair.ToDTO()).ToList();
        }

        public IEnumerable<IEnumerable<EventDTO>> GetListsByPeriod(LoginSession session, string server, IEnumerable<int> listIndexes, string from, string to)
        {
            var result = new List<List<EventDTO>>();

            foreach (var idx in listIndexes)
            {
                result.Add(_adcSrv.GetListByPeriod(server, idx, from, to).Select(pair => pair.ToDTO()).ToList());
            }

            return result;
        }

        public IEnumerable<EventDTO> GetListOfSecondaries(LoginSession session, string server, int list, Guid primary)
        {
            return _adcSrv.GetListOfSecondaries(server, list, primary).Select(pair => pair.ToDTO()).ToList();
        }

        public TimecodeCompareResult RippleTime(LoginSession session, String server, Int32 list, Guid position, out TimeCodeDTO holdtime)
        {
            TimeCode tc;
            TimecodeCompareResult result = _adcSrv.RippleTime(server, list, position, out tc);
            holdtime = new TimeCodeDTO(tc);
            return result;
        }

        public void PerformEventToggleHardStart(LoginSession session, String server, Int32 list, IEnumerable<Guid> position)
        {
            _adcSrv.PerformEventToggleHardStart(server, list, position);
        }

        public IEnumerable<EventDTO> GetPullDataList(LoginSession session, String server, Int32 list)
        {
            return _adcSrv.GetPullDataList(server, list).Select(pair => pair.ToDTO()).ToList();
        }

        public IEnumerable<String> GetCommercialContentInfo(LoginSession ls)
        {
            return Config.Instance.ConfigObject.CommercialContentIdentifiers.ToList();
        }

        public IEnumerable<Boolean> CheckMaterialIds(LoginSession session, String server, Int32 list, IEnumerable<String> ids)
        {
            return _adcSrv.CheckMaterialIds(server, list, ids);
        }

        #region Break-Away Feature

        public BreakAwayConfigurationDTO GetBreakAwayConfiguration()
        {
            return _breakAwayManager.GetConfiguration();
        }

        public Boolean SetBreakAwayConfiguration(BreakAwayConfigurationDTO breakAwayCfg)
        {
            return _breakAwayManager.SetConfiguration(breakAwayCfg);
        }

        public BreakAwayListStatusDTO GetBreakAwayListStatus(String server,Int32 list)
        {
            return _breakAwayManager.GetBreakAwayListStatus(server,list);
        }

        [ParameterInspectionBehavior(typeof(ServicesParameterInspector))]
        public void PerformBreakAway(String server, Int32 list, IEnumerable<EventDTO> baList)
        {
            _breakAwayManager.PerformBreakAway(server, list, baList);
        }

        public void PerformBreakAwayReturn(String server, Int32 list)
        {
            _breakAwayManager.PerformBreakAwayReturn(server,list);
        }

        public Guid GetOnAirEventPosition(String server, Int32 list, out TimeCode duration)
        {
            return _adcSrv.GetOnAirEventPosition(server, list, out duration);
        }

        public TimeCode GetServerTimeCode(String server, Int32 list)
        {
            return _adcSrv.GetServerTimeCode(server, list);
        }

        public void PerformBreakAwayFastBackUp(String server, Int32 baList, Guid position, Int32 txList)
        {
            _adcSrv.PerformBreakAwayFastBackUp(server, baList, position, txList);
        }

        public void PerformBreakAwayFastRestore(String server, Int32 balist, Guid brokenEventPos, Int32 txList)
        {
            _adcSrv.PerformBreakAwayFastRestore(server, balist, brokenEventPos, txList);
        }

        public void SetAOAttributeForListPair(String server, Int32 txList, Int32 baList, Boolean value)
        {
            _adcSrv.SetAOAttributeForListPair(server, txList, baList, value);
        }

        public Boolean GetAOAttribute(String server, Int32 txList)
        {
           return  _adcSrv.GetAOAttribute(server, txList);
        }

        public void ChangeBreakAwayState(String server, Int32 txList,Int32 baList,Boolean value)
        {
            _adcSrv.ChangeBreakAwayState(server,txList,baList,value);
        }

        public Boolean GetBreakAwayState(String server, Int32 list)
        {
            return _adcSrv.IsInBreakAway(server, list);
        }

        #endregion

        #endregion

        #region Callback functionality

        public bool RegisterListListener(LoginSession session, String server, int list)
        {
            try
            {
                IListServiceClient callback = _callbackCreator();

                lock (_subscribersLock)
                {
                    if (!_subscribers.Exists(client => client.IsCurrentCallback(callback)))
                    {
                        _subscribers.Add(new AsyncListServiceClient(callback));
                    }
                    _subscribers.Find(client => client.IsCurrentCallback(callback)).AddChannel(new Channel(server, list));
                }

                CheckClients(null, null);

                return true;
            }
            catch (CommunicationException ex)
            {
                ServiceLogger.Error("Error while registering ListListener", ex);
                return false;
            }
        }

        public void UnregisterListListener(LoginSession session, String server, int list)
        {
            try
            {
                IListServiceClient callback = _callbackCreator();

                lock (_subscribersLock)
                {
                    if (_subscribers.Exists(client => client.IsCurrentCallback(callback)))
                    {
                        _subscribers.Find(client => client.IsCurrentCallback(callback)).RemoveChannel(new Channel(server, list));
                    }
                    //_subscribers.RemoveAll(client => !client.HasCallbacks());
                    RemoveDeadClients();
                }
            }
            catch (CommunicationException ex)
            {
                ServiceLogger.Error("Error while unregistering ListListener", ex);
            }
        }

        public void UnregisterAllListListeners(LoginSession session, String server)
        {
            try
            {
                IListServiceClient callback = _callbackCreator();

                lock (_subscribersLock)
                {
                    if (_subscribers.Exists(client => client.IsCurrentCallback(callback)))
                    {
                        _subscribers.Find(client => client.IsCurrentCallback(callback)).RemoveAllChannels(server);
                    }
                    //_subscribers.RemoveAll(client => !client.HasCallbacks());
                    RemoveDeadClients();
                }
            }
            catch (CommunicationException ex)
            {
                ServiceLogger.Error("Error while unregistering ListListeners", ex);
            }
        }

        internal void OnListChanged(Object sender, Types.ListChangedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().ForEach(client => client.OnListChange(e.ServerName, e.ListNumber, e.ChangeType));
            }
        }

        private void OnEventsUpdated(object sender, EventsUpdatedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().ForEach(
                    client =>
                    client.OnEventsUpdated(e.ServerName, e.ListNumber,
                                           e.UpdatedEvents.Select(evt => evt.ToDTO())));
            }
        }

        private void OnEventsDeleted(object sender, EventsDeletedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().ForEach(
                    client =>
                    client.OnEventsDeleted(e.ServerName, e.ListNumber,
                                           e.DeletedEvents.Select(evt => evt.ToDTO())));
            }
        }

        private void OnEventsMoved(object sender, EventsMovedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().ForEach(client =>
                                              client.OnEventsMoved(e.ServerName, e.ListNumber, e.Source, e.Destination,
                                                                   e.MovedEvents));
            }
        }

        private void OnEventsAdded(object sender, EventsAddedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().ForEach(
                    client =>
                    client.OnEventsAdded(e.ServerName, e.ListNumber, e.AfterGuid,
                                         e.AddedEvents.Select(evt => evt.ToDTO())));
                _dataServiceManager.CreateTaskOnEventsAdded(e.ServerName, e.ListNumber, e.AddedEvents);
            }
        }

        private void OnListLocked(Channel list, String clientName)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ForEach(client => client.ListLocked(list.Server, list.List, clientName));
            }
            //ServiceLogger.Informational(String.Format(@"List #{0} on server {1} has been locked by {2}", list.List, list.Server, clientName));
        }

        private void OnListUnlocked(Channel list, String clientName)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ForEach(client => client.ListUnlocked(list.Server, list.List, clientName));
            }
            //ServiceLogger.Informational(String.Format(@"List #{0} on server {1} has been unlocked by {2}", list.List, list.Server, clientName));
        }

        private void OnBreakAwayStatusChanged(Object sender, BreakAwayListStatusChangedArgs e)
        {
            lock (_subscribersLock)
            {
                RemoveDeadClients();
                _subscribers.ForEach(client => client.OnBreakAwayListStatusChanged(e.Server,e.List,e.BreakAwayListStatus));
            }
        }

        public bool RegisterConnectionStateListener(LoginSession session, String server)
        {
            try
            {
                IListServiceClient callback = _callbackCreator();
                lock (_subscribersLock)
                {
                    if (!_subscribers.Exists(client => client.IsCurrentCallback(callback)))
                    {
                        _subscribers.Add(new AsyncListServiceClient(callback));
                    }
                    _subscribers.Find(client => client.IsCurrentCallback(callback)).AddServer(server);
                }
                return true;
            }
            catch (CommunicationException ex)
            {
                ServiceLogger.Error("Error while registering ConnectionStateListener", ex);
                return false;
            }
        }

        public void UnregisterConnectionStateListener(LoginSession session, String server)
        {
            try
            {
                IListServiceClient callback = _callbackCreator();
                lock (_subscribersLock)
                {
                    if (_subscribers.Exists(client => client.IsCurrentCallback(callback)))
                    {
                        _subscribers.Find(client => client.IsCurrentCallback(callback)).RemoveServer(server);
                    }
                    //_subscribers.RemoveAll(client => !client.HasCallbacks());
                    RemoveDeadClients();
                }
            }
            catch (CommunicationException ex)
            {
                ServiceLogger.Error("Error while unregistering ConnectionStateListener", ex);
            }
        }

        internal void OnServerStatusChanged(Object sender, ServerConnectionStatusChangedEventArgs e)
        {
            lock (_subscribersLock)
            {
                //_subscribers.RemoveAll(client => !client.IsAlive);
                RemoveDeadClients();
                _subscribers.ToList().
                    ForEach(client => client.OnConnectionStateChange(e.ServerName, e.Status));
            }
        }

        internal void RemoveDeadClients()
        {
            _subscribers.RemoveAll(client =>
            {
                var reasonIsNotAlive = !client.IsAlive;
                var reasonNoCallbacks = !client.HasCallbacks();
                if (reasonIsNotAlive || reasonNoCallbacks)
                {
                    ServiceLogger.InformationalFormat("Client {0} was removed (reason:{1}{2})",
                                                      client.GetClientName(),
                                                      reasonIsNotAlive ? " is not 'alive'" : String.Empty,
                                                      reasonNoCallbacks ? " doesn't has subscriptions" : String.Empty);
                    client.Dispose();
                }
                return reasonIsNotAlive || reasonNoCallbacks;
            });
        }

        #endregion
    }
}