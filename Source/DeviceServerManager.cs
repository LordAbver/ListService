using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Harris.Automation.ADC.DeviceServerAdapter;
using Harris.Automation.ADC.Services.Common;
using Harris.Automation.ADC.Types;
using Harris.Automation.ADC.Types.Events;


namespace Harris.Automation.ADC.Services.ListService
{
    public class DeviceServerManager : DeviceServerManagerThreadedBase
    {
        // Dictionary of a lists on servers
        private ConcurrentDictionary<String, List<ADCListHelper>> _lists;
        private ConcurrentBag<String> _serversToInitialize;
        private ConcurrentBag<String> _serversToRemove;
        private Boolean _bDisposed;
        private int _lowPriorityCounter;

        public event EventHandler<ListChangedEventArgs> ListChanged;
        public event EventHandler<EventsAddedEventArgs> EventsAdded;
        public event EventHandler<EventsUpdatedEventArgs> EventsUpdated;
        public event EventHandler<EventsDeletedEventArgs> EventsDeleted;
        public event EventHandler<EventsMovedEventArgs> EventsMoved;

        public DeviceServerManager()
            : base(Config.Instance.ConfigObject.ADCConnectionOptions.ApplicationName)
        {
            _lists = new ConcurrentDictionary<String, List<ADCListHelper>>();
            _serversToInitialize = new ConcurrentBag<String>();
            _serversToRemove = new ConcurrentBag<String>();
            Start();
        }

        protected override void Dispose(Boolean disposing)
        {
            lock (_disposeLock)
            {
                if (!this._bDisposed)
                {
                    // Free the unmanaged sources
                    this.Stop();
                    // Free the managed sources
                    if (disposing)
                    {
                        _lists = null;
                        _serversToInitialize = null;
                        ListChanged = null;
                        EventsAdded = null;
                        EventsUpdated = null;
                        EventsDeleted = null;
                    }
                    base.Dispose(disposing);
                    this._bDisposed = true;
                } 
            }

        }

        protected override void Execute()
        {
            try
            {
            while (!this._bStop)
            {
                _statusUpdateHandle.WaitOne();

                while (!_serversToInitialize.IsEmpty)
                {
                    String serverName;
                    if (_serversToInitialize.TryTake(out serverName))
                    {
                        var server = _servers.FirstOrDefault(adapter => adapter.ServerName == serverName);
                        ServerAdapter tmpServer = server;
                        if (tmpServer != null && !this._lists.Keys.Any(listServer => listServer == tmpServer.ServerName))
                        {
                            Logger.ServiceLogger.Informational(String.Format("Initialization for server {0} started.", server.ServerName));
                            List<String> listNames = server.GetListNames();
                            List<ADCListHelper> tmpList = new List<ADCListHelper>();
                            for (Int32 i = 0; i < server.NumberOfLists; i++)
                            {
                                ADCListHelper newListHelper = new ADCListHelper(server, i + 1, true);
                                try
                                {
                                    newListHelper.ListName = listNames.ElementAt(i);
                                }
                                catch (ArgumentNullException)
                                {
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                }
                                newListHelper.OnListChange += this.OnListChange;
                                newListHelper.OnDisplayChange += this.OnListChange;
                                newListHelper.OnSystemChange += this.OnListChange;
                                newListHelper.OnEventsAdded += this.OnEventsAdded;
                                newListHelper.OnEventsUpdated += this.OnEventsUpdated;
                                newListHelper.OnEventsDeleted += this.OnEventsDeleted;
                                newListHelper.OnEventsMoved += this.OnEventsMoved;
                                tmpList.Add(newListHelper);
                            }
                            while (!this._lists.TryAdd(server.ServerName, tmpList))
                            {
                            }
                            this.OnServerInitialized(tmpServer);
                            Logger.ServiceLogger.Informational(String.Format("Initialization for server {0} has been completed successfully.", server.ServerName));
                        }
                    }
                }

                while (!_serversToRemove.IsEmpty)
                {
                    String serverName;
                    if (_serversToRemove.TryTake(out serverName))
                    {
                        RemoveAllListHelpersForServer(serverName);
                    }
                }

                var serversToUpdate = new List<String>();
                String srv;
                while (_updates.TryDequeue(out srv))
                {
                    if (!serversToUpdate.Contains(srv))
                    {
                        serversToUpdate.Add(srv);
                    }
                }

                foreach (var serverName in serversToUpdate)
                {
                    if (_servers.Any(server => server.ServerName == serverName
                                               && server.ServerStatus == ServerStatus.Connected))
                    {
                        // Execute NetworkBackground for each listHelper
                        try
                        {
                            if (this._lists.Keys.Any(key => key == serverName))
                            {
                                List<ADCListHelper> listHelpers = this._lists[serverName];
                                if (listHelpers != null)
                                {
                                    listHelpers.ForEach(lc => lc.NetworkBackground());
                                }
                                this.IsReady = true;
                            }

                            LowPriorityTasks(_lowPriorityCounter);
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    }
                    _lowPriorityCounter++;
                }
                _statusUpdateHandle.Reset();
            }
        }
            catch (ThreadInterruptedException)
            {
            }
        }

        protected override void OnServerConnected(string serverName)
        {
            if (!_serversToInitialize.Contains(serverName))
            {
                _serversToInitialize.Add(serverName);
            }
            _statusUpdateHandle.Set();
        }

        protected override void OnServerDisconnected(string serverName)
        {
            var server = _servers.FirstOrDefault(adapter => adapter.ServerName == serverName);
            if (server != null && !server.TryToConnect && !_serversToRemove.Contains(serverName))
            {
                _serversToRemove.Add(serverName);
            }

            _statusUpdateHandle.Set();
        }

        private void RemoveAllListHelpersForServer(String server)
        {
            try
            {
                var serverAndHelpers = _lists[server];
                if (serverAndHelpers != null)
                {
                    foreach (var listHelper in serverAndHelpers)
                    {
                        if (listHelper != null) 
                            listHelper.Dispose();
                    }
                    serverAndHelpers.Clear();
                }

                List<ADCListHelper> tmpList;
                while (!_lists.TryRemove(server, out tmpList))
                {
                    if (!_lists.Keys.Contains(server))
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
            }
        }

        private void OnEventsDeleted(object sender, EventsDeletedEventArgs e)
        {
            if (EventsDeleted != null)
                EventsDeleted(this, e);
        }

        private void OnEventsUpdated(object sender, EventsUpdatedEventArgs e)
        {
            if (EventsUpdated != null)
                EventsUpdated(this, e);
        }

        private void OnEventsMoved(object sender, EventsMovedEventArgs e)
        {
            if (EventsMoved != null)
                EventsMoved(this, e);
        }

        private void OnEventsAdded(object sender, EventsAddedEventArgs e)
        {
            if (EventsAdded != null)
                EventsAdded(this, e);
        }

        private ADCListHelper GetListHelper(String server, Int32 list)
        {
            if (_lists == null)
            {
                throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
            }
            var targetServer = _lists.FirstOrDefault(listServer => listServer.Key == server);
            if (targetServer.Equals(default(KeyValuePair<string, List<ADCListHelper>>)))
            {
                if (_servers.Any(serverAdapter => serverAdapter.ServerName == server))
                {
#if (DEBUG)
                    Logger.ServiceLogger.Warning(String.Format("Server ({0}) is not Running. There is no list helpers.", server));
#endif
                    throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
                }
#if (DEBUG)
                Logger.ServiceLogger.Warning(String.Format("Server ({0}) is not Configured. '_servers' collection does not contain this server.", server));
#endif
                throw new ServerNotExistException(ServerNotExistException.GetFormattedMessage(server));
            }
            if (targetServer.Value == null)
            {
#if (DEBUG)
                Logger.ServiceLogger.Warning(String.Format("Server ({0}) is not Running. The list helpers collection is null.", server));
#endif
                throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
            }
            var listHelper = targetServer.Value.FirstOrDefault(helper => helper.GetListNumber() == list);
            if (listHelper == null)
            {
                throw new ListServiceListNotEnabledException(ListServiceListNotEnabledException.GetFormattedMessage(server, list));
            }
            if (!listHelper.DS.IsAlive)
            {
#if (DEBUG)
                Logger.ServiceLogger.Warning(String.Format("Server ({0}) is not Configured. The server adapter is not alive.", server));
#endif
                throw new ServerNotExistException(ServerNotExistException.GetFormattedMessage(server));
            }

            return listHelper;
        }

        public void UpdateListNames()
        {
            foreach (ServerAdapter server in this._servers)
            {
                if (server.ServerStatus == ServerStatus.Connected)
                {
                    List<String> listNames = server.GetListNames();
                    if (listNames != null)
                    {
                        List<ADCListHelper> listHelpers;
                        if (_lists.TryGetValue(server.ServerName, out listHelpers))
                        {
                            //List<ADCListHelper> listHelpers = this._lists[server.ServerName];
                            if (listHelpers != null)
                            {
                                if (listNames.Count == listHelpers.Count)
                                {
                                    int i = 0;
                                    foreach (ADCListHelper helper in listHelpers)
                                    {
                                        try
                                        {
                                            helper.ListName = listNames.ElementAt(i);
                                        }
                                        catch (ArgumentNullException)
                                        {
                                        }
                                        catch (ArgumentOutOfRangeException)
                                        {
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Boolean IsTimeToFire(int counter, int ticks)
        {
            return counter % ticks == 0;
        }

        private void LowPriorityTasks(int counter)
        {
            if (IsTimeToFire(counter, 150)) // fire every 150 frames
            {
                UpdateListNames();
            }
        }

        /// <summary>
        /// Get events count from the specified server/list 
        /// </summary>
        public Int32 GetEventsCount(String server, Int32 list)
        {
            ADCListHelper lc = GetListHelper(server, list);
            return lc.GetEventsCount();
        }

        /// <summary>
        /// Get events count from the specified server/list 
        /// </summary>
        public IEnumerable<EventEventIdPair> GetEventsList(String server, Int32 list)
        {
            ADCListHelper lc = GetListHelper(server, list);
            IEnumerable<EventEventIdPair> resList = lc.GetList();
            return resList;
        }

        public String GetListName(String server, Int32 list)
        {
            String result;
            //UpdateListNames();
            ADCListHelper lc = GetListHelper(server, list);
            result = lc.GetListName();

            return result;
        }

        public TypeOfList GetListType(String server, Int32 list)
        {
            ADCListHelper lc = GetListHelper(server, list);
            return lc.GetListType();
        }

        public String GetExtListName(String server, Int32 list)
        {
            String result;
            ADCListHelper lc = GetListHelper(server, list);
            result = lc.GetExtListName();

            return result;
        }

        public void SetExtListName(String server, Int32 list, String filename)
        {
            GetListHelper(server, list).SetExtListName(filename);
        }

        /// <summary>
        /// Get List Count
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <returns>List Count on specified server</returns>
        public Int32 GetListCount(String server)
        {
            ADCServer deviceServer;

            if (_lists == null)
            {
                throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
            }
            var targetServer = _lists.FirstOrDefault(listServer => listServer.Key == server);
            if (targetServer.Equals(default(KeyValuePair<string, List<ADCListHelper>>)))
            {
                if (_servers.Any(serverAdapter => serverAdapter.ServerName == server))
                {
                    throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
                }
                throw new ServerNotExistException(ServerNotExistException.GetFormattedMessage(server));
            }
            if (targetServer.Value == null)
            {
                throw new ServerNotRunningException(ServerNotRunningException.GetFormattedMessage(server));
            }
            deviceServer = targetServer.Value.ElementAt(0).DS;
            if (deviceServer != null && deviceServer.IsAlive)
            {
                return deviceServer.NumberOfLists;
            }
            return 0;
        }

        public IEnumerable<ListStateValue> GetListState(String server, Int32 list)
        {
            IEnumerable<ListStateValue> result;
            ADCListHelper lc = GetListHelper(server, list);
            result = lc.GetListState();
            return result;
        }

        public EventEventIdPair InsertEvent(String server, Int32 list, EventEventIdPair ev,InsertPlaceType place, Guid position)
        {
            ADCListHelper lc = GetListHelper(server, list);
            List<EventEventIdPair> el = new List<EventEventIdPair> { ev };
            var res = lc.InsertEvents(position, el, place);
            return res.FirstOrDefault();
        }

        public IEnumerable<EventEventIdPair> InsertEventList(String server, Int32 list, IEnumerable<EventEventIdPair> events,InsertPlaceType place, Guid position )
        {
            IEnumerable<EventEventIdPair> result;
            result = GetListHelper(server, list).InsertEvents(position, events, place);
            return result;
        }

        public Boolean ModifyEvent(String server, Int32 list, EventEventIdPair ev, Guid position)
        {
            return GetListHelper(server, list).ModifyEvent(position, ev.ADCEvent);
        }

        public Boolean ModifyEventList(String server, Int32 list, List<EventEventIdPair> events)
        {
            return GetListHelper(server, list).ModifyEventList(events);
        }

        public Boolean MoveEvent(String server, Int32 list, Guid ev, Guid destination)
        {
            return GetListHelper(server, list).MoveEvents(new List<Guid> { ev }, destination);
        }

        public Boolean MoveEvents(String server, Int32 list, IEnumerable<Guid> events, Guid destination)
        {
            return GetListHelper(server, list).MoveEvents(events, destination);
        }

        public Boolean DeleteEvents(String server, Int32 list, Guid position, Int32 numberOfEvents)
        {
            return GetListHelper(server, list).DeleteEvents(position, numberOfEvents);
        }

        public void DeleteAllEvents(String server, Int32 list)
        {
            GetListHelper(server, list).DeleteAllEvents();
        }

        private UInt32 ListSetToInt(IEnumerable<Int32> lists)
        {
            UInt32 result = 0;

            foreach (Int32 lst in lists)
            {
                if (lst > 0)
                {
                    result |= (UInt32)1 << lst;
                }
            }

            return result;
        }

        public void GangHold(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangHold(ListSetToInt(lists));
        }

        public void GangAirProtect(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangAirProtect(ListSetToInt(lists));
        }

        public void GangTensionRelease(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangTensionRelease(ListSetToInt(lists));
        }

        public void GangUnthread(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangUnthread(ListSetToInt(lists));
        }

        public void GangPlus1Sec(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangPlus1Sec(ListSetToInt(lists));
        }

        public void GangPlayGang(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangPlayGang(ListSetToInt(lists));
        }

        public void GangRoll(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangRoll(ListSetToInt(lists));
        }

        public void GangThread(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangThread(ListSetToInt(lists));
        }

        public void GangReady(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangReady(ListSetToInt(lists));
        }

        public void GangMinus1Sec(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangMinus1Sec(ListSetToInt(lists));
        }

        public void GangFreeze(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangFreeze(ListSetToInt(lists));
        }

        public void GangPlay(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangPlay(ListSetToInt(lists));
        }

        public void GangRecue(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangRecue(ListSetToInt(lists));
        }

        public void GangSkip(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangSkip(ListSetToInt(lists));
        }

        public void GangPlaySecondary(String server, IEnumerable<Int32> lists)
        {
            GetListHelper(server, 1).GangPlaySecondary(ListSetToInt(lists));
        }

        public void PerformListThread(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListThread();
        }

        public void PerformListUnthread(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListUnthread();
        }

        public void PerformListPlay(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListPlay();
        }

        public void PerformListHold(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListHold();
        }

        public void PerformListSkip(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListSkip();
        }

        public void PerformListFreeze(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListFreeze();
        }

        public void PerformListRoll(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListRoll();
        }

        public void PerformListRecue(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListRecue();
        }

        public void PerformListPlus1Sec(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListPlus1Sec();
        }

        public void PerformListMinus1Sec(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListMinus1Sec();
        }

        public void PerformListReady(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListReady();
        }

        public void PerformListProtect(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListProtect();
        }

        public void PerformAirProtect(String server, Int32 list, IEnumerable<Guid> position)
        {
            var listHelper = GetListHelper(server, list);
            foreach (var guid in position)
            {
                listHelper.PerformAirProtect(guid);
            }
        }

        public void PerformEventPreview(String server, Int32 list, Guid position)
        {
            GetListHelper(server, list).PerformEventPreview(position);
        }

        public void ToggleLookahead(String server, Int32 list)
        {
            GetListHelper(server, list).ToggleLookahead();
        }

        public void SetLookahead(String server, Int32 list, Int32 lookahead)
        {
            GetListHelper(server, list).SetLookahead(lookahead);
        }

        public void PerformCutNext(String server, Int32 list)
        {
            GetListHelper(server, list).PerformCutNext();
        }

        public void PerformLetRoll(String server, Int32 list)
        {
            GetListHelper(server, list).PerformLetRoll();
        }

        public void PerformRollSecondary(String server, Int32 list)
        {
            GetListHelper(server, list).PerformRollSecondary();
        }

        public void PerformTensionRelease(String server, Int32 list)
        {
            GetListHelper(server, list).PerformTensionRelease();
        }

        public void PerformListClearDone(String server, Int32 list)
        {
            GetListHelper(server, list).PerformListClearDone();
        }

        public void PerformEventClearDone(String server, Int32 list, Guid position)
        {
            GetListHelper(server, list).PerformEventClearDone(position);
        }

        public void PerformEventThread(String server, Int32 list, Guid position)
        {
            GetListHelper(server, list).PerformEventThread(position);
        }

        public void PerformEventUnthread(String server, Int32 list, Guid position)
        {
            GetListHelper(server, list).PerformEventUnthread(position);
        }

        public void PerformEventRecue(String server, Int32 list, Guid position)
        {
            GetListHelper(server, list).PerformEventRecue(position);
        }

        public void PerformBreakAwayFastBackUp(String server, Int32 baList, Guid position, Int32 txList)
        {
            var txListPosition = GetListHelper(server, txList).GetEventIndex(position);
            GetListHelper(server, baList).PerformBreakAwayFastBackUp(txListPosition, txList);
        }

        public void PerformBreakAwayFastRestore(String server, Int32 balist, Guid brokenEventPos, Int32 txList)
        {
            var intPosition = GetListHelper(server, balist).GetEventIndex(brokenEventPos);
            GetListHelper(server, balist).PerformBreakAwayFastRestore(intPosition, txList);
        }

        public void SetAOAttributeForListPair(String server, Int32 txList,Int32 baList,Boolean value)
        {
            GetListHelper(server, txList).SaveAOAttribute = value;
            GetListHelper(server, baList).SaveAOAttribute = value;
        }

        public Boolean GetAOAttribute(String server, Int32 txList)
        {
            return GetListHelper(server, txList).SaveAOAttribute;
        }

        public Boolean IsInBreakAway(String server, Int32 txList)
        {
            return GetListHelper(server, txList).InBreakAway; 
        }

        public void ChangeBreakAwayState(String server, Int32 txList,Int32 baList,Boolean value)
        {
            GetListHelper(server, txList).InBreakAway = value;
            GetListHelper(server, baList).InBreakAway = value;
        }

        public Int32 GetLookAhead(String server, Int32 list)
        {
            Int32 result;
            ADCListHelper lc = GetListHelper(server, list);
            result = lc.GetLookAhead();
            return result;
        }

        public IEnumerable<EventEventIdPair> GetListPartial(String server, Int32 list, Guid start, Int32 count)
        {
            IEnumerable<EventEventIdPair> result;
            result = GetListHelper(server, list).GetList().SkipWhile(pair => pair.ADCEventId != start).
                    Take(count);
            return result;
        }


        public IEnumerable<EventEventIdPair> GetListPage(String server, Int32 list, Int32 offset, Int32 count)
        {
            var eventList = GetListHelper(server, list).GetList().ToList();
            var maxCount = Math.Min(offset + count, eventList.Count);
            var result = eventList.Skip(offset).Take(maxCount - offset);
            return result;
        }


        public IEnumerable<EventEventIdPair> GetListFiltered(
            String server, Int32 list, Int32 doneCount, Int32 onAirCount, Int32 nextCount, Int32 errorCount)
        {

            List<EventEventIdPair> result = new List<EventEventIdPair>();

            List<EventEventIdPair> all;
            all = GetListHelper(server, list).GetList().ToList();

            int done = 0, onair = 0, next = 0, error = 0;
            foreach (var ev in all.Where(it => it.IsPrimary))
            {
                if (done == doneCount && onair == onAirCount && next == nextCount && error == errorCount) break;
                var mapped = Extensions.Map(ev.ADCEvent.EventStatus, ev);
                switch (mapped)
                {
                    case EventStatus.Done:
                        if (done < doneCount || doneCount < 0)
                        {
                            result.Add(ev);
                            done++;
                        }
                        break;
                    case EventStatus.Running:
                        if (onair < onAirCount || onAirCount < 0)
                        {
                            result.Add(ev);
                            onair++;
                        }
                        break;
                    case EventStatus.Cued:
                    case EventStatus.Unknown:
                        if (next < nextCount || nextCount < 0)
                        {
                            result.Add(ev);
                            next++;
                        }
                        break;
                    case EventStatus.Error:
                        if (error < errorCount || errorCount < 0)
                        {
                            result.Add(ev);
                            error++;
                        }
                        break;
                }
            }

            return result;
        }

        public IEnumerable<EventEventIdPair> GetListByPeriod(String server, Int32 list, String from, String to)
        {
            List<EventEventIdPair> all = GetListHelper(server, list).GetList().ToList();
            var adapter = _servers.First(srv => srv.ServerName == server);
            List<EventEventIdPair> result = new List<EventEventIdPair>();
            TimeCode fromTC;
            TimeCode toTC;
            try
            {
                fromTC = new TimeCode(adapter.FrameRate, adapter.DropFrame, from);
                toTC = new TimeCode(adapter.FrameRate, adapter.DropFrame, to);
            }
            catch (TimeCodeException ex)
            {
                Logger.ServiceLogger.Error(ex.Message, ex.StackTrace);
                throw new ServiceParametersValidationException(ex.Message);
            }

            EventEventIdPair currentPrimary = null;
            TimeCode currentPrimaryStart = default(TimeCode);
            TimeCode currentPrimaryEnd = default(TimeCode);
            all.ForEach(e =>
            {
                if (e.IsPrimary)
                {
                    currentPrimary = e;
                    currentPrimaryStart = new TimeCode(adapter.FrameRate, adapter.DropFrame, currentPrimary.ADCEvent.OnAirTime);
                    currentPrimaryEnd = currentPrimaryStart + new TimeCode(adapter.FrameRate, adapter.DropFrame, currentPrimary.ADCEvent.Duration);
                }

                TimeCode
                    start = new TimeCode(adapter.FrameRate, adapter.DropFrame, e.ADCEvent.OnAirTime);
                TimeCode
                    end = start + new TimeCode(adapter.FrameRate, adapter.DropFrame, e.ADCEvent.OnAirTime);
                if ((e.IsPrimary &&
                    start != TimeCode.Default &&
                    !start.IsEmpty &&
                    ((fromTC <= start && start < toTC) || (fromTC < end && end < toTC))) ||
                    (!e.IsPrimary &&
                    !currentPrimaryStart.IsEmpty &&
                    ((fromTC <= currentPrimaryStart && currentPrimaryStart < toTC) || (fromTC < currentPrimaryEnd && currentPrimaryEnd < toTC))))
                {
                    e.RelatedPrimary = currentPrimary == null ? default(Guid) : currentPrimary.ADCEventId;
                    result.Add(e);
                }
            });

            return result;
        }

        public IEnumerable<EventEventIdPair> GetListOfSecondaries(String server, Int32 list, Guid primary)
        {
            var result = new List<EventEventIdPair>();

            List<EventEventIdPair> all;
            all = GetListHelper(server, list).GetList().ToList();

            var primaryFound = false;
            foreach (var item in all)
            {
                if (primaryFound)
                {
                    if (item.IsPrimary)
                    {
                        break;
                    }
                    item.RelatedPrimary = primary;
                    result.Add(item);
                }
                else if (item.ADCEventId == primary)
                {
                    if (item.IsPrimary)
                    {
                        primaryFound = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public TimecodeCompareResult RippleTime(string server, Int32 list, Guid position, out TimeCode holdtime)
        {
            var result = TimecodeCompareResult.Equal;
            
            var allFromPosition = GetListHelper(server, list).GetList().SkipWhile(pair => pair.ADCEventId != position).Select(eventEventIdPair => new EventEventIdPair
            {
                ADCEvent = (Event)eventEventIdPair.ADCEvent.Clone(),
                ADCEventId = eventEventIdPair.ADCEventId
            }).ToList();

            holdtime = TimeCode.Default;
            if (allFromPosition.Count > 1)
            {
                for (int i = 0; i < allFromPosition.Count; i++)
                {
                    var currEvent = allFromPosition[i];
                    if (currEvent.ADCEvent.EventType != ADCEventType.Break)
                    {
                        ServerAdapter serverAdapter = _servers.FirstOrDefault(adapter => adapter.ServerName == server);
                        var currOnAir = new TimeCode(serverAdapter.FrameRate, serverAdapter.DropFrame, currEvent.ADCEvent.OnAirTime);
                        if (currOnAir != TimeCode.Default && currEvent.ADCEvent.EventControl.Contains(EventControlType.AutoTimed))
                        {
                            TimeCode currTimecode = TimeCode.Empty;
                            if (serverAdapter != null)
                            {
                                currTimecode = serverAdapter.CurrentTimecode;
                            }
                            TimeCode hardTimecode = currOnAir;
                            if (hardTimecode < currTimecode)
                            {
                                TimeCode hours24 = TimeCode.Empty;
                                hours24.InitTime(TimeCode.ToBCD(24), 0, 0, 0);
                                hardTimecode += hours24;
                            }
                            if (i > 0)
                            {
                                if (holdtime == TimeCode.Default)
                                {
                                    holdtime = TimeCode.Empty;
                                }
                                
                                if (holdtime != hardTimecode)
                                {
                                    if (holdtime > hardTimecode)
                                    {
                                        result = TimecodeCompareResult.Greater;
                                        TimeCode tempTC = hardTimecode - holdtime;
                                        if (tempTC.Hour < TimeCode.ToBCD(12))
                                        {
                                            result = TimecodeCompareResult.Less;
                                        }
                                    }
                                    else
                                    {
                                        result = TimecodeCompareResult.Less;
                                        if (hardTimecode < holdtime && hardTimecode.Hour > TimeCode.ToBCD(12) && holdtime.Hour < TimeCode.ToBCD(12))
                                        {
                                            result = TimecodeCompareResult.Greater;
                                            holdtime -= hardTimecode;
                                        }
                                        else
                                        {
                                            holdtime = hardTimecode - holdtime;
                                        }
                                    }
                                }
                            }
                        }
                        if (RippleEvent(ref currEvent, ref holdtime))
                        {
                            ModifyEvent(server, list, currEvent, currEvent.ADCEventId);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        private Boolean RippleEvent(ref EventEventIdPair evPair, ref TimeCode holdtime)
        {
            Boolean result = true;
            if (evPair.ADCEvent.EventType == ADCEventType.Primary && !evPair.ADCEvent.EventControl.Contains(EventControlType.AutoRecord))
            {
                var evPairOnAir = new TimeCode(holdtime.FrameRate, holdtime.DropFrame, evPair.ADCEvent.OnAirTime);
                var evPairDuration = new TimeCode(holdtime.FrameRate, holdtime.DropFrame, evPair.ADCEvent.Duration);
                if (holdtime == TimeCode.Default) // First iteration
                {
                    result = false;
                    holdtime = TimeCode.Empty;
                    if (evPairOnAir != TimeCode.Default)
                    {
                        holdtime = evPairOnAir + evPairDuration;
                    }
                }
                else // Second and all next iterations
                {
                    if (evPair.ADCEvent.TransitionEffect == Transition.Mix ||
                        evPair.ADCEvent.TransitionEffect == Transition.MixedAV ||
                        evPair.ADCEvent.TransitionEffect == Transition.Wipe)
                    {
                        TimeCode subSeed = TimeCode.Empty;
                        byte frameCount = 30;
                        int effectSpeed = 60;
                        if (evPairOnAir.FrameRate == FrameRateEnum.PAL)
                        {
                            frameCount = 25;
                        }
                        
                        switch (evPair.ADCEvent.TransitionEffectRate)
                        {
                            case SwitchRate.Slow:
                                effectSpeed = 60;
                                break;
                            case SwitchRate.Medium:
                                effectSpeed = 30;
                                break;
                            case SwitchRate.Fast:
                                effectSpeed = 15;
                                break;
                        }

                        subSeed.InitTime(0, 0, TimeCode.ToBCD((byte)(effectSpeed / frameCount)), TimeCode.ToBCD((byte)(effectSpeed % frameCount)));
                        holdtime -= subSeed;
                    }

                    if (evPairOnAir != TimeCode.Default)
                    {
                        evPair.ADCEvent.OnAirTime = TimeCode.Int32ToBCD(holdtime.ToInt32());
                    }

                    if (evPairDuration != TimeCode.Default)
                    {
                        holdtime += evPairDuration;
                    }
                }
            }
            else
            {
                result = false;
            }
            
            return result;
        }

        private void OnListChange(Object sender, ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        internal void CheckListAvailability(String server, Int32 list)
        {
            GetListHelper(server, list);
        }

        public void PerformEventToggleHardStart(String server, Int32 list, IEnumerable<Guid> position)
        {
            GetListHelper(server, list).PerformEventToggleHardStart(position);
        }

        public IEnumerable<EventEventIdPair> GetPullDataList(String server, Int32 list)
        {
            return GetListHelper(server, list).GetPullDataList();
        }

        public Int32 GetMaxEventsInList(String server, Int32 list)
        {
            return GetListHelper(server, list).GetMaxEventsInList();
        }

        public IEnumerable<Boolean> CheckMaterialIds(String server, Int32 list, IEnumerable<String> ids)
        {
            return GetListHelper(server, list).CheckMaterialIds(ids);
        }

        public void SetDatabaseFields(String server, Int32 list, IEnumerable<EventsDatabaseFields> eventsDatabaseFields)
        {
            try
            {
                GetListHelper(server, list).SetDatabaseFields(eventsDatabaseFields);
            }
            catch (ServerNotRunningException){}
            catch (ServerNotExistException){}
            catch (ListServiceListNotEnabledException){}
        }

        public Guid GetOnAirEventPosition(String server,Int32 list,out TimeCode duration)
        {
            return GetListHelper(server, list).GetOnAirEventPosition(out duration);
        }

        public TimeCode GetServerTimeCode(String server,Int32 list)
        {
            return GetListHelper(server, list).GetServerTimeCode();
        }
    }
}