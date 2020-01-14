using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Harris.Automation.ADC.DeviceServerAdapter;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Events;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Timecode;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;
using Harris.Automation.ADC.Types;
using Harris.Automation.ADC.Types.Events;

namespace Harris.Automation.ADC.Services.ListService.Source.Features.BreakAway
{
    class BreakAwayManager:IDisposable
    {
        #region Private fields

        private BreakAwayCfgHelper _breakAwayCfg;
        private LoginSession _loginSession;
        private readonly ListService _listService;
        private const String CLIENT_LOCKER_NAME = "List Service (Break-Away)";
        private BreakAwayConfigurationDTO _configuration;
        private readonly TimeSpan _breakAwayLatency;

        #endregion

        #region Public fields
        public event EventHandler<BreakAwayListStatusChangedArgs> OnBreakAwayListStatusChanged = null;
        #endregion

        #region Constructor/Disposer

        public BreakAwayManager(ListService listServiceInstance)
        {
            _breakAwayCfg = new BreakAwayCfgHelper();
            _configuration = _breakAwayCfg.GetConfiguration();
            _loginSession = new LoginSession();
            _listService = listServiceInstance;
            _breakAwayLatency = GetBreakAwayLatencyValue();
        }

        public void Dispose()
        {
            _breakAwayCfg = null;
            _configuration = null;
            _loginSession = null;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods
        public BreakAwayConfigurationDTO GetConfiguration()
        {
            return (_configuration.GroupList == null)
                       ? _breakAwayCfg.GetConfiguration()
                       : _configuration;
        }

        public Boolean SetConfiguration(BreakAwayConfigurationDTO breakAwayConfiguration)
        {
            if (_breakAwayCfg.SetConfiguration(breakAwayConfiguration))
            {
                if(_configuration.GroupList != null)
                {
                    var newChannels = GetAllBreakAwayChannels(breakAwayConfiguration).ToList();
                    var currentChannels = GetAllBreakAwayChannels(_configuration).ToList();
                    var channelsToRemove = Compare(currentChannels, newChannels);
                    foreach (var ch in channelsToRemove)
                    {
                        var chStatus = GetBreakAwayListStatus(ch.ServerName, ch.List);
                        if (OnBreakAwayListStatusChanged == null) continue;
                        // Channel was removed from configuration
                        chStatus.HasBreakAway = false;
                        OnBreakAwayListStatusChanged(this, new BreakAwayListStatusChangedArgs(ch.ServerName, ch.List, chStatus));
                    }
                    foreach (var newCh in newChannels)
                    {
                        var newStatus = GetBreakAwayListStatus(newCh.ServerName,newCh.List,breakAwayConfiguration);
                        var currentCh =currentChannels.FirstOrDefault(ch => ch.Equals(newCh));
                        if(currentCh != null)
                        {
                            var currentStatus = GetBreakAwayListStatus(newCh.ServerName, newCh.List);
                            if(!currentStatus.Equals(newStatus))
                            {
                                if (OnBreakAwayListStatusChanged != null)
                                {
                                    //Channel was updated
                                    OnBreakAwayListStatusChanged(this,new BreakAwayListStatusChangedArgs(newCh.ServerName,newCh.List,newStatus));
                                }
                            }
                        }
                        else
                        {
                            if (OnBreakAwayListStatusChanged != null)
                            {
                                //Channel was added
                                newStatus.HasBreakAway = true;
                                OnBreakAwayListStatusChanged(this, new BreakAwayListStatusChangedArgs(newCh.ServerName, newCh.List, newStatus));
                            }
                        }
                    }
                    _configuration = breakAwayConfiguration;
                }
                else
                {
                    //Configuration is empty
                    _configuration = breakAwayConfiguration;
                    var channels = GetAllBreakAwayChannels(_configuration);
                    foreach (var ch in channels)
                    {
                        var chStatus = GetBreakAwayListStatus(ch.ServerName, ch.List);
                        if (OnBreakAwayListStatusChanged != null)
                        {
                            OnBreakAwayListStatusChanged(this, new BreakAwayListStatusChangedArgs(ch.ServerName, ch.List, chStatus));
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void PerformBreakAway(String server, Int32 txList, IEnumerable<EventDTO> baContent)
        {
            var baList = GetEqualBreakAwayListIndex(server, txList);
            if (!BreakAwayImmediateEx(server, txList, baList, baContent))
            {
                throw new BreakAwayOperationFailedException();
            }
            ChangeBreakAwayState(server, txList, baList, true);
            var chStatus = GetBreakAwayListStatus(server, txList);
            if (OnBreakAwayListStatusChanged != null)
            {
                OnBreakAwayListStatusChanged(this, new BreakAwayListStatusChangedArgs(server, txList, chStatus));
            }   
        }

        public void PerformBreakAwayReturn(String server, Int32 txList)
        {
            var returnType = GetReturnType(server, txList);
            var baList = GetEqualBreakAwayListIndex(server, txList);
            switch (returnType)
            {
                case ReturnType.Jip:
                    if (!ReturnJip(server, txList,baList))
                        throw new BreakAwayOperationFailedException();
                    break;
                case ReturnType.Slide:
                    if (!ReturnSlide(server, txList, baList))
                        throw new BreakAwayOperationFailedException();
                    break;
                default:
                    throw new BreakAwayOperationFailedException();
            }
            ChangeBreakAwayState(server, txList, baList, false);
            var chStatus = GetBreakAwayListStatus(server, txList);
            if (OnBreakAwayListStatusChanged != null)
            {
                OnBreakAwayListStatusChanged(this, new BreakAwayListStatusChangedArgs(server, txList, chStatus));
            }
        }

        #endregion

        #region Private Methods

        private Boolean ReturnSlide(String server, Int32 txList, Int32 baList)
        {
            var chStatus = GetBreakAwayListStatus(server, txList);
            if (chStatus.InBreakAway)
            {
                var txContent = GetList(server, txList);
                var lastEv = txContent.LastOrDefault();
                var returnPos = lastEv != null ? lastEv.AdcEventId : Guid.Empty;
                var baContent = GetList(server, baList);
                if (!GetSaveAOAttribute(server, txList))
                {
                    var firstEv = baContent.FirstOrDefault(ev => ev.EventType == ADCEventType.Primary);
                    var pos = firstEv != null ? firstEv.AdcEventId : Guid.Empty;
                    ClearAOAttribute(server, baList, pos);
                }
                PerformBreakAwayFastRestore(server, txList, Guid.Empty, baList);
                AddCommentEvent(server, txList, CommentIds.Return, CommentTypes.FromBreakAway, returnPos);

                if (!DeleteAllEvents(server, baList))
                    return false;

                return true;
            }
            return false;
        }

        private bool ReturnJip(String server, Int32 txList, Int32 baList)
        {
            var breakAwayListStatusDTO = GetBreakAwayListStatus(server, txList);
            if (breakAwayListStatusDTO.InBreakAway)
            {
                TimeCode remainingDur;
                var txContent = GetList(server, txList);
                var baContent = GetList(server, baList);

                var lastPrimaryEvent = txContent.LastOrDefault(ev => ev.EventType == ADCEventType.Primary);
                var returnPosition = lastPrimaryEvent != null ? lastPrimaryEvent.AdcEventId : Guid.Empty;
                var brokenEventPos = FindBrokenEventPosition(server, txList, txContent, out remainingDur, true);
                if (brokenEventPos != Guid.Empty)
                {
                    if (!PerformListUnThread(server, baList))
                        return false;

                    var aEvent = GetEventByPosition(server, baList, brokenEventPos);
                    if (aEvent != null)
                    {
                        var srvTc = GetServerTimeCode(server, baList);
                        // save initial event duration
                        var tempDur = aEvent.Duration.ToTimeCode(srvTc.FrameRate, srvTc.DropFrame);
                        if (tempDur != remainingDur)
                        {
                            // set new event duration (remaining duration)
                            aEvent.Duration = new TimeCodeDTO(remainingDur);
                            
                            // calculate SOM difference
                            var eventSom = tempDur - remainingDur;
                            var origSom = aEvent.Som.ToTimeCode(srvTc.FrameRate, srvTc.DropFrame);
                            
                            if (origSom != TimeCode.Default)
                                eventSom += aEvent.Som.ToTimeCode(srvTc.FrameRate, srvTc.DropFrame);

                            aEvent.Som = new TimeCodeDTO(eventSom);

                            if (aEvent.EventControl.Contains(EventControlType.AutoTimed))
                            {
                                var types = aEvent.EventControl.ToList();
                                types.Remove(EventControlType.AutoTimed);
                                aEvent.EventControl = types;
                            }
                        }

                        if(!ModifyEvent(server, baList, aEvent, brokenEventPos))
                            return false;
                    }

                    if (brokenEventPos == Guid.Empty)
                    {
                       if(!GetSaveAOAttribute(server, txList))
                       {
                           var firstEv = baContent.FirstOrDefault(ev => ev.EventType == ADCEventType.Primary);
                           var pos = firstEv != null ? firstEv.AdcEventId : Guid.Empty;
                           ClearAOAttribute(server, baList, pos);
                       } 
                    }

                    PerformBreakAwayFastRestore(server, txList, brokenEventPos, baList);
                }

                AddCommentEvent(server, txList, CommentIds.Return, CommentTypes.Jip, returnPosition);
                if(!DeleteAllEvents(server, baList))
                    return false;

                return true;
            }
            return false;
        }

        private Boolean BreakAwayImmediateEx(String server, Int32 txList, Int32 baList, IEnumerable<EventDTO> baContent)
        {
            var brDuration = TimeCode.Empty;
            var brSom = TimeCode.Empty;
            TimeCode brokenEventDur;
            var res = false;
            var chStatus = GetBreakAwayListStatus(server, txList);
            var minFragment = GetMinFragmentForChannel(server, txList);

            var brokenEventPosition = GetOnAirEventPosition(server, txList, out brokenEventDur);
            if (brokenEventPosition != Guid.Empty)
            {
                chStatus.InBreakAway = true;
                var commandTime = GetServerTimeCode(server, txList);
                var baLatency = new TimeCode(commandTime.FrameRate, commandTime.DropFrame, _breakAwayLatency);
                var remainingDur = new TimeCode(commandTime.FrameRate, commandTime.DropFrame, brokenEventDur.Hour,
                                           brokenEventDur.Minute, brokenEventDur.Second, brokenEventDur.Frame); 

                var targetEv = GetEventByPosition(server, txList, brokenEventPosition);
                if(targetEv != null)
                {
                    var savedDuration = targetEv.Duration.ToTimeCode(commandTime.FrameRate, commandTime.DropFrame);
                    var baOnUpcount = targetEv.EventControl.Contains(EventControlType.AutoUpcount);
                    var divideRequired = false;
                    var bawayTime = TimeCode.Empty;
                    if (baOnUpcount)
                    {
                        bawayTime = commandTime + baLatency;
                        brDuration = savedDuration;
                        brSom = new TimeCode(commandTime.FrameRate, commandTime.DropFrame,0,0,0,0);
                        divideRequired = true;
                    }
                    else
                    {
                        if (remainingDur > baLatency + minFragment)
                        {
                            bawayTime = commandTime + baLatency;
                            brDuration = remainingDur - baLatency;
                            brSom = savedDuration + remainingDur + baLatency; 
                            divideRequired = true;
                        }
                        if (remainingDur == (baLatency + minFragment))
                        {
                            bawayTime = commandTime + baLatency;
                            brDuration = remainingDur - baLatency;
                            brSom = savedDuration - remainingDur + baLatency;
                            divideRequired = true;
                        }
                        if (remainingDur < baLatency + minFragment 
                            || remainingDur == baLatency)
                        {
                            bawayTime = commandTime + remainingDur;
                            divideRequired = false;
                        }
                        if (remainingDur < baLatency)
                        {
                            targetEv = GetNextPrimaryEvent(server, txList, brokenEventPosition);
                            if(targetEv!=null)
                            {
                                brokenEventDur = targetEv.Duration.ToTimeCode(commandTime.FrameRate, commandTime.DropFrame);
                            }
                            bawayTime = commandTime + baLatency;
                            var tempMoment = commandTime + remainingDur + minFragment; 
                            if(tempMoment>bawayTime)
                            {
                                //FHtemp1, FHtemp2 - durations for determination breakaway point.
                                var fHtemp1 = commandTime + remainingDur + minFragment; 
                                var fHtemp2 = commandTime + remainingDur + brokenEventDur; 
                                if (fHtemp1 < fHtemp2)
                                {
                                    bawayTime = tempMoment;
                                    brDuration = brokenEventDur + minFragment;
                                    brSom = minFragment;
                                    divideRequired = true;
                                }
                                else
                                {
                                    bawayTime = fHtemp2;
                                    divideRequired = false;
                                }
                            }
  
                        }
                    }

                    Guid nextPrimaryPosition;
                    if (divideRequired)
                    {
                        // if dividing current playing event
                        if (!VerifyAOAttribute(server, txList, baList, brokenEventPosition))
                            return false;
                        nextPrimaryPosition = GetNextPrimaryEvent(server, txList, brokenEventPosition).AdcEventId;
                        if (!PerformBreakAwayFastBackUp(server, txList, nextPrimaryPosition, baList))
                            return false;

                        // calculations
                        targetEv = GetEventByPosition(server, txList, brokenEventPosition);
                        if (targetEv != null)
                        {
                            targetEv.EventStatus.ToList().Clear();
                            targetEv.Duration = new TimeCodeDTO(brDuration);

                            // brSOM contains value to which the original event's SOM should be shifted.
                            // brSOM is not event's new SOM, it should be added to original SOM
                            var origSom = targetEv.Som.ToTimeCode(commandTime.FrameRate, commandTime.DropFrame);
                            if(origSom != TimeCode.Empty)
                            {
                                brSom += origSom;
                            }
                            targetEv.Som = new TimeCodeDTO(brSom);
                            if (!InsertEvent(server, baList, targetEv, InsertPlaceType.AtListBegin, Guid.Empty))
                                return false;
                        }
                        var previousPrimary = GetPreviousPrimaryEvent(server, txList, nextPrimaryPosition);
                        brokenEventPosition = previousPrimary != null ? previousPrimary.AdcEventId : nextPrimaryPosition;
                        if (!PerformListThread(server, baList))
                            return false;
                    }
                    else
                    {
                        // if we should start from next primary event
                        nextPrimaryPosition = GetNextPrimaryEvent(server, txList, brokenEventPosition).AdcEventId;
                        VerifyAOAttribute(server, txList, baList, nextPrimaryPosition);
                        if (!PerformBreakAwayFastBackUp(server, txList, nextPrimaryPosition, baList))
                            return false;
                        brokenEventPosition = GetPreviousPrimaryEvent(server, txList, nextPrimaryPosition).AdcEventId; 
                    }

                    //Insert the Ba content to the transmission list
                    var tmpBaList = baContent.ToList();
                    var insertedBreakAwayContent = InsertEventList(server, txList, InsertPlaceType.AtListEnd, tmpBaList,
                                                            Guid.Empty);
                    var first = insertedBreakAwayContent.FirstOrDefault();
                    if (first == null)
                        return false;
                    AppendAOAttribute(server, txList, bawayTime, first);

                    if (!GetSaveAOAttribute(server, txList))
                    {
                        // the first event in the ba list arrives with a delay (because of subscribe logic)
                        var attempts = 20;
                        while (attempts-- > 0)
                        {
                            Thread.Sleep(50);
                            var ev = _listService.GetList(_loginSession, server, baList).FirstOrDefault();
                            if (ev != null)
                            {
                                AppendAOAttribute(server, baList, bawayTime, ev);
                                break;
                            }
                        }
                    }

                    AddCommentEvent(server, txList, CommentIds.BreakAway, CommentTypes.Immediate, brokenEventPosition);
                        
                    if (!PerformListUnThread(server, baList))
                        return false;
                    if (!PerformListThread(server, baList))
                        return false;
                    res = true;
                }
            }

            return res;

        }

        // Return from the Break-Away content to this event.
        private Guid FindBrokenEventPosition(String server, Int32 list, IEnumerable<EventDTO> txList, out TimeCode remainingDur, Boolean verifyMinFrag)
        {
            var res = Guid.Empty;
            var serverTc = GetServerTimeCode(server, list);
            remainingDur = TimeCode.Empty;
            var txEndTime = TimeCode.Empty;
            TimeCode eventDur;
            TimeCode eventOnAir;
            
            // find time when BA sequence in txList ends
            var targetEv = txList.LastOrDefault(ev => ev.EventType == ADCEventType.Primary);
            if(targetEv != null)
            {
                eventOnAir = targetEv.OnAirTime.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                eventDur = targetEv.Duration.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                txEndTime = eventOnAir + eventDur;
            }

            var bAList = GetList(server, GetEqualBreakAwayListIndex(server, list));
            
            // find breakaway time
            var firstBreakAwayEvent = bAList.FirstOrDefault(ev => ev.EventType == ADCEventType.Primary);
            if (firstBreakAwayEvent == null)
                return Guid.Empty;
            var prevEventEnd = firstBreakAwayEvent.OnAirTime.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);


            // find broken event in BA list
            for (int i = 0; i < bAList.Count - 1; i++)
            {
                targetEv = bAList[i];

                if (bAList[i].EventStatus.Contains(EventRunStatus.Done) ||
                    targetEv.EventType == ADCEventType.Secondary) continue;
                
                eventOnAir = bAList[i].OnAirTime.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                eventDur = targetEv.Duration.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                var eventEnd = eventOnAir + eventDur;

                if (bAList[i].EventStatus.Contains(EventRunStatus.Running) &&
                    bAList[i].EventControl.Contains(EventControlType.AutoUpcount))
                {
                    // AU event is running in BA list - return from it
                    remainingDur = targetEv.Duration.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                    res = bAList[i].AdcEventId;
                }

                if (IsBetween(txEndTime, eventOnAir, eventEnd))
                {
                    // BA Sequence End Time is between Event Start Time and Event End Time
                    remainingDur = eventEnd - txEndTime;
                    res = bAList[i].AdcEventId;
                    if (verifyMinFrag && remainingDur < GetMinFragmentForChannel(server, list))
                    {
                        if (i + 1 < bAList.Count)
                        {
                            res = bAList[i + 1].AdcEventId;
                            targetEv = bAList[i + 1];
                            remainingDur = targetEv.Duration.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                        }
                        else
                        {
                            res = Guid.Empty;
                        }
                    }
                    break;
                }

                if (bAList[i].EventControl.Contains(EventControlType.AutoTimed))
                {
                    // operate with hard-starts
                    if (IsBetween(txEndTime, prevEventEnd, eventOnAir))
                    {
                        // end of BA sequence is between end of previous event in BA list
                        // and start of current event in BA list
                        // (in case if current event is AO and there is gap)
                        res = bAList[i + 1].AdcEventId;
                        remainingDur = targetEv.Duration.ToTimeCode(serverTc.FrameRate, serverTc.DropFrame);
                        break;
                    }
                }
                prevEventEnd = eventEnd;
            }

            return res;
        }

        private TimeCode GetMinFragmentForChannel(String server,Int32 list)
        {
            if (_configuration.GroupList != null)
            {
                foreach (var @group in _configuration.GroupList
                    .Where(@group => @group.Channels.Find(ch => ch.ServerName == server && ch.List == list) != null))
                {
                    var serverTc = GetServerTimeCode(server, list);
                    return group.MinFragment.ToTimeCode(serverTc.FrameRate,serverTc.DropFrame);
                }
            }
            return TimeCode.Empty;
        }

        private ReturnType GetReturnType(String server, Int32 list)
        {
            if (_configuration.GroupList != null)
            {
                foreach (var @group in _configuration.GroupList
                    .Where(@group => @group.Channels.Find(ch => ch.ServerName == server && ch.List == list) != null))
                {
                    if (group.Jip) return ReturnType.Jip;

                    return ReturnType.Slide;
                }
            }
            return ReturnType.Unspecified;
        }

        #region List Service API

        //The following API can be moved to List Helper as a part of the
        //separate BreakAway service

        private Boolean PerformBreakAwayFastBackUp(String server, Int32 txList, Guid position, Int32 baList)
        {
            try
            {
                _listService.PerformBreakAwayFastBackUp(server, baList, position, txList);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        public BreakAwayListStatusDTO GetBreakAwayListStatus(String server, Int32 list)
        {
            var res = new BreakAwayListStatusDTO();
            if (_configuration.GroupList != null)
            {
                foreach (var @group in _configuration.GroupList
                    .Where(@group => @group.Channels.Find(ch => ch.ServerName == server && ch.List == list) != null))
                {
                    res.DefaultPath = group.DefaultPath;
                    res.HasBreakAway = true;
                    res.SequencePath = group.SequencePath;
                    res.InBreakAway = _listService.GetBreakAwayState(server, list);
                    break;
                }
            }
            return res;
        }

        private  BreakAwayListStatusDTO GetBreakAwayListStatus(String server, Int32 list, BreakAwayConfigurationDTO configurationSource)
        {
            var res = new BreakAwayListStatusDTO();
            if (configurationSource.GroupList != null)
            {
                foreach (var @group in configurationSource.GroupList
                    .Where(@group => @group.Channels.Find(ch => ch.ServerName == server && ch.List == list) != null))
                {
                    res.DefaultPath = group.DefaultPath;
                    res.HasBreakAway = true;
                    res.SequencePath = group.SequencePath;
                    res.InBreakAway = _listService.GetBreakAwayState(server, list);
                    break;
                }
            }
            return res;
        }

        private void PerformBreakAwayFastRestore(String server, Int32 txList, Guid brokenEventPos, Int32 baList)
        {
            _listService.PerformBreakAwayFastRestore(server, baList, brokenEventPos, txList);
        }

        private List<EventDTO> GetList(String server, Int32 list)
        {
            var res = new List<EventDTO>();
            try
            {
                res = _listService.GetList(_loginSession, server, list).ToList();
            }
            catch (ListServiceEventProcessingException) { return res; }
            catch (ListServiceListNotEnabledException) { return res; }
            catch (ListServiceListLockedException) { return res; }
            catch (ADCEventException) { return res; }
            catch (ADCException) { return res; }
            return res;
        } 

        private Guid GetOnAirEventPosition(String server, Int32 list, out TimeCode duration)
        {
            //Go to the Break-Away content from this running event.
            return _listService.GetOnAirEventPosition(server, list, out duration);
        }

        private EventDTO GetEventByPosition(String server, Int32 list, Guid position)
        {
            var tmpList = _listService.GetList(_loginSession, server, list).ToList();
            return tmpList.FirstOrDefault(pair => pair.AdcEventId == position);
        }

        private EventDTO GetNextPrimaryEvent(String server, Int32 list, Guid position)
        {
            var tmpList = _listService.GetList(_loginSession, server, list).
                           Where(ev => ev.EventType == ADCEventType.Primary).ToList();
            if (tmpList.Count == 0)
                return null;
            var curEv = tmpList.FirstOrDefault(pair => pair.AdcEventId == position);
            var nextIdx = tmpList.Count > tmpList.IndexOf(curEv) + 1
                              ? tmpList.IndexOf(curEv) + 1
                              : tmpList.Count;
            return tmpList[nextIdx];
        }

        private EventDTO GetPreviousPrimaryEvent(String server, Int32 list, Guid position)
        {
            var tmpList = _listService.GetList(_loginSession, server, list).
                                       Where(ev => ev.EventType == ADCEventType.Primary).ToList();
            if (tmpList.Count == 0)
                return null;
            var curEv = tmpList.FirstOrDefault(pair => pair.AdcEventId == position);
            var nextIdx = tmpList.IndexOf(curEv) - 1 >= 0
                              ? tmpList.IndexOf(curEv) - 1
                              : 0;
            return tmpList[nextIdx];
        }

        private Boolean DeleteAllEvents(String server, Int32 list)
        {
            if (_listService == null) return false;
            try
            {
                _listService.LockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                _listService.DeleteAllEvents(_loginSession, server, list);
                _listService.UnlockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }

        }

        private TimeCode GetServerTimeCode(String server, Int32 list)
        {
            try
            {
                return _listService.GetServerTimeCode(server, list);
            }
            catch (ListServiceEventProcessingException) { return TimeCode.Empty; }
            catch (ListServiceListNotEnabledException) { return TimeCode.Empty; }
            catch (ListServiceListLockedException) { return TimeCode.Empty; }
            catch (ADCEventException) { return TimeCode.Empty; }
            catch (ADCException) { return TimeCode.Empty; }
        }

        private Boolean PerformListThread(String server, Int32 list)
        {
            if (_listService == null) return false;
            try
            {
                _listService.PerformListThread(_loginSession, server, list);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        private Boolean PerformListUnThread(String server, Int32 list)
        {
            if (_listService == null) return false;
            try
            {
                _listService.PerformListUnthread(_loginSession, server, list);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        private Boolean InsertEvent(String server, Int32 list, EventDTO ev, InsertPlaceType insertType, Guid position)
        {
            if (_listService == null) return false;
            try
            {
                _listService.LockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                _listService.InsertEvent(_loginSession, server, list, ev, insertType, position);
                _listService.UnlockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        private IEnumerable<EventDTO> InsertEventList(String server, Int32 list, InsertPlaceType insertType, IEnumerable<EventDTO> events, Guid position)
        {
            IEnumerable<EventDTO> result = null;
            if (_listService != null)
            {

                try
                {
                    _listService.LockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                    result = _listService.InsertEventList(_loginSession, server, list, events, insertType,
                                                                  position);
                    _listService.UnlockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                }
                catch (ListServiceEventProcessingException){}
                catch (ListServiceListNotEnabledException){}
                catch (ListServiceListLockedException){}
                catch (ADCEventException){}
                catch (ADCException){}
            }
            return result;
        }

        private Boolean ModifyEvent(String server, Int32 list,EventDTO ev,Guid position)
        {
            try
            {
                _listService.LockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                _listService.ModifyEvent(_loginSession, server, list, ev, position);
                _listService.UnlockList(_loginSession, server, list, CLIENT_LOCKER_NAME);
                return true;
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        private Int32 GetEqualBreakAwayListIndex(String server, Int32 txListIndex)
        {
            try
            {
                if (_listService != null)
                {
                    var listCount = _listService.GetListCount(server);
                    var baLists = new List<int>();
                    var txLists = new List<int>();
                    for (var i = 1; i <= listCount; i++)
                    {
                        var listType = _listService.GetListType(_loginSession, server, i);
                        if (listType == TypeOfList.Breakaway)
                        {
                            baLists.Add(i);
                        }
                        if (listType == TypeOfList.Sequence)
                        {
                            txLists.Add(i);
                        }
                    }
                    var search = txLists.IndexOf(txListIndex);
                    return search == -1 ? search : baLists[search];
                }
            }
            catch (ListServiceEventProcessingException) { return -1; }
            catch (ListServiceListNotEnabledException) { return -1; }
            catch (ListServiceListLockedException) { return -1; }
            catch (ADCException) { return -1; }
            return -1;
        }

        private Boolean VerifyAOAttribute(String server, Int32 txList, Int32 baList,Guid brokenEventPosition)
        { 
            try
            {
                var targetEv = GetEventByPosition(server, txList, brokenEventPosition);
                if (targetEv.EventControl.Contains(EventControlType.AutoTimed) &&
                      !targetEv.EventStatus.Contains(EventRunStatus.Running))
                {
                    _listService.SetAOAttributeForListPair(server, txList, baList, true);
                    return true;
                }
                _listService.SetAOAttributeForListPair(server, txList, baList, false);
                return true;  
            }
            catch (ListServiceEventProcessingException) { return false; }
            catch (ListServiceListNotEnabledException) { return false; }
            catch (ListServiceListLockedException) { return false; }
            catch (ADCEventException) { return false; }
            catch (ADCException) { return false; }
        }

        private Boolean GetSaveAOAttribute(String server, Int32 txList)
        {
           return  _listService.GetAOAttribute(server, txList);
        }

        public void ChangeBreakAwayState(String server, Int32 txList, Int32 baList, Boolean value)
        {
            _listService.ChangeBreakAwayState(server, txList, baList, value);
        }

        #endregion

        private void AddCommentEvent(string server, int list, string cID, string cType, Guid position)
        {
            var ev = new EventDTO(new SecAppFlag()) {ID = cID, Title = cType};
            InsertEvent(server, list, ev, InsertPlaceType.AfterGuid, position);
        }

        private void AppendAOAttribute(String server, Int32 list, TimeCode bawayTime, EventDTO targetEv)
        {
            if (targetEv != null)
            {
                targetEv.OnAirTime = new TimeCodeDTO(bawayTime);
                var eventControl = targetEv.EventControl.ToList();
                eventControl.Add(EventControlType.AutoTimed);
                targetEv.EventControl = eventControl;
                ModifyEvent(server, list, targetEv, targetEv.AdcEventId);
            }
        }

        private void ClearAOAttribute(String server, Int32 list, Guid position)
        {
            var targetEv = GetEventByPosition(server, list, position);
            if (targetEv != null)
            {
                var eventControl = targetEv.EventControl.ToList();
                eventControl.Remove(EventControlType.AutoTimed);
                targetEv.EventControl = eventControl;
                ModifyEvent(server, list, targetEv, position);
            }
        }

        // verifies if Requested time is between Time1 and Time 2
        private static Boolean IsBetween(TimeCode requestedTime, TimeCode startTime, TimeCode endTime)
        {
            if (startTime.ToMilliseconds() == endTime.ToMilliseconds())
            {
                if (startTime.ToMilliseconds() == requestedTime.ToMilliseconds())
                {
                    return true;
                }
            }
            else if (endTime.ToMilliseconds() > startTime.ToMilliseconds())
            {
                // if EndTime is more than StartTime, i.e. they are related to the same day 
                if (startTime.ToMilliseconds() <= requestedTime.ToMilliseconds() &&
                    requestedTime.ToMilliseconds() <= endTime.ToMilliseconds())
                {
                    // StartTime is not more than RequestedTime and 
                    // RequestedTime is not more than EndTime
                    return true;
                }
            }
            else // if EndTime is less than StartTime, i.e. EndTime is the next day
            {
                if (requestedTime.ToMilliseconds() > startTime.ToMilliseconds())
                {
                    // if RequestedTime is the same day as StartTime
                    if (requestedTime.ToMilliseconds() >= endTime.ToMilliseconds())
                    {
                        // if RequestedTime is not less than EndTime, which is next day
                        return true;
                    }
                }
                else
                {
                    // if RequestedTime is the next day after StartTime
                    if (requestedTime <= endTime)
                    {
                        // if RequestedTime is not more than EndTime, which is the same day
                        return true;
                    }

                }
            }
            return false;
        }

        private static IEnumerable<ChannelDescription> GetAllBreakAwayChannels(BreakAwayConfigurationDTO configuration)
        {
            var res = new List<ChannelDescription>();
            if (configuration.GroupList != null)
            {
                foreach (var group in configuration.GroupList)
                {
                    res.AddRange(group.Channels);
                }
            }
            return res;
        }
 
        private static IEnumerable<ChannelDescription> Compare(IEnumerable<ChannelDescription> one, IEnumerable<ChannelDescription> other)
        {
            return (from channel in one let target = other.FirstOrDefault(ch => ch.Equals(channel)) 
                    where target == null select channel).ToList();
        }

        private static TimeSpan GetBreakAwayLatencyValue()
        {
            TimeSpan chTime;
            return TimeSpan.TryParse(ConfigurationManager.AppSettings["ChannelAvailabilityTimeOut"], out chTime) 
                    ? chTime : new TimeSpan(0,0,10);
        }

        #endregion
    }

}
