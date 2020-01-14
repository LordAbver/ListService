using System;
using System.Collections.Generic;
using System.ServiceModel;
using Harris.Automation.ADC.Services.Common;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Events;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Timecode;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;
using Harris.Automation.ADC.Types;
using Harris.Automation.ADC.Types.Events;
using Harris.Automation.ADC.Types.ServiceErrors;


namespace Harris.Automation.ADC.Services.ListService
{
    #region Description and comments
    /// <summary>
    /// The interface of List Service.
    /// <para>
    /// Use the Add Service Reference dialog box to get the List service reference to the current solution, locally, on a local area network, or on the Internet.
    /// </para>
    /// <example>
    /// This example shows how to create a simple console client. But first you have to create a callback class based on a callback interface of service references ( see <see cref="IListServiceClient"/>). 
    /// <code>
    /// using Client.ListServiceReference; //include your namespace used for the reference
    /// using System.ServiceModel;
    /// 
    /// namespace Client
    /// {
    ///     class ListClient
    ///     {
    ///     static void Main(string[] args)
    ///        {
    ///            // This is a callback class based on a callback interface
    ///            ListServiceCallback clListCB = new ListServiceCallback();
    ///            
    ///            // This is client proxy
    ///            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
    ///                       
    ///            try
    ///            {
    ///                 // Do something here    
    ///            }
    ///            catch (Exception ex)
    ///            {
    ///                Console.WriteLine("Error: {0}, StackTrace: {1}",ex.Message,ex.StackTrace);                    
    ///            }
    ///        }   
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// 
    #endregion
    [ServiceContract(CallbackContract = typeof (IListServiceClient))]
    public interface IListService: IHeartbeatableService
    {
        #region Description and comments
        /// <summary>
        /// Gets events count in a specified list.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>            
        /// <returns>Returns events count in the list </returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <code language="cs">
        /// static void Main(string[] args)
        ///{
        ///    ListServiceCallback clListCB = new ListServiceCallback();
        ///    ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///    
        ///    LoginSession _ls = new LoginSession();
        ///    String _server = "MAIN";
        ///    String ClientName = "Client_ADC";
        ///    int _list = 2;
        ///
        ///    try
        ///    {
        ///        // Locking list
        ///        if (clList.LockList(_ls, _server, _list, ClientName))
        ///        {
        ///
        ///            // Get count of events before adding of new events
        ///            int result = clList.GetEventsCount(_ls, _server, _list);
        ///            Console.WriteLine("The number of events in the list {0} on the server {1} before adding of new events is {2}", _list.ToString(), _server, result.ToString());
        ///
        ///            EventDTO _event;
        ///
        ///            for (int i = 0; i &lt; 5; i++)
        ///            {
        ///                _event = new EventDTO() { ID = i.ToString() };
        ///                clList.InsertEvent(_ls, _server, _list, _event, Guid.Empty);
        ///
        ///                // Wait for adding current event
        ///                Thread.Sleep(100);
        ///            }
        ///
        ///            // Get count of events after adding of new events
        ///            result = clList.GetEventsCount(_ls, _server, _list);
        ///            Console.WriteLine("The number of events in the list {0} on the server {1} after adding of new events is {2}", _list.ToString(), _server, result.ToString());
        ///
        ///            // Unlocking list
        ///            clList.UnlockList(_ls, _server, _list, ClientName);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("List {0} was not locked", _list);
        ///        }
        ///
        ///    }
        ///    catch (Exception ex)                
        ///    {
        ///        Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
        ///    }                
        ///}
        /// </code>
        /// Console result
        /// <code language="Console">
        /// The number of events on list 2 on server MAIN before adding of new events is 0
        /// The number of events on list 2 on server MAIN after adding of new events is 5
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Int32 GetEventsCount(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets all events for a specified list.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns the list of events</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _server = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking list
                if (clList.LockList(_ls, _server, _list, ClientName))
                {

                    // Get the list of events before adding of new events
                    EventDTO[] result = clList.GetList(_ls, _server, _list);
                    Console.WriteLine("The number of events in the list {0} on the server {1} before adding of new events is {2}", _list.ToString(), _server, result.Length.ToString());

                    EventDTO _event;

                    for (int i = 0; i &lt; 5; i++)
                    {
                        _event = new EventDTO() { ID = "Event" + i.ToString() };
                        clList.InsertEvent(_ls, _server, _list, _event, Guid.Empty);

                        // Wait for adding current event
                        Thread.Sleep(100);
                    }

                    // Get the list of events after adding of new events
                    result = clList.GetList(_ls, _server, _list);
                    Console.WriteLine("The number of events in the list {0} on the server {1} after adding of new events is {2}", _list.ToString(), _server, result.Length.ToString());

                    foreach (EventDTO ev in result)
                    {
                        Console.WriteLine("ID = {0}", ev.ID);
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _server, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list);
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
          */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
The number of events on list 2 on server MAIN before adding of new events is 0
The number of events on list 2 on server MAIN after adding of new events is 5
ID = Event0
ID = Event1
ID = Event2
ID = Event3
ID = Event4
         * */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetList(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Subscribes to receive notifications of list changes. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>RegisterListListener returns true if the subscription is successful, otherwise it returns false. </returns>
        /// <example>
        /// At this example notifications of list changes will be received through ListLocked(),ListUnlocked(),OnEventsAdded() and OnListChange() callbacks ( see <see cref="IListServiceClient"/>).
        /// <code language="cs">
        /**
         static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _server = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
         
                // Subscribe to receive notifications of list changes      
                clList.RegisterListListener(_ls, _server, _list);

                // Locking list
                if (clList.LockList(_ls, _server, _list, ClientName))
                {
                    EventDTO _event;

                    for (int i = 0; i &lt; 5; i++)
                    {
                        _event = new EventDTO() { ID = "Event" + i.ToString() };
                        clList.InsertEvent(_ls, _server, _list, _event, Guid.Empty);

                        // Wait for adding the current event
                        Thread.Sleep(100);
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _server, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list);
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
OnEventsAdded notification: MAIN list 2 added event : ID Event0 Guid 24021c11-8c1a-42c6-ac77-e85c5967a0f1
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event1 Guid 8e978f06-7076-44b3-8888-6f7e66fe6b28
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event2 Guid 0116e84a-639c-4c73-bc81-e9c433f7f700
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event3 Guid 69bce99a-13af-4eee-b0d0-dc313beaad01
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event4 Guid a7dfda20-d8c7-4b0f-9004-07f96295f5d0
OnListChange notification: MAIN list 2 changeType ContentsChanged
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        Boolean RegisterListListener(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Stops receiving notifications of list changes for a specified list.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**         
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                        EventDTO _event;

                        for (int i = 0; i &lt; 5; i++)
                        {
                            _event = SimpleEvent("Event" + i.ToString());
                            clList.InsertEvent(_ls, _serverName, _list, _event, Guid.Empty);

                            // Wait for adding current event
                            Thread.Sleep(100);
                        }

                        // Stop receiving notifications of list changes 
                        clList.UnregisterListListener(_ls, _serverName, _list);

                        for (int i = 5; i &lt; 11; i++)
                        {
                            _event = SimpleEvent("Event" + i.ToString());
                            clList.InsertEvent(_ls, _serverName, _list, _event, Guid.Empty);

                            // Wait for adding the current event
                            Thread.Sleep(100);
                        }

                        // Get count of events after adding of new events
                        int result = clList.GetEventsCount(_ls, _serverName, _list);
                        Console.WriteLine("The number of events on the list {0} on the server {1} after adding of new events is {2}", _list.ToString(), _serverName, result.ToString());

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is unsuccessful", _list, _serverName);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }  
         }
         
         
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;            
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        }
        */
        /// </code>
        /// Console result. Only 5 notification was received.
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
OnEventsAdded notification: MAIN list 2 added event : ID Event0 Guid a7f8c87f-4e97-4bec-bca7-9201a2a0521c
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event1 Guid e875f0bb-0862-464a-aec3-01160d5825af
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event2 Guid 6bae8d7b-6a37-4536-8c29-f2cc0730ec66
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event3 Guid 7c144dfd-1f50-4a1d-8b13-59d5e8cad50e
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsAdded notification: MAIN list 2 added event : ID Event4 Guid 0668184b-bfad-47d5-9318-c30d1a1c48ba
OnListChange notification: MAIN list 2 changeType ContentsChanged
The number of events on list 2 on server MAIN after adding of new events is 10
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract(IsOneWay = true)]
        void UnregisterListListener(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Stops receiving all notifications of list changes for all lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <example>
        /// This method works like UnregisterListListener(), but stops receiving notifications for all lists ( see <see cref="UnregisterListListener"/>).
        /// </example>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void UnregisterAllListListeners(LoginSession session, String server);

        #region Description and comments
        /// <summary>
        /// Gets the first event on the list with missing media. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns first MissingMedia event</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the GetFirstMissingMediaEvent method.
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
         
               const string wrong_id = "!event";

                EventDTO[] ev_list = new EventDTO[4]
                {
                    SimpleEvent("Demo0001"),
                    SimpleEvent("Demo0002"),
                    SimpleEvent(wrong_id),
                    SimpleEvent("Demo0004"),
                };

                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    // Insert events
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for adding events
                    Thread.Sleep(1000);

                    // Get the first missing event
                    EventDTO _out = clList.GetFirstMissingMediaEvent(_ls, _serverName, _list);
                    Console.WriteLine("First missing media event is {0} ", _out.ID);

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;          
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
First missing media event is !event
ListUnlocked: MAIN list 2        
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        EventDTO GetFirstMissingMediaEvent(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets the event that is currently on air. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns OnAir event</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
         
               EventDTO[] ev_list = new EventDTO[4]
                {
                    SimpleEvent("Demo0001"),
                    SimpleEvent("Demo0002"),
                    SimpleEvent("Demo0003"),
                    SimpleEvent("Demo0004"),
                };

                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    // Insert events
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    //Threading list
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(100);

                    // Playing list
                    clList.PerformListPlay(_ls, _serverName, _list);

                    // Wait for starting playing
                    Thread.Sleep(5000);

                    // Get the OnAir event
                    EventDTO _out = clList.GetOnAirEvent(_ls, _serverName, _list);
                    Console.WriteLine("OnAir event is {0} ", _out.ID);

                    // Skip the current event
                    clList.PerformListSkip(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the OnAir event
                    _out = clList.GetOnAirEvent(_ls, _serverName, _list);
                    Console.WriteLine("OnAir event is {0} ", _out.ID);
                    
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
         
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;         
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
OnAir event is Demo0001
OnAir event is Demo0002
ListUnlocked: MAIN list 2        
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        EventDTO GetOnAirEvent(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets the name for a specified list number.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns the name of the list</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list = 2;

            try
            {
         
               // Get list count
                int cnt = clList.GetListCount(_serverName);

                for (int i = 1; i &lt;= cnt; i++)
                {
                    Console.WriteLine("Name of list number {0} is {1}", i.ToString(), clList.GetListName(_ls,_serverName,i));
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Name of list number 1 is PlayList 1
Name of list number 2 is PlayList 2
Name of list number 3 is PlayList 3
Name of list number 4 is PlayList 4
Name of list number 5 is PlayList 5
Name of list number 6 is PlayList 6
Name of list number 7 is PlayList 7
Name of list number 8 is PlayList 8
Name of list number 9 is PlayList 9
Name of list number 10 is PlayList 10
Name of list number 11 is PlayList 11
Name of list number 12 is PlayList 12
Name of list number 13 is PlayList 13
Name of list number 14 is PlayList 14
Name of list number 15 is PlayList 15
Name of list number 16 is PlayList 16
Name of list number 17 is MediaList 17
Name of list number 18 is MediaList 18
Name of list number 19 is MediaList 19
Name of list number 20 is MediaList 20
Name of list number 21 is GMTList 21
Name of list number 22 is GMTList 22
Name of list number 23 is CompileList 23
Name of list number 24 is CompileList 24
Name of list number 25 is PlayList 25
Name of list number 26 is PlayList 26
Name of list number 27 is PlayList 27
Name of list number 28 is PlayList 28
Name of list number 29 is PlayList 29
Name of list number 30 is PlayList 30
Name of list number 31 is PlayList 31
Name of list number 32 is PlayList 32
Name of list number 33 is PlayList 33
Name of list number 34 is PlayList 34
Name of list number 35 is PlayList 35
Name of list number 36 is PlayList 36
Name of list number 37 is PlayList 37
Name of list number 38 is PlayList 38
Name of list number 39 is PlayList 39
Name of list number 40 is PlayList 40
        
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        String GetListName(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets the type for the list.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns the TypeOfList value</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        TypeOfList GetListType(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets the lookahead range for a specified list. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns the Lookahead value for list</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list = 2;

            try
            {
         
               // Get list count
                int cnt = clList.GetListCount(_serverName);

                for (int i = 1; i &lt;= cnt; i++)
                {
                    Console.WriteLine("Name of list number {0} is {1}", i.ToString(), clList.GetLookahead(_ls,_serverName,i));
                }

            }
            catch (Exception ex)                
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
            }                
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Lookahead of list number 1 is 50
Lookahead of list number 2 is 50
Lookahead of list number 3 is 50
Lookahead of list number 4 is 50
Lookahead of list number 5 is 50
Lookahead of list number 6 is 50
Lookahead of list number 7 is 50
Lookahead of list number 8 is 50
Lookahead of list number 9 is 50
Lookahead of list number 10 is 50
Lookahead of list number 11 is 50
Lookahead of list number 12 is 50
Lookahead of list number 13 is 50
Lookahead of list number 14 is 50
Lookahead of list number 15 is 50
Lookahead of list number 16 is 50
Lookahead of list number 17 is 50
Lookahead of list number 18 is 50
Lookahead of list number 19 is 50
Lookahead of list number 20 is 50
Lookahead of list number 21 is 50
Lookahead of list number 22 is 50
Lookahead of list number 23 is 50
Lookahead of list number 24 is 50
Lookahead of list number 25 is 50
Lookahead of list number 26 is 50
Lookahead of list number 27 is 50
Lookahead of list number 28 is 50
Lookahead of list number 29 is 50
Lookahead of list number 30 is 50
Lookahead of list number 31 is 50
Lookahead of list number 32 is 50
Lookahead of list number 33 is 50
Lookahead of list number 34 is 50
Lookahead of list number 35 is 50
Lookahead of list number 36 is 50
Lookahead of list number 37 is 50
Lookahead of list number 38 is 50
Lookahead of list number 39 is 50
Lookahead of list number 40 is 50
        
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Int32 GetLookahead(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets the set of states the list is currently in (LISTISPLAYING, LISTTHREADING etc) 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns the array of list's states</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Get the list state
                printListState();

                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {

                    // Insert event
                    Console.WriteLine("Insert event in list number {0}", _list.ToString());
                    EventDTO ev_list = SimpleEvent("Demo0001");
                    clList.InsertEvent(_ls, _serverName, _list, ev_list, Guid.Empty);
                    Thread.Sleep(100);

                    //Threading list
                    Console.WriteLine("Threading list number {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(100);

                    // Get the list state
                    printListState();

                    // Playing list
                    Console.WriteLine("Playing list");
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(3000);

                    // Get the list state
                    printListState();
         
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);

                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
         
         
        // Call GetListState() method and write the result on the console
        private void printListState()
        {
            // Get the list state
            ListStateValue[] res = clList.GetListState(_ls, _serverName, _list);

            if (res.Length != 0)
            {
                Console.WriteLine("List number {0} contains the following states:", _list.ToString());
                for (int i = 0; i &lt; res.Length; i++)
                {
                    Console.WriteLine("- {0}", res[i].ToString());
                }
            }
            else
            {
                Console.WriteLine("List number {0} does not contain states:", _list.ToString());               
            }
        }
          
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
List number 2 does not contain states:
Insert event into list number 2
Threading list number 2
List number 2 contains the following states:
- Threading
Playing list
List number 2 contains the following states:
- Playing
- Threading

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<ListStateValue> GetListState(LoginSession session, String server, Int32 list);

        #region Description and comments

        /// <summary>
        /// Get information about Gap(gap event and last event before gap).
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>Returns two objects EventDTO(gap event and last event before gap)</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>

        #endregion
        [OperationContract]
        [FaultContract(typeof (ListServiceError))]
        [FaultContract(typeof (ListLockedError))]
        [FaultContract(typeof (DeviceServerNotRunningError))]
        [FaultContract(typeof (DeviceServerNotCreatedError))]
        EventProblemInformationDTO GetGapProblemInformation(LoginSession session, String server, int list);

        #region Description and comments
        /// <summary>
        /// Inserts an event to a specified list before a specified position (if Guid.Empty then add to the end). 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="ev">EventDTO object</param>
        /// <param name="place">The place of inserted event</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <returns>Returns the inserted event</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="UnregisterListListener"/> method
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        EventDTO InsertEvent(LoginSession session, String server, Int32 list, EventDTO ev, InsertPlaceType place, Guid position);

        #region Description and comments
        /// <summary>
        /// Inserts the event list to a specified list at a specified position (if Guid.Empty then add to the end). 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="events">The list of EventDTO objects</param>
        /// <param name="place">The place of insertion event</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <returns>Returns the list of inserted events</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ServiceParametersValidationError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="GetOnAirEvent"/> method
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        [FaultContract(typeof(ServiceParametersValidationError))]
        IEnumerable<EventDTO> InsertEventList(LoginSession session, String server, Int32 list, IEnumerable<EventDTO> events, InsertPlaceType place, Guid position);
        
        #region Description and comments
        /// <summary>
        /// Modifies an event on a specified list at a specified position. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="ev">EventDTO object</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <returns>Returns true if the event was modified otherwise false</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ServiceParametersValidationError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                        EventDTO ev = SimpleEvent("Demo0001");

                        // Insert events
                        Console.WriteLine("Insert events");
                        clList.InsertEvent(_ls, _serverName, _list, ev, Guid.Empty);
                        Thread.Sleep(100);

                        // Get inserted event Guid
                        Guid ev_guid;
                        try
                        {
                            ev_guid = clList.GetList(_ls, _serverName, _list).First(_ev => ev.ID == ev.ID).AdcEventId;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Retrieving event GUID failed : {0}", e.Message);
                            return;
                        }

                        clListCB.UpdatedEventList.Clear();

                        // Modify event
                        Console.WriteLine("Modify events");
                        ev.ID = "Demo0004";
                        ev.Som = new TimeCodeDTO();
                        clList.ModifyEvent(_ls, _serverName, _list, ev, ev_guid);
                        Thread.Sleep(1000);

                        // Compear callbacks

                        if (clListCB.UpdatedEventList.Exists(_ev => _ev.EventObject.ID == ev.ID &amp;&amp; _ev.EventObject.Som == ev.Som))
                        {
                            Console.WriteLine("OnEventsUpdated callback is correct for {0} event", ev.ID);
                        }
                        else
                        {
                            Console.WriteLine("OnEventsUpdated callback for {0} event does not exist", ev.ID);
                        }
                                            

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
Insert events
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid bf10195e-3186-4ea0-b385-e352e57b319e
OnListChange notification: MAIN list 2 changeType ContentsChanged
Modify events
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0004 Guid bf10195e-3186-4ea0-b385-e352e57b319e
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated callback is correct for Demo0004 event
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        [FaultContract(typeof(ServiceParametersValidationError))]
        Boolean ModifyEvent(LoginSession session, String server, Int32 list, EventDTO ev, Guid position);

        #region Description and comments
        /// <summary>
        /// Modifies an events on a specified list at a specified positions.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="events">The list of EventDTO objects</param>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        [FaultContract(typeof(ServiceParametersValidationError))]
        Boolean ModifyEventList(LoginSession session, String server, Int32 list, IEnumerable<EventDTO> events);

        #region Description and comments
        /// <summary>
        /// Moves an event on a specified list at a specified position 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="eventId">Guid of moved event</param>
        /// <param name="destination">GUID event in a playlist after which you want to insert the selection event</param>
        /// <returns>Returns true if the event is moved otherwise false</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the MoveEvent method.
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String ClientName = "Client_ADC";
            String _serverName = "MAIN";
            int _list = 2;
         
            try
            {
                // Subscribe to recive notifications about list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                        // Wait for receiving all callbacks
                        Thread.Sleep(1000);

                        // Get the list of inserted events
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        if (ev_list.Length > 0)
                        {
                            // Clear moved events list
                            clListCB.MovedEventList.Clear();

                            // Gets the first event guid 
                            Guid firstEventGuid = ev_list[0].AdcEventId;
                            // Get the fourth event guid
                            Guid lastEventGuid = ev_list[3].AdcEventId;

                            // Move the first event to the fourth event
                            Console.WriteLine("Move event with guid: {0}", firstEventGuid.ToString());
                            clList.MoveEvent(_ls, _serverName, _list, firstEventGuid, lastEventGuid);

                            // Wait for receiving callbacks
                            Thread.Sleep(1000);

                            // Compare GUIDs from callbacks
                            if (clListCB.MovedEventList.Exists(info => info.MovedEventsGuid.ToList().Exists(guid => guid == firstEventGuid)))
                            {
                                Console.WriteLine("OnEventsMoved callback is correct for event with guid {0}", firstEventGuid.ToString());
                            }
                            else
                            {
                                Console.WriteLine("OnEventsMoved callback for event with guid {0} does not exist", firstEventGuid.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: The events are not inserted");
                        }

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
         }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
        ListListener subscription for list number 2 on MAIN server is successful
        ListLocked notification: List 2 on server MAIN was locked
        Insert events list
        OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid 7e00c630-d9bc-4e53-bf6e-6f791dec36e8
        OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid 579d206e-9178-4a6c-a666-b1f9cc6aa3a5
        OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid 9f18f39a-b634-4af9-add2-16a466f3679a
        OnEventsAdded notification: MAIN list 2 added event : ID Demo0004 Guid b2d24632-4ac8-4ae3-b7c3-232a1b929eae
        OnListChange notification: MAIN list 2 changeType ContentsChanged
        Move event with guid: 7e00c630-d9bc-4e53-bf6e-6f791dec36e8
        OnEventsMoved notification: The 1 events were moved : from 00000000-0000-0000-0000-000000000000 to b2d24632-4ac8-4ae3-b7c3-23
        2a1b929eae guid, on 'MAIN' Server and '2' list.
        OnListChange notification: MAIN list 2 changeType ContentsChanged
        OnEventsMoved callback is correct for event with guid 7e00c630-d9bc-4e53-bf6e-6f791dec36e8
        ListUnlocked notification: List 2 on server MAIN was unlocked*/
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean MoveEvent(LoginSession session, String server, Int32 list, Guid eventId, Guid destination);

        #region Description and comments
        /// <summary>
        /// Moves the events collection to a specified list at a specified position(if Guid.Empty then move to the begin of the list)
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="events">Events collection for moving </param>
        /// <param name="destination">GUID event in a playlist after which you want to insert the events collection</param>
        /// <returns>Returns true if the list of events are moved, otherwise false</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ServiceParametersValidationError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        ///  The following example demonstrates how you can use the MoveEvents method.
        ///  <code language="cs">
        /**
         static void Main(string[] args)
         {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAINES";
            String ClientName = "Client_ADC";// The Client name to lock and unlock list
            int _list = 2;
            try
            {
                // Subscribe to recive notifications about list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                        Guid[] guid_move = new Guid[2];

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                        // Wait for receiving all callbacks
                        Thread.Sleep(1000);

                        // Get the list of inserted events
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        if (ev_list.Length > 0)
                        {
                            for (int i = 0; i &lt; 2; i++)
                            {
                                guid_move[i] = ev_list[i].AdcEventId;
                            }
                            // Clear moved events list
                            clListCB.MovedEventList.Clear();

                            // Gets the last event guid 
                            Guid destionationGuid = ev_list[3].AdcEventId;

                            // Move the first event to the fourth event
                            bool result = clList.MoveEvents(_ls, _serverName, _list, guid_move, destionationGuid);
                            // Wait for receiving callbacks
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.WriteLine("Error: The events are not inserted");
                        }
                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        */
        ///  </code>
        ///  Console result
        ///  <code language="Console">
        /**
ListListener subscription for list number 2 on MAINES server is successful
ListLocked notification: The list 2 on the server MAINES was locked by the Client_ADC client.
Insert events list
OnEventsAdded notification: MAINES list 2 added event : ID Demo0001 Guid 8864d328-0233-458f-a873-44fa54e0ccc3
OnEventsAdded notification: MAINES list 2 added event : ID Demo0002 Guid 9f6efabd-ac6c-4fcc-89d2-149df8a12806
OnEventsAdded notification: MAINES list 2 added event : ID Demo0003 Guid 38d61471-5859-475a-b3e0-c5c0bff3481f
OnEventsAdded notification: MAINES list 2 added event : ID Demo0004 Guid 385418ec-cfa9-4769-b0db-48ea1c11d31a
OnListChange notification: MAINES list 2 changeType ContentsChanged
OnEventsMoved notification: The 2 events were moved : from 00000000-0000-0000-0000-000000000000 to 385418ec-cfa9-4769-b0db-48
ea1c11d31a guid, on 'MAINES' Server and '2' list.
OnListChange notification: MAINES list 2 changeType ContentsChanged
         */
        ///  </code>
        ///  </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        [FaultContract(typeof(ServiceParametersValidationError))]
        Boolean MoveEvents(LoginSession session, String server, Int32 list, IEnumerable<Guid> events, Guid destination);

        #region Description and comments
        /// <summary>
        /// Deletes an event from a specified list at a specified position. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <returns>Returns true if the event is deleted otherwise false</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the DeleteEvent method. 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";// The Client name to lock and unlock list
            int _list = 2;

            try
            {
               // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                        EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                        // Wait for receiving all callbacks
                        Thread.Sleep(1000);

                        // Gets the inserted events list
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        if (ev_list.Length > 0)
                        {
                            // Clear the deleted event list
                            clListCB.DeletedEventList.Clear();

                            // Gets first event guid 
                            Guid guid_deleted = ev_list[0].AdcEventId;

                            // Delete an event
                            Console.WriteLine("Delete event with guid {0}", guid_deleted.ToString());
                            clList.DeleteEvent(_ls, _serverName, _list, guid_deleted);

                            // Wait for receiving callbacks
                            Thread.Sleep(1000);

                            // Compare callbacks
                            if (clListCB.DeletedEventList.Exists(info => info.DeletedEvents.ToList().Exists(ev => ev.AdcEventId == guid_deleted)))
                            {
                                Console.WriteLine("OnEventsDeleted callback is correct for event with guid {0}", guid_deleted.ToString());
                            }
                            else
                            {
                                Console.WriteLine("OnEventsDeleted callback for event with guid {0} does not exist", guid_deleted.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: The events are not inserted");
                        }

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
                    
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription for list number 2 on MAIN server is successful
ListLocked: MAIN list 2
Insert events list
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid 0cbe57ea-70c2-4836-ab1c-352526808793
OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid 42afeb71-98b1-458e-ae31-31be7d052119
OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid 18a9eb6a-7440-44a0-af47-3be5a6312d0b
OnEventsAdded notification: MAIN list 2 added event : ID Demo0004 Guid def30dc4-e65f-496c-a366-463a4fc9a999
OnListChange notification: MAIN list 2 changeType ContentsChanged
Delete event with guid 0cbe57ea-70c2-4836-ab1c-352526808793
OnEventsDeleted notification: MAIN list 2 deleted event : Guid 0cbe57ea-70c2-4836-ab1c-352526808793
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsDeleted callback is correct for the event with guid 0cbe57ea-70c2-4836-ab1c-352526808793
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean DeleteEvent(LoginSession session, String server, Int32 list, Guid position);
        
        #region Description and comments
        /// <summary>
        /// Deletes events list from specified list at specified position. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">GUID initial position to delete from list</param>
        /// <param name="numberOfEvents">Number of deleted events</param>
        /// <returns>Returns true if events are deleted, otherwise false</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the DeleteEvents method.
        ///  <code language="cs">
        /**
         static void Main(string[] args)
         {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";// The Client name to lock and unlock list
            int _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);
                    TimeCodeDTO holdtime = new TimeCodeDTO();

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);
                    var evTodel = clList.GetList(_ls, _serverName, _list);
                    if (evTodel != null)
                    {
                        if (clList.DeleteEvents(_ls, _serverName, _list, evTodel.ToList().First().AdcEventId,
                                                evTodel.Count()))
                        {
                            Console.WriteLine("The {0} events are deleted", evTodel.Count());
                        }
                        else
                        {
                            Console.WriteLine("The events are not deleted");
                        }
                    }
                    Thread.Sleep(2000);
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
         }
         */
        ///  </code>
        ///  Console result
        ///  <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
The 3 events are deleted
ListUnlocked: MAIN list 2
         */
        ///  </code>
        ///  </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean DeleteEvents(LoginSession session, String server, Int32 list, Guid position, Int32 numberOfEvents);

        #region Description and comments
        /// <summary>
        /// Deletes events from specified list at specified positions. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="positions">Collection of events GUIDs in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the DeleteEventsByGuid method.
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
         
             try
            {
                clList.RegisterListListener(_ls, _serverName, _list);
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);
                    var evToDel = clList.GetList(_ls, _serverName, _list);

                    if (evToDel != null)
                    {
                        Guid[] guids = new Guid[evToDel.Count()];
                        for (int i = 0; i &lt; evToDel.Count(); i++)
                        {
                            guids[i] = evToDel[i].AdcEventId;
                        }
                        // Delete specified events
                        clList.DeleteEventsByGuid(_ls, _serverName, _list, guids);
                    }
                    Thread.Sleep(3000);
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }

                clList.UnregisterListListener(_ls, _serverName, _list);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /**
         ListLocked notification: The list 2 on the server MAINES was locked by the Client_ADC client.
         Insert events list
         OnEventsAdded notification: MAINES list 2 added event : ID Demo0001 Guid 35883415-96a4-40e6-9926-ccb943606b69
         OnEventsAdded notification: MAINES list 2 added event : ID Demo0002 Guid a3781c88-55d1-4407-a4c8-2c77874bd989
         OnEventsAdded notification: MAINES list 2 added event : ID Demo0003 Guid 76eba05a-f09c-4927-9e0e-101c1f498071
         OnListChange notification: MAINES list 2 changeType ContentsChanged
         OnEventsDeleted notification: MAINES list 2 deleted event : Guid ADCServicesExamples.ListServiceReference.EventDTO
         OnEventsDeleted notification: MAINES list 2 deleted event : Guid ADCServicesExamples.ListServiceReference.EventDTO
         OnEventsDeleted notification: MAINES list 2 deleted event : Guid ADCServicesExamples.ListServiceReference.EventDTO
         OnListChange notification: MAINES list 2 changeType ContentsChanged
        */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof (ListServiceError))]
        [FaultContract(typeof (ListLockedError))]
        [FaultContract(typeof (DeviceServerNotRunningError))]
        [FaultContract(typeof (DeviceServerNotCreatedError))]
        void DeleteEventsByGuid(LoginSession session, String server, Int32 list, IEnumerable<Guid> positions);

        #region Description and comments
        /// <summary>
        /// Deletes all events from a specified list. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                        EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                        // Wait for receiving all callbacks
                        Thread.Sleep(1000);

                        // Gets the inserted events list
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        if (ev_list.Length > 0)
                        {
                            // Clear the deleted event list
                            clListCB.DeletedEventList.Clear();

                            // Delete an event
                            Console.WriteLine("Delete all events");
                            clList.DeleteAllEvents(_ls, _serverName, _list);

                            // Wait for receiving callbacks
                            Thread.Sleep(1000);

                            // Compare callbacks
                            ev_list.ToList().ForEach(ev =>
                                {
                                    if (clListCB.DeletedEventList.Exists(_ev => _ev.guid == ev.AdcEventId))
                                    {
                                        Console.WriteLine("OnEventsDeleted callback is correct for the event with guid {0}", ev.AdcEventId.ToString());
                                    }
                                    else
                                    {
                                        Console.WriteLine("OnEventsDeleted callback for the event with guid {0} does not exist", ev.AdcEventId.ToString());
                                    }
                                });

                            // Get count of events
                            int result = clList.GetEventsCount(_ls, _serverName, _list);
                            Console.WriteLine("The number of events on list {0} on server {1} is {2}", _list.ToString(), _serverName, result.ToString());


                        }
                        else
                        {
                            Console.WriteLine("Error: The events are not inserted");
                        }

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        { 
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
Insert events list
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid 9ecac62a-fab7-4acd-bd78-5f164ba98fcc
OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid debed61a-f8f1-4d43-89b8-693108c32d28
OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid 5f82c3ef-65c6-47da-b19b-e141169a4a8e
OnEventsAdded notification: MAIN list 2 added event : ID Demo0004 Guid 8e3754b7-2d03-4657-b470-dee1f680ec6c
OnListChange notification: MAIN list 2 changeType ContentsChanged
Delete all events
OnEventsDeleted notification: MAIN list 2 deleted event : Guid 9ecac62a-fab7-4acd-bd78-5f164ba98fcc
OnEventsDeleted notification: MAIN list 2 deleted event : Guid debed61a-f8f1-4d43-89b8-693108c32d28
OnEventsDeleted notification: MAIN list 2 deleted event : Guid 5f82c3ef-65c6-47da-b19b-e141169a4a8e
OnEventsDeleted notification: MAIN list 2 deleted event : Guid 8e3754b7-2d03-4657-b470-dee1f680ec6c
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsDeleted callback is correct for the event with guid 9ecac62a-fab7-4acd-bd78-5f164ba98fcc
OnEventsDeleted callback is correct for the event with guid debed61a-f8f1-4d43-89b8-693108c32d28
OnEventsDeleted callback is correct for the event with guid 5f82c3ef-65c6-47da-b19b-e141169a4a8e
OnEventsDeleted callback is correct for the event with guid 8e3754b7-2d03-4657-b470-dee1f680ec6c
The number of events on list 2 on server MAIN is 0
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void DeleteAllEvents(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Holds the countdown of events which are playing for the specified lists collection. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened and be assigned 
        /// on the first and second play lists.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2, _list1 = 1;
         
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp; clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] lists = new int[] { _list, _list1 };

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Insert events to the list {0}", list);
                        clList.InsertEventList(_ls, _serverName, list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }
                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread list 
                    Console.WriteLine("Thread the lists.");
                    clList.GangThread(_ls, _serverName, lists);
                    Thread.Sleep(2000);

                    // Play lists
                    Console.WriteLine("Play the lists.");
                    clList.GangPlay(_ls, _serverName, lists);
                    Thread.Sleep(4000);

                    // Skip lists
                    Console.WriteLine("Skip the lists.");
                    clList.GangSkip(_ls, _serverName, lists);
                    Thread.Sleep(3000);

                    // Hold events for lists 
                    Console.WriteLine("Hold the lists");
                    clList.GangHold(_ls, _serverName, lists);
                    Thread.Sleep(1000);

                    foreach (var list in lists)
                    {
                        // Get list state and check that Hold state is exist
                        EventDTO[] listEvents = clList.GetList(_ls, _serverName, list);
                        Console.WriteLine("Get events states for the list {0}", list);
                        foreach (var stateEvent in listEvents)
                        {
                            foreach (EventRunStatus status in stateEvent.EventStatus)
                            {
                                Console.WriteLine("Event {0} has status {1} ", stateEvent.ID, status);
                            }
                        }
                    }
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    clList.UnlockList(_ls, _serverName, _list1, ClientName);
                }
                else
                {
                    Console.WriteLine("Lists were not locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        { 
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
*/
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
        Insert events to the list 2
        Insert events to the list 1
        Thread the lists.
        Play the lists.
        Skip the lists.
        Hold the lists
        Get events states for the list 2
        Event Demo0001 has status Done
        Event Demo0001 has status Skipped
        Event Demo0002 has status Running
        Event Demo0002 has status StandbyOn
        Event Demo0003 has status Prerolled
        Event Demo0003 has status StandbyOn
        Get events states for the list 1
        Event Demo0001 has status Done
        Event Demo0001 has status Skipped
        Event Demo0002 has status Running
        Event Demo0002 has status StandbyOn
        Event Demo0003 has status Prerolled
        Event Demo0003 has status StandbyOn
        */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangHold(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Toggles the signal switching paths between the on-air and protect devices for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Four Demo Video Disks should have the Output Port opened. 
        /// The first and third Disks should be assigned to the first and second lists as the Air devices. 
        /// The second and fourth Disks should be assigned as the Protect. All the Air and Protect devices must 
        /// have exactly the same media available in storage.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2, _list1 = 1;
         
            try
            {
               // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp;
                    clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] lists = new int[] { _list, _list1 };
                    EventDTO[] eventsList;

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Insert events to the list {0}.", list);
                        clList.InsertEventList(_ls, _serverName, list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }
                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread list 
                    Console.WriteLine("Thread the lists.");
                    clList.GangThread(_ls, _serverName, lists);

                    foreach (var list in lists)
                    {
                        eventsList = clList.GetList(_ls, _serverName, list);
                        foreach (var eventDto in eventsList)
                        {
                            Console.WriteLine("Air device channel: {0} for event: {1} and the list {2}.", eventDto.DeviceIndex, eventDto.ID, list);
                        }
                    }
                    Thread.Sleep(2000);

                    // Play lists
                    Console.WriteLine("Play the lists.");
                    clList.GangPlay(_ls, _serverName, lists);
                    Thread.Sleep(3000);

                    // Toggle the signal
                    Console.WriteLine("Toggle the signal between the on-air and protect devices.");
                    clList.GangAirProtect(_ls, _serverName, lists);
                    Thread.Sleep(4000);

                    foreach (var list in lists)
                    {
                        eventsList = clList.GetList(_ls, _serverName, list);
                        foreach (var eventDto in eventsList)
                        {
                            Console.WriteLine("Protect device channel: {0} for event: {1} and the list {2}.", eventDto.DeviceIndex,
                                              eventDto.ID, list);
                        }
                    }
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    clList.UnlockList(_ls, _serverName, _list1, ClientName);
                }
                else
                {
                    Console.WriteLine("Lists were not locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
           TimeCodeDTO tcDTO = new TimeCodeDTO();
           tcDTO.Second = 10;         
           return new EventDTO
           {
               AdcEventId = Guid.NewGuid(),
               Duration = tcDTO,
               DeviceIndex = 4,
               EventType = ADCEventType.Primary,
               EventControl = new EventControlType[3] 
               { 
                   EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
               },
               EventStatus = new EventRunStatus[] {},
               ID = newID,
               OnAirTime = tcDTO,
               SegmentNumber = 255,
               Size = 350,
               Som = tcDTO,
               Title = "",
               TransitionEffect = Transition.Cut,
               TransitionEffectRate = SwitchRate.Slow,
           };
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Insert events to the list 2.
Insert events to the list 1.
Thread the lists.
Air device channel: 3 for event: Demo0001 and the list 2.
Air device channel: 3 for event: Demo0002 and the list 2.
Air device channel: 3 for event: Demo0003 and the list 2.
Air device channel: 1 for event: Demo0001 and the list 1.
Air device channel: 1 for event: Demo0002 and the list 1.
Air device channel: 1 for event: Demo0003 and the list 1.
Play the lists.
Toggle the signal between the on-air and protect devices.
Protect device channel: 4 for event: Demo0001 and the list 2.
Protect device channel: 4 for event: Demo0002 and the list 2.
Protect device channel: 4 for event: Demo0003 and the list 2.
Protect device channel: 2 for event: Demo0001 and the list 1.
Protect device channel: 2 for event: Demo0002 and the list 1.
Protect device channel: 2 for event: Demo0003 and the list 1.
*/
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangAirProtect(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Places a cued event’s VTR into tension release for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <example>
        /// See <see cref="GangReady"/> for an example of how to use <c>GangTensionRelease</c> method.
        /// </example>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangTensionRelease(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        ///  Stops a running transmission list, including the on-air event and any events that follow for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>See <see cref="GangPlay"/> for an example of how to use <see cref="GangUnthread"/> method.</example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangUnthread(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Adds one second to the playing event’s duration for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened and be assigned on the first 
        /// and second play lists.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list1 = 1, _list = 2;
         
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp; 
                    clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] lists = new int[] { _list, _list1 };

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                          };

                    // Insert events
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Insert events to the list {0}", list);
                        clList.InsertEventList(_ls, _serverName, list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }
                    // Wait for inserting events
                    Thread.Sleep(500);
                    Console.WriteLine("Thread the lists");
                    clList.GangThread(_ls, _serverName, lists);
                    Thread.Sleep(2000);

                    Console.WriteLine("Recue the lists");
                    clList.GangRecue(_ls, _serverName, lists);
                    Thread.Sleep(2000);

                    Console.WriteLine("Play the lists");
                    clList.GangPlay(_ls, _serverName, lists);
                    Thread.Sleep(3000);

                    Console.WriteLine("Adds one second to the playing event’s duration");
                    clList.GangPlus1Sec(_ls, _serverName, lists);
                    Console.WriteLine("Subtracts one second from the playing event’s duration.");
                    clList.GangMinus1Sec(_ls, _serverName, lists);

                    foreach (var list in lists)
                    {
                        // Get list state and check that Hold state is exist
                        EventDTO[] listEvents = clList.GetList(_ls, _serverName, list);
                        Console.WriteLine("Get events states for the list {0}", list);
                        foreach (var stateEvent in listEvents)
                        {
                            foreach (EventRunStatus status in stateEvent.EventStatus)
                            {
                                Console.WriteLine("Event {0} has status {1} ", stateEvent.ID, status);
                            }
                        }
                    }

                    // Unlocking lists
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    clList.UnlockList(_ls, _serverName, _list1, ClientName);
                }
                else
                {
                    Console.WriteLine("Lists were not locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
           TimeCodeDTO tcDTO = new TimeCodeDTO();
           tcDTO.Second = 10;         
           return new EventDTO
           {
               AdcEventId = Guid.NewGuid(),
               Duration = tcDTO,
               DeviceIndex = 4,
               EventType = ADCEventType.Primary,
               EventControl = new EventControlType[3] 
               { 
                   EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
               },
               EventStatus = new EventRunStatus[] {},
               ID = newID,
               OnAirTime = tcDTO,
               SegmentNumber = 255,
               Size = 350,
               Som = tcDTO,
               Title = "",
               TransitionEffect = Transition.Cut,
               TransitionEffectRate = SwitchRate.Slow,
           };
        }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Insert events to the list 2
Insert events to the list 1
Thread the lists
Recue the lists
Play the lists
Adds one second to the playing event's duration
Subtracts one second from the playing event's duration.
Get events states for the list 2
Event Demo0001 has status Running
Event Demo0002 has status StandbyOn
Get events states for the list 1
Event Demo0001 has status Running
Event Demo0002 has status StandbyOn
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangPlus1Sec(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Performs Play Gang command for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangPlayGang(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Rolls bypasses normal preroll values and plays the next events as fast as the equipment will allow, for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened and be assigned on the 
        /// first and second play lists.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list1 = 1, _list = 2;
         
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp;
                    clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] lists = new int[] { _list, _list1 };

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                          };

                    // Insert events
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Insert events to the list {0}", list);
                        clList.InsertEventList(_ls, _serverName, list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }
                    // Wait for inserting events
                    Thread.Sleep(500);
                    Console.WriteLine("Thread the lists");
                    clList.GangThread(_ls, _serverName, lists);
                    Thread.Sleep(3000);

                    Console.WriteLine("Play the lists");
                    clList.GangPlay(_ls, _serverName, lists);
                    Thread.Sleep(6000);

                    Console.WriteLine("Rolls events");
                    clList.GangRoll(_ls, _serverName, lists);
                    
                    foreach (var list in lists)
                    {
                        // Get list state and check that Hold state is exist
                        EventDTO[] listEvents = clList.GetList(_ls, _serverName, list);
                        Console.WriteLine("Get events states for the list {0}", list);
                        foreach (var stateEvent in listEvents)
                        {
                            foreach (EventRunStatus status in stateEvent.EventStatus)
                            {
                                Console.WriteLine("Event {0} has status {1} ", stateEvent.ID, status);
                            }
                        }
                    }

                    // Unlocking lists
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    clList.UnlockList(_ls, _serverName, _list1, ClientName);
                }
                else
                {
                    Console.WriteLine("Lists were not locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
           TimeCodeDTO tcDTO = new TimeCodeDTO();
           tcDTO.Second = 10;         
           return new EventDTO
           {
               AdcEventId = Guid.NewGuid(),
               Duration = tcDTO,
               DeviceIndex = 4,
               EventType = ADCEventType.Primary,
               EventControl = new EventControlType[3] 
               { 
                   EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
               },
               EventStatus = new EventRunStatus[] {},
               ID = newID,
               OnAirTime = tcDTO,
               SegmentNumber = 255,
               Size = 350,
               Som = tcDTO,
               Title = "",
               TransitionEffect = Transition.Cut,
               TransitionEffectRate = SwitchRate.Slow,
           };
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Insert events to the list 2
Insert events to the list 1
Thread the lists
Play the lists
Rolls events
Get events states for the list 2
Event Demo0001 has status Running
Event Demo0001 has status Skipped
Event Demo0002 has status Prerolled
Event Demo0002 has status StandbyOn
Get events states for the list 1
Event Demo0001 has status Running
Event Demo0001 has status Skipped
Event Demo0002 has status Prerolled
Event Demo0002 has status StandbyOn
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangRoll(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Threads and cues the first events in a set of playlists and any events within the time range specified in the lookahead.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <example>See <see cref="GangPlay"/> for an example of how to use <see cref="GangThread"/> method.</example>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangThread(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Turns on tension to the next VTR event in tension release to prepare upcoming VTR events for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo VTRs should be assigned to the first and second list.
        /// Configure the following parameters of VTR as required: Check "Keep Media in VTR" option in configuration dialog for VTR( General tab).
        /// Select Diagnostics tab and enter VTR_eve to 'ID' field and press 'Inject' button.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list1 = 1, _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp; clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] PlayLists = {1, 2};

                    foreach (var playList in PlayLists)
                    {
                        // Insert event
                        Console.WriteLine("Insert the event to list {0}", playList);
                        clList.InsertEvent(_ls, _serverName, playList, SimpleEvent("VTR_eve"), InsertPlaceType.AtListEnd, Guid.Empty);
                    }

                    // Thread lists
                    clList.GangThread(_ls, _serverName, PlayLists);

                    Thread.Sleep(3000);
                    foreach (var playList in PlayLists)
                    {
                        EventDTO[] eventsBefore = clList.GetList(_ls, _serverName, playList);
                        foreach (var eventDto in eventsBefore)
                        {
                            Console.WriteLine("Play List: {0}, Event ID: {1}, event has state '{2}'", playList, eventDto.ID,
                                              eventDto.ExtendedStatus);
                        }
                    }
                    // Perform List Ready
                    clList.GangReady(_ls, _serverName, PlayLists);

                    Thread.Sleep(3000);
                    foreach (var playList in PlayLists)
                    {
                        EventDTO[] eventsBefore = clList.GetList(_ls, _serverName, playList);
                        foreach (var eventDto in eventsBefore)
                        {
                            Console.WriteLine("Play List: {0}, Event ID: {1}, event has state '{2}'", playList, eventDto.ID,
                                              eventDto.ExtendedStatus);
                        }
                    }

                    // Perform Tension Release command
                    clList.GangTensionRelease(_ls, _serverName, PlayLists);

                    Thread.Sleep(3000);
                    foreach (var playList in PlayLists)
                    {
                        EventDTO[] eventsBefore = clList.GetList(_ls, _serverName, playList);
                        foreach (var eventDto in eventsBefore)
                        {
                            Console.WriteLine("Play List: {0}, Event ID: {1}, event has state '{2}'", playList, eventDto.ID,
                                              eventDto.ExtendedStatus);
                        }
                    }
                    // Unlocking list
                    foreach (var playList in PlayLists)
                    {
                        clList.UnlockList(_ls, _serverName, playList, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, playList);
                    }
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /**
        Insert the event to list 1
        Insert the event to list 2
        Play List: 1, Event ID: VTR_eve, event has state 'TENREL'
        Play List: 2, Event ID: VTR_eve, event has state 'TENREL'
        Play List: 1, Event ID: VTR_eve, event has state 'READY'
        Play List: 2, Event ID: VTR_eve, event has state 'READY'
        Play List: 1, Event ID: VTR_eve, event has state 'TENREL'
        Play List: 2, Event ID: VTR_eve, event has state 'TENREL'
        ListUnlocked: MAINES list 1
        ListUnlocked: MAINES list 2 
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangReady(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Subtracts one second from the playing event’s duration for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>See <see cref="GangPlus1Sec"/> for an example of how to use <see cref="GangMinus1Sec"/> method.</example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangMinus1Sec(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Stops play and countdown for a set of lists.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>See <see cref="GangPlay"/> for an example of how to use <see cref="GangFreeze"/> method.</example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangFreeze(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Performs Play command for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened and be assigned 
        /// on the first and second play lists.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list1 = 1, _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp; clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] lists = new int[] { _list, _list1 };

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                          };

                    // Insert events
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Insert events to the list {0}", list);
                        clList.InsertEventList(_ls, _serverName, list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }
                    // Wait for inserting events
                    Thread.Sleep(500);
                    Console.WriteLine("Thread the lists");
                    clList.GangThread(_ls, _serverName, lists);
                    Thread.Sleep(2000);
         
                    Console.WriteLine("Play the lists");
                    clList.GangPlay(_ls, _serverName, lists);
                    Thread.Sleep(3000);
         
                    Console.WriteLine("Freeze the lists");
                    clList.GangFreeze(_ls, _serverName, lists);
                    Thread.Sleep(1000);
         
                    foreach (var list in lists)
                    {
                        Console.WriteLine("Get states for the list {0}", list);
                        ListStateValue[] listStates = clList.GetListState(_ls, _serverName, list);
                        foreach (var stateValue in listStates)
                        {
                            Console.WriteLine("State: {0} for the list {1}", stateValue, list);
                        }
                    }
                    // Unthread events for lists 
                    Console.WriteLine("Unthread the lists");
                    clList.GangUnthread(_ls, _serverName, lists);
                    Thread.Sleep(2000);
                    // Unlocking lists
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    clList.UnlockList(_ls, _serverName, _list1, ClientName);
                }
                else
                {
                    Console.WriteLine("Lists were not locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
           TimeCodeDTO tcDTO = new TimeCodeDTO();
           tcDTO.Second = 10;         
           return new EventDTO
           {
               AdcEventId = Guid.NewGuid(),
               Duration = tcDTO,
               DeviceIndex = 4,
               EventType = ADCEventType.Primary,
               EventControl = new EventControlType[3] 
               { 
                   EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
               },
               EventStatus = new EventRunStatus[] {},
               ID = newID,
               OnAirTime = tcDTO,
               SegmentNumber = 255,
               Size = 350,
               Som = tcDTO,
               Title = "",
               TransitionEffect = Transition.Cut,
               TransitionEffectRate = SwitchRate.Slow,
           };
         }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Insert events to the list 2
Insert events to the list 1
Thread the lists
Play the lists
Freeze the lists
Get states for the list 2
State: Playing for the list 2
State: Threading for the list 2
State: Freeze for the list 2
State: Upcounting for the list 2
Get states for the list 1
State: Playing for the list 1
State: Threading for the list 1
State: Freeze for the list 1
State: Upcounting for the list 1
Unthread the lists
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangPlay(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Recues an on-air events for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>See <see cref="GangPlus1Sec"/> for an example of how to use <see cref="GangRecue"/> method.</example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangRecue(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Skips the events currently playing and plays the next events for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>See <see cref="GangHold"/> for an example of how to use <see cref="GangSkip"/> method.</example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangSkip(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Perform Play Secondary command for a set of lists. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="lists">Collection of Play Lists numbers on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration:  Assign Two Switch Only Devices and two Demo Video Disks to the desired channels. Demo Video Disks should 
        /// have the Output Port opened. All parameters for Switch Only Devices should be by default(For details on assigning a device, see ADC ConfigTool Help)
        /// The first Disk and Switch Only Device should be assigned to the first list. 
        /// The second Disk and Switch Only Device should be assigned to the second list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list1 = 1, _list = 2;
            
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName) &amp;&amp;
                    clList.LockList(_ls, _serverName, _list1, ClientName))
                {
                    int[] PlayLists = { 1, 2 };
                    // The 'OnAirTime' time code should be empty for the secondary Audio/Video event.
                    TimeCodeDTO AirTimeEmpty = new TimeCodeDTO();
                    AirTimeEmpty.Hour = 0xFF;
                    AirTimeEmpty.Minute = 0xFF;
                    AirTimeEmpty.Second = 0xFF;
                    AirTimeEmpty.Frame = 0xFF;
                    _tcDTO.Minute = 1;

                    EventDTO[] ev_list = new EventDTO[2]
                    {
                         SimpleEvent("Demo0001"),
                         new EventDTO{
                             AdcEventId = Guid.NewGuid(),
                             Duration = _tcDTO,
                             RelatedPrimary = Guid.Empty,
                             DeviceIndex = 4,
                             EventType = ADCEventType.Secondary,
                             EventControl = new EventControlType[3] 
                             { 
                                 EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                             },
                             EventStatus = new EventRunStatus[] { },
                             ID = "SWTCH1",
                             OnAirTime = AirTimeEmpty,
                             SegmentNumber = 255,
                             Size = 350,
                             Som = _tcDTO,
                             Title = "",
                             TransitionEffect = Transition.Cut,
                             TransitionEffectRate = SwitchRate.Slow,
                         }
                    };

                    // Insert events
                    Console.WriteLine("Insert events to the first and second lists.");
                    foreach (var playList in PlayLists)
                    {
                        clList.InsertEventList(_ls, _serverName, playList, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    }

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread lists 
                    Console.WriteLine("Thread the first and second lists.");
                    clList.GangThread(_ls, _serverName, PlayLists);
                    Thread.Sleep(2000);

                    // Play lists 
                    Console.WriteLine("Play the first and second lists.");
                    clList.GangPlay(_ls, _serverName, PlayLists);
                    Thread.Sleep(10000);

                    foreach (int playList in PlayLists)
                    {
                        Console.WriteLine("Get states for the secondary and primary events:");
                        foreach (var eventDto in clList.GetList(_ls, _serverName, playList))
                        {
                            Console.WriteLine("Event ID : {0} has state {1} in list {2}", eventDto.ID, eventDto.ExtendedStatus, playList);
                        }
                    }

                    Console.WriteLine("The secondary events are triggered for the first and second lists.");
                    clList.GangPlaySecondary(_ls, _serverName, PlayLists);
                    Thread.Sleep(10000);

                    foreach (int playList in PlayLists)
                    {
                        Console.WriteLine("Get states for the secondary and primary events:");
                        foreach (var eventDto in clList.GetList(_ls, _serverName, playList))
                        {
                            Console.WriteLine("Event ID : {0} has state {1} in list {2}", eventDto.ID, eventDto.ExtendedStatus, playList);
                        }
                    }

                    foreach (int playList in PlayLists)
                    {
                        clList.UnlockList(_ls, _serverName, playList, ClientName);
                    }
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /**
        Insert events to the first and second lists.
        Thread the first and second lists.
        Play the first and second lists.
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY in list 1
        Event ID : SWTCH1 has state CUED in list 1
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY in list 2
        Event ID : SWTCH1 has state CUED in list 2
        The secondary events are triggered for the first and second lists.
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY in list 1
        Event ID : SWTCH1 has state PLAY in list 1
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY in list 2
        Event ID : SWTCH1 has state PLAY in list 2
        */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void GangPlaySecondary(LoginSession session, String server, IEnumerable<Int32> lists);

        #region Description and comments
        /// <summary>
        /// Threads and cues the first event in a playlist and any events within the time range specified in the lookahead.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// In this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for receiving all callbacks
                    Thread.Sleep(1000);

                    // Gets the inserted events list
                    ev_list = clList.GetList(_ls, _serverName, _list);
                    if (ev_list.Length > 0)
                    {
                        // Thread the list and wait for Threading status
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(1000);

                        ListStateValue[] listStatus = clList.GetListState(_ls, _serverName, _list);
                        if (listStatus.ToList().Exists(status => status == ListStateValue.Threading))
                        {
                            Console.WriteLine("List {0} is threaded successfully", _list.ToString());
                        }
                        else
                        {
                            Console.WriteLine("Error : List {0} is not threaded", _list.ToString());
                        }


                        // Unthread the list
                        Console.WriteLine("Unthread list {0}", _list.ToString());
                        clList.PerformListUnthread(_ls, _serverName, _list);
                        Thread.Sleep(1000);

                        listStatus = clList.GetListState(_ls, _serverName, _list);
                        if (!listStatus.ToList().Exists(status => status == ListStateValue.Threading))
                        {
                            Console.WriteLine("List {0} is unthreaded", _list.ToString());
                        }
                        else
                        {
                            Console.WriteLine("Error : List {0} is still threading", _list.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: The events are not inserted");
                    }

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert the events list
Thread list 2
List 2 is threaded successfully
Unthread list 2
List 2 is unthreaded
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListThread(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Stops a running transmission list, including the on-air event and any events that follow.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="PerformListThread"/> example.
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListUnthread(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Plays the first event in the specified list.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list = 2;

            try
            {
               // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the On Air event
                    EventDTO onAirEvent = clList.GetOnAirEvent(_ls, _serverName, _list);
                    if (onAirEvent != null)
                    {
                        string onAirID = onAirEvent.ID;
                        if (ev_list[0].ID == onAirID)
                        {
                            Console.WriteLine("First event {0} is On air", ev_list[0].ID);
                        }
                        else
                        {
                            Console.WriteLine("Error : First event {0} is not On air", ev_list[0].ID);
                        }
                    }
                    else
                        Console.WriteLine("Error : There are no On air event", ev_list[0].ID);

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
First event Demo0001 is On air
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListPlay(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Holds the countdown of an event that is playing.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list = 2;

            try
            {
               // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting ivents
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the list state and check that Hold state does not exist
                    var listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (!listStates.ToList().Exists(state => state == ListStateValue.Hold))
                    {
                        Console.WriteLine("List doesn't contain Hold state");
                    }
                    else
                    {
                        Console.WriteLine("Error: list conteains Hold state");
                    }
                    
                    // Hold list 
                    Console.WriteLine("Hold list {0}", _list.ToString());
                    clList.PerformListHold(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    // Get the list state and check that Hold state exists
                    listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (listStates.ToList().Exists(state => state == ListStateValue.Hold))
                    {
                        Console.WriteLine("List contains Hold state ");
                    }
                    else
                    {
                        Console.WriteLine("Error: list doesn't contain Hold state");
                    }
                    
                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
Get states of list number 2
List doesn't contain Hold state
Hold list 2
Get states of list number 2
List contains Hold state
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListHold(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Skips the event currently playing and plays the next event.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking the list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                        //Clear the cache
                        clListCB.UpdatedEventList.Clear();

                        EventDTO[] ev_list = new EventDTO[4]
                            {
                                SimpleEvent("Demo0001"),
                                SimpleEvent("Demo0002"),
                                SimpleEvent("Demo0003"),
                                SimpleEvent("Demo0004"),
                            };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                        // Wait for inserting ivents
                        Thread.Sleep(500);

                        // Thread the list 
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        // Play the list 
                        Console.WriteLine("Play list {0}", _list.ToString());
                        clList.PerformListPlay(_ls, _serverName, _list);
                        Thread.Sleep(4000);

                        // Check that Skipped events do not exist on the list (checking callback cache)
                        if (!clListCB.UpdatedEventList.Exists(ev => ev.EventObject.EventStatus.ToList().Exists(st => st == EventRunStatus.Skipped)))
                        {
                            Console.WriteLine("List doesn't contain Skipped events");
                        }
                        else
                        {
                            Console.WriteLine("Error: list contain Skipped events");
                        }

                        // Skip event 
                        Console.WriteLine("Skip event");
                        clList.PerformListSkip(_ls, _serverName, _list);
                        Thread.Sleep(8000);

                        // Check that Skipped events exist on the list
                        if (clListCB.UpdatedEventList.Exists(ev => ev.EventObject.EventStatus.ToList().Exists(st => st == EventRunStatus.Skipped)))
                        {
                            Console.WriteLine("List contain Skipped events");
                        }
                        else
                        {
                            Console.WriteLine("Error: list doesn't contain Skipped events");
                        }

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
<b>Insert events list</b>
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid a72d8a9d-3907-499d-bd3f-fb7f189cea67
OnEventsAdded notification: MAIN list 2 added event : ID Demo0004 Guid fb1a61eb-5faa-4061-8f6d-3091f0003110
OnListChange notification: MAIN list 2 changeType ContentsChanged
<b>Thread list 2</b>
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnListChange notification: MAIN list 2 changeType ContentsChanged
<b>Play list 2</b>
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid a72d8a9d-3907-499d-bd3f-fb7f189cea67
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0004 Guid fb1a61eb-5faa-4061-8f6d-3091f0003110
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnListChange notification: MAIN list 2 changeType DisplayChanged
<b>List doesn't contain Skipped events</b>
<b>Skip event</b>
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid a72d8a9d-3907-499d-bd3f-fb7f189cea67
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0004 Guid fb1a61eb-5faa-4061-8f6d-3091f0003110
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 05e3a287-fc37-4527-86ad-598a4119c4c7
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 691c8260-a4db-45f4-a565-9b1c72be2c6a
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid a72d8a9d-3907-499d-bd3f-fb7f189cea67
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid a72d8a9d-3907-499d-bd3f-fb7f189cea67
OnListChange notification: MAIN list 2 changeType DisplayChanged
<b>List contains Skipped events</b>
ListUnlocked: MAIN list 2
................

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListSkip(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Stops play and countdown. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting ivents
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the list state and check that Freeze state does not exist
                    var listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (!listStates.ToList().Exists(state => state == ListStateValue.Freeze))
                    {
                        Console.WriteLine("List doesn't contain Freeze state");
                    }
                    else
                    {
                        Console.WriteLine("Error: list contains Freeze state");
                    }

                    // Freeze event 
                    Console.WriteLine("Freeze list {0}", _list.ToString());
                    clList.PerformListFreeze(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    // Get the list state and check that Freeze state exists
                    listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (listStates.ToList().Exists(state => state == ListStateValue.Freeze))
                    {
                        Console.WriteLine("List contains Freeze state ");
                    }
                    else
                    {
                        Console.WriteLine("Error: list doesn't contain Freeze state");
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert the events list
Thread list 2
Play list 2
Get states of list number 2
List doesn't contain Freeze state
Freeze list 2
Get states of list number 2
List contain Freeze state
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListFreeze(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Rolls bypasses normal preroll values and plays the next event as fast as the equipment will allow.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the OnAir event
                    EventDTO onAirEvent = clList.GetOnAirEvent(_ls, _serverName, _list);
                    Console.WriteLine("OnAir event is {0}", onAirEvent.ID);

                    // Roll the event 
                    Console.WriteLine("Roll OnAir event");
                    clList.PerformListRoll(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get a new OnAir event
                    onAirEvent = clList.GetOnAirEvent(_ls, _serverName, _list);
                    Console.WriteLine("OnAir event is {0}", onAirEvent.ID);

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
OnAir event is Demo0001
Roll the OnAir event
OnAir event is Demo0002
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListRoll(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Recues an on-air event for the specified list. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting ivents
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Get the list state and check that the list is being played
                    var listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (listStates.ToList().Exists(state => state == ListStateValue.Playing))
                    {
                        Console.WriteLine("List is playing");
                    }
                    else
                    {
                        Console.WriteLine("Error: list is not playing");
                    }

                    // Recue the list 
                    Console.WriteLine("Recue list {0}", _list.ToString());
                    clList.PerformListRecue(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    // Get the list state and check that the list is not being played
                    listStates = clList.GetListState(_ls, _serverName, _list);
                    Console.WriteLine("Get states of list number {0}", _list.ToString());
                    if (!listStates.ToList().Exists(state => state == ListStateValue.Playing))
                    {
                        Console.WriteLine("List is not playing");
                    }
                    else
                    {
                        Console.WriteLine("Error: List is playing");
                    }

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert the events list
Thread list 2
Play list 2
Get states of list number 2
List is playing
Recue list 2
Get states of list number 2
List is not being playing
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListRecue(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Adds one second to the playing event’s duration 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play the list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    // Subscribe to receive notifications of list changes 
                    clList.RegisterListListener(_ls, _serverName, _list);

                    // + 1 sec 
                    Console.WriteLine("Call PerformListPlus1Sec() method");
                    clList.PerformListPlus1Sec(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    // - 1 sec 
                    Console.WriteLine("Call PerformListMinus1Sec() method");
                    clList.PerformListMinus1Sec(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    clList.UnregisterListListener(_ls, _serverName, _list);

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
Call PerformListPlus1Sec() method
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid d5791325-b037-4a7d-8daa-a036de9f6dbd
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid d5791325-b037-4a7d-8daa-a036de9f6dbd
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid 5f8a259e-a5ad-4adc-8a0e-0ebc76e5bba8
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0004 Guid d15a3350-f8e4-49a2-9493-48254730e78a
OnListChange notification: MAIN list 2 changeType DisplayChanged
Call PerformListMinus1Sec() method
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid d5791325-b037-4a7d-8daa-a036de9f6dbd
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid 5f8a259e-a5ad-4adc-8a0e-0ebc76e5bba8
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0004 Guid d15a3350-f8e4-49a2-9493-48254730e78a
OnListChange notification: MAIN list 2 changeType DisplayChanged
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListPlus1Sec(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Subtracts one second from the playing event’s duration.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="PerformListPlus1Sec"/> example.
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListMinus1Sec(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Turns on tension to the next VTR event in tension release to prepare upcoming VTR events. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo VTR should be assigned to the second list(For details on assigning a device, see ADC ConfigTool Help)
        /// Configure the following parameters of VTR as required: Check "Keep Media in VTR" option in configuration dialog for VTR( General tab).
        /// Select Diagnostics tab and enter VTR_eve to 'ID' field and press 'Inject' button.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int  _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    // Insert event
                    Console.WriteLine("Insert the event to list {0}", _list);
                    clList.InsertEvent(_ls, _serverName, _list, SimpleEvent("VTR_eve"), InsertPlaceType.AtListEnd, Guid.Empty);
                    // Thread list 
                    Console.WriteLine("Thread list {0}", _list);
                    clList.PerformListThread(_ls, _serverName, _list);

                    Thread.Sleep(3000);
                    EventDTO[] eventsBefore = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventsBefore)
                    {
                        Console.WriteLine("Event ID: {0}, event has state '{1}'", eventDto.ID, eventDto.ExtendedStatus);
                    }

                    // Perform List Ready
                    Console.WriteLine("Perform Ready list: {0}", _list);
                    clList.PerformListReady(_ls, _serverName, _list);
                    Thread.Sleep(3000);
                    
                    EventDTO[] eventReady = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventReady)
                    {
                        Console.WriteLine("Event ID: {0}, event has state '{1}'", eventDto.ID, eventDto.ExtendedStatus);
                    }

                    // Perform Tension Release command
                    Console.WriteLine("Perform Tension Release list: {0}", _list);
                    clList.PerformTensionRelease(_ls, _serverName, _list);
                    Thread.Sleep(3000);

                    EventDTO[] eventTenRel = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventTenRel)
                    {
                        Console.WriteLine("Event ID: {0}, event has state '{1}'", eventDto.ID, eventDto.ExtendedStatus);
                    }
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /** 
        Insert the event to list 2
        Thread list 2
        Event ID: VTR_eve, event has state 'TENREL'
        Perform Ready list: 2
        Event ID: VTR_eve, event has state 'READY'
        Perform Tension Release list: 2
        Event ID: VTR_eve, event has state 'TENREL'
        ListUnlocked: MAIN list 2
        */
        /// </code>
        /// </example>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListReady(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Toggles the signal switching paths between the on-air and protect devices for the specified list. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened. The first Disk should be assigned
        /// as the Air device. The second Disk should be assigned as the Protect. Both the Air and Protect devices must have 
        /// exactly the same media available in storage.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events to the list {0}", _list);
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    Console.WriteLine("Thread the list {0}", _list);
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    EventDTO[] eventsList = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventsList)
                    {
                        Console.WriteLine("Air device channel: {0} for event: {1} ", eventDto.DeviceIndex, eventDto.ID);
                    }
                    Console.WriteLine("Play list {0}", _list);
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);

                    Console.WriteLine("Toggle the signal between the on-air and protect devices.");
                    clList.PerformListProtect(_ls, _serverName, _list);

                    eventsList = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventsList)
                    {
                        Console.WriteLine("Protect device channel: {0} for event: {1} ", eventDto.DeviceIndex, eventDto.ID);
                    }
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
        
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {  
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        }          
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Insert events to the list 2
Thread the list 2
Air device channel: 1 for event: Demo0001
Air device channel: 1 for event: Demo0002
Air device channel: 1 for event: Demo0003
Play list 2
Toggle the signal between the on-air and protect devices.
Protect device channel: 2 for event: Demo0001
Protect device channel: 2 for event: Demo0002
Protect device channel: 2 for event: Demo0003
        */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListProtect(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Toggles the signal switching paths between the on-air and protect devices for the specified events.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Collection of events GUIDs in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Two Demo Video Disks should have the Output Port opened. The first Disk should be assigned
        /// as the Air device. The second Disk should be assigned as the Protect. Both the Air and Protect devices must have 
        /// exactly the same media available in storage.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events to the list {0}", _list);
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    Console.WriteLine("Thread the list {0}", _list);
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    EventDTO[] eventsList = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventsList)
                    {
                        Console.WriteLine("Air device channel: {0} for event: {1} ", eventDto.DeviceIndex, eventDto.ID);
                    }
                    Console.WriteLine("Play list {0}", _list);
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(5000);
                    Guid[] lastGuid = new Guid[] { eventsList.Last().AdcEventId };
                    Console.WriteLine("Toggle the signal between the on-air and protect devices for the event {0}", eventsList.Last().ID);
                    clList.PerformAirProtect(_ls, _serverName, _list, lastGuid);

                    eventsList = clList.GetList(_ls, _serverName, _list);
                    foreach (var eventDto in eventsList)
                    {
                        Console.WriteLine("Protect device channel: {0} for event: {1} ", eventDto.DeviceIndex, eventDto.ID);
                    }
                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /**
Insert events to the list 2
Thread the list 2
Air device channel: 1 for event: Demo0001
Air device channel: 1 for event: Demo0002
<b>Air device channel: 1 for event: Demo0003</b>
Play list 2
Toggle the signal between the on-air and protect devices for the event Demo0003
Protect device channel: 1 for event: Demo0001
Protect device channel: 1 for event: Demo0002
<b>Protect device channel: 2 for event: Demo0003</b>
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformAirProtect(LoginSession session, String server, Int32 list, IEnumerable<Guid> position);

        #region Description and comments
        /// <summary>
        /// Re-activates all events with done state for specified list, so they can be played on the list again.  
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking the list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                        EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                        // Wait for inserting events
                        Thread.Sleep(500);

                        // Thread the list 
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        //Clear the cache
                        clListCB.UpdatedEventList.Clear();

                        // Play the list 
                        Console.WriteLine("Play list {0}", _list.ToString());
                        clList.PerformListPlay(_ls, _serverName, _list);
                        Thread.Sleep(5000);

                        // Wait foк the DONE status of 3 events
                        bool done = false;
                        while (!done)
                        {
                            done = true;
                            for (int i = 0; i &lt; ev_list.Length; i++)
                            {
                                var item = clListCB.UpdatedEventList.Find(info => info.EventObject.ID == "Demo000" + (i+1).ToString()
                                    &amp;&amp; info.EventObject.EventStatus.Any(status => status == EventRunStatus.Done));
                                done = done &amp;&amp; (item != null);
                            }
                            Thread.Sleep(1000);
                        }

                        // Clear the DONE status of all events
                        Console.WriteLine("Clear DONE status for all events");
                        clList.PerformListClearDone(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        // Verify
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        for (int i = 0; i &lt; ev_list.Length; i++)
                            if (ev_list[i].EventStatus.Length == 0)
                            {
                                Console.WriteLine("Event # {0} status is cleared", ev_list[i].ID);
                            }
                            else
                            {
                                Console.WriteLine("Error : Event # {0} status is {1}", ev_list[i].ID, String.Join(", ", ev_list[i].EventStatus));
                            }


                        // Unlocking the list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
                   
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {  
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
Insert events list
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid c56eae47-0641-4cce-9cf3-6c899a91b302
OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid 180b5a8a-a8f1-489b-b9b6-977435936837
OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid cfee6042-13e0-4ad8-b1c1-d5166b68f212
OnListChange notification: MAIN list 2 changeType ContentsChanged
Thread list 2
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid c56eae47-0641-4cce-9cf3-6c899a91b302
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 180b5a8a-a8f1-489b-b9b6-977435936837
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid cfee6042-13e0-4ad8-b1c1-d5166b68f212
OnListChange notification: MAIN list 2 changeType DisplayChanged
OnListChange notification: MAIN list 2 changeType ContentsChanged
Play list 2
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid c56eae47-0641-4cce-9cf3-6c899a91b302
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid c56eae47-0641-4cce-9cf3-6c899a91b302         
         
/ Here are a few OnEventsUpdated notifications /
         
Clear the DONE status of all events
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid c56eae47-0641-4cce-9cf3-6c899a91b302
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 180b5a8a-a8f1-489b-b9b6-977435936837
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0003 Guid cfee6042-13e0-4ad8-b1c1-d5166b68f212
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnListChange notification: MAIN list 2 changeType ContentsChanged
Event # Demo0001 status is cleared
Event # Demo0002 status is cleared
Event # Demo0003 status is cleared
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformListClearDone(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Re-activates the specified events with done state, so they can be played on the list again. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Collection of events GUIDs in the list</param> 
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the PerformEventsClearDone method.
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Subscribe to recive notifications about list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        EventDTO[] ev_list = new EventDTO[2]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                        };

                        // Insert events
                        Console.WriteLine("Insert {0} events to the {1} list", ev_list.Count(), _list);
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                        // Wait for inserting events
                        Thread.Sleep(500);

                        // Thread list 
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        // Clear cache
                        clListCB.UpdatedEventList.Clear();

                        // Play list 
                        Console.WriteLine("Play the {0} list", _list.ToString());
                        clList.PerformListPlay(_ls, _serverName, _list);
                        Thread.Sleep(25000);
                        EventDTO[] ev_ret = clList.GetList(_ls, _serverName, _list);

                        Guid[] clearEvents;
                        if (ev_ret.Count() > 0)
                        {
                            clearEvents = new Guid[ev_ret.Count()];
                            for (int i = 0; i &lt; ev_ret.Count(); i++)
                            {
                                clearEvents[i] = ev_ret[i].AdcEventId;
                            }
                            // Perform events clear done
                            clList.PerformEventsClearDone(_ls, _serverName, _list, clearEvents);
                        }

                        Thread.Sleep(10000);
                        foreach (EventInfo info in clListCB.UpdatedEventList)
                        {
                            foreach (EventRunStatus status in info.EventObject.EventStatus)
                            {
                                Console.WriteLine("Event {0} has status {1} ", info.EventObject.ID, status);
                            }
                        }
                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for the {0} list on the {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventsClearDone(LoginSession session, String server, Int32 list, IEnumerable<Guid> position);

        #region Description and comments
        /// <summary>
        /// Re-activates the specified event with done state.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
               // Subscribe to receive notifications of list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is successful", _list.ToString(), _serverName);

                    // Locking the list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                        EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                        // Wait for inserting events
                        Thread.Sleep(500);

                        // Thread the list 
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        //Clear the cache
                        clListCB.UpdatedEventList.Clear();

                        // Play the list 
                        Console.WriteLine("Play list {0}", _list.ToString());
                        clList.PerformListPlay(_ls, _serverName, _list);
                        Thread.Sleep(5000);

                        // Wait for the DONE status of 3 events
                        bool done = false;
                        while (!done)
                        {
                            done = true;
                            for (int i = 0; i &lt; ev_list.Length; i++)
                            {
                                var item = clListCB.UpdatedEventList.Find(info => info.EventObject.ID == "Demo000" + (i + 1).ToString()
                                    &amp;&amp; info.EventObject.EventStatus.Any(status => status == EventRunStatus.Done));
                                done = done &amp;&amp; (item != null);
                            }
                            Thread.Sleep(1000);
                        }
                                                
                        // Clear the DONE status of the 2nd event                        
                        Console.WriteLine("Clear the DONE status of the 2nd event");

                        int index = 1;
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        clList.PerformEventClearDone(_ls, _serverName, _list, ev_list[index].AdcEventId);
                        Thread.Sleep(2000);

                        // Verify
                        ev_list = clList.GetList(_ls, _serverName, _list);
                        if (ev_list[index].EventStatus.Length == 0)
                            {
                                Console.WriteLine("Event # {0} status is cleared", ev_list[index].ID);
                            }
                            else
                            {
                                Console.WriteLine("Error : Event # {0} status is {1}", ev_list[index].ID, String.Join(", ", ev_list[index].EventStatus));
                            }


                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                        Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription to list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListListener subscription to list number 2 on MAIN server is successful
ListLocked: MAIN list 2
Insert events list
OnEventsAdded notification: MAIN list 2 added event : ID Demo0001 Guid 807adb0b-cfba-4666-ab07-2f2cfcb62449
OnEventsAdded notification: MAIN list 2 added event : ID Demo0002 Guid 1f507233-2275-4740-abe1-8a34c96d44e8
OnEventsAdded notification: MAIN list 2 added event : ID Demo0003 Guid c883aef4-35ef-40b5-b06b-96de4a1d75ad
OnListChange notification: MAIN list 2 changeType ContentsChanged
Thread list 2
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 807adb0b-cfba-4666-ab07-2f2cfcb62449
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 1f507233-2275-4740-abe1-8a34c96d44e8
OnListChange notification: MAIN list 2 changeType ContentsChanged
Play list 2
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 807adb0b-cfba-4666-ab07-2f2cfcb62449
OnListChange notification: MAIN list 2 changeType ContentsChanged
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0001 Guid 807adb0b-cfba-4666-ab07-2f2cfcb62449
         
/ Here are a few OnEventsUpdated notifications /
     
Clear the DONE status of the 2nd event
OnEventsUpdated  notification: MAIN list 2 updated events : ID Demo0002 Guid 1f507233-2275-4740-abe1-8a34c96d44e8
OnListChange notification: MAIN list 2 changeType ContentsChanged
Event # Demo0002 status is cleared
ListUnlocked: MAIN list 2
OnListChange notification: MAIN list 2 changeType ContentsChanged
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventClearDone(LoginSession session, String server, Int32 list, Guid position);

        #region Description and comments
        /// <summary>
        /// Threads and cues the specified event.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// In this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the event
                    Console.WriteLine("Event is being threaded", _list.ToString());
                    int index = 1;
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[index].AdcEventId;
                    clList.PerformEventThread(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    //Trying to launch preview play
                    Console.WriteLine("Trying to launch preview play");
                    clList.PerformEventPreview(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    var res = clList.GetList(_ls, _serverName, _list).ToList();
                    if (res.Any(info => info.ID == ev_list[index].ID &amp;&amp; info.EventStatus.Any(status => status == EventRunStatus.Previewed)))
                    {
                        Console.WriteLine("Event was previewed");
                    }
                    else
                    {
                        Console.WriteLine("Error : event wasn't previewed");
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Event is being threaded
Trying to launch preview play
Event was previewed
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventThread(LoginSession session, String server, Int32 list, Guid position);

        #region Description and comments
        /// <summary>
        /// Stops a running the specified event. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the event
                    Console.WriteLine("Event is being threaded", _list.ToString());
                    int index = 1;
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[index].AdcEventId;
                    clList.PerformEventThread(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    // Unthread the event
                    Console.WriteLine("Event is being unthread", _list.ToString());
                    clList.PerformEventUnthread(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    //Trying to launch preview play
                    Console.WriteLine("Trying to launch preview play");
                    clList.PerformEventPreview(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    var res = clList.GetList(_ls, _serverName, _list).ToList();
                    if (!res.Any(info => info.ID == ev_list[index].ID &amp;&amp; info.EventStatus.Any(status => status == EventRunStatus.Previewed)))
                    {
                        Console.WriteLine("Event wasn't previewed");
                    }
                    else
                    {
                        Console.WriteLine("Error : event was previewed");
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Event is being threaded
Event is being unthread
Trying to launch preview play
Event wasn't previewed
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventUnthread(LoginSession session, String server, Int32 list, Guid position);

        #region Description and comments
        /// <summary>
        /// Recues the specified event. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the event
                    Console.WriteLine("Event is being threaded", _list.ToString());
                    int index = 1;
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[index].AdcEventId;
                    clList.PerformEventThread(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    //Trying to launch preview play
                    Console.WriteLine("Trying to launch preview play");
                    clList.PerformEventPreview(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    // Clear the cache
                    clListCB.UpdatedEventList.Clear();
                                        
                    // Recue the event 
                    Console.WriteLine("Recue event");
                    clList.PerformEventRecue(_ls, _serverName, _list, evGuid);
                    Thread.Sleep(1000);

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        { 
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Event is being threaded
Trying to launch preview play
Recue event
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventRecue(LoginSession session, String server, Int32 list, Guid position);

        #region Description and comments
        /// <summary>
        /// Previews the specified list. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Event GUID ID in the list</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="PerformEventThread"/> example.
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventPreview(LoginSession session, String server, Int32 list, Guid position);

        #region Description and comments
        /// <summary>
        /// Adjusts the lookahead area from the current setting to a full list look ahead.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the ToggleLookahead method.
        ///  <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    EventDTO[] ev_list = new EventDTO[10]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                            SimpleEvent("Demo0005"),
                            SimpleEvent("Demo0006"),
                            SimpleEvent("Demo0007"),
                            SimpleEvent("Demo0008"),
                            SimpleEvent("Demo0009"),
                            SimpleEvent("Demo0010")
                        };
                    int laNum = 5;
                    clList.SetLookahead(_ls, _serverName, _list, laNum);
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);
                    Console.WriteLine("The lookahead is set {0} for list {1}", laNum, _list);
                    clList.ToggleLookahead(_ls, _serverName, _list);
                    
                    laNum = clList.GetLookahead(_ls, _serverName, _list);
                    Console.WriteLine("The lookahead was changed to {0} for list {1}", laNum, _list);

                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }       
        */
        ///  </code>
        ///  Console result
        ///  <code language="Console">
        /// The lookahead is set 5 for list 2
        /// The lookahead was changed to 10 for list 2
        ///  </code>
        ///  </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void ToggleLookahead(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Sets lookahead events number
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="lookahead">Number of lookahead events</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the SetLookahead method.
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
             
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            try
            {
               int laNum = clList.GetLookahead(_ls, _serverName, _list);
               Console.WriteLine("The lookahead is {0} for list {1}",laNum,  _list);
               clList.SetLookahead(_ls, _serverName, _list, laNum + 10);
               Thread.Sleep(500);
               laNum = clList.GetLookahead(_ls, _serverName, _list);
               Console.WriteLine("The lookahead was changed to {0} for list {1}", laNum, _list);

            }
            catch (Exception ex)
            {
               Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
       */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
The lookahead is 60 for list 2
The lookahead was changed to 70 for list 2
        */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void SetLookahead(LoginSession session, String server, Int32 list, Int32 lookahead);

        #region Description and comments
        /// <summary>
        /// Cuts next event
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformCutNext(LoginSession session, String server, Int32 list);
        
        #region Description and comments
        /// <summary>
        /// Lets Roll command.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformLetRoll(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Plays the next secondary event.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// <remarks>
        /// Device Server configuration:  Assign Switch Only Device and Demo Video Disk to the desired channels. Demo Video Disk should 
        /// have the Output Port opened. All parameters for Switch Only Device should be by default(For details on assigning a device, see ADC ConfigTool Help)
        /// The Disk and Switch Only Device should be assigned to the second list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
           
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    // The time code for 'OnAirTime' should be empty for the secondary Audio/Video event.
                    TimeCodeDTO AirTimeEmpty = new TimeCodeDTO();
                    AirTimeEmpty.Hour = 0xFF;
                    AirTimeEmpty.Minute = 0xFF;
                    AirTimeEmpty.Second = 0xFF;
                    AirTimeEmpty.Frame = 0xFF;
                    _tcDTO.Minute = 1;

                    EventDTO[] ev_list = new EventDTO[2]
                    {
                         SimpleEvent("Demo0001"),
                         new EventDTO{
                             AdcEventId = Guid.NewGuid(),
                             Duration = _tcDTO,
                             RelatedPrimary = Guid.Empty,
                             DeviceIndex = 4,
                             EventType = ADCEventType.Secondary,
                             EventControl = new EventControlType[3] 
                             { 
                                 EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                             },
                             EventStatus = new EventRunStatus[] { },
                             ID = "SWTCH1",
                             OnAirTime = AirTimeEmpty,
                             SegmentNumber = 255,
                             Size = 350,
                             Som = _tcDTO,
                             Title = "",
                             TransitionEffect = Transition.Cut,
                             TransitionEffectRate = SwitchRate.Slow,
                         }
                    };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread list 
                    Console.WriteLine("Thread list {0}", _list);
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play list 
                    Console.WriteLine("Play list {0}", _list);
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(10000);

                    Console.WriteLine("Get states for the secondary and primary events:");
                    foreach (var eventDto in clList.GetList(_ls, _serverName, _list))
                    {
                        Console.WriteLine("Event ID : {0} has state {1}", eventDto.ID, eventDto.ExtendedStatus);
                    }

                    Console.WriteLine("The secondary event is triggered.");
                    clList.PerformRollSecondary(_ls, _serverName, _list);
                    Thread.Sleep(10000);

                    Console.WriteLine("Get states for the secondary and primary events:");
                    foreach (var eventDto in clList.GetList(_ls, _serverName, _list))
                    {
                        Console.WriteLine("Event ID : {0} has state {1}", eventDto.ID, eventDto.ExtendedStatus);
                    }

                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
           
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result:
        /// <code language="Console">
        /**
        Insert events list
        Thread list 2
        Play list 2
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY
        Event ID : SWTCH1 has state CUED
        The secondary event is triggered.
        Get states for the secondary and primary events:
        Event ID : Demo0001 has state PLAY
        Event ID : SWTCH1 has state PLAY
         */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformRollSecondary(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Places a cued event’s VTR into tension release.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <example>
        /// See <see cref="PerformListReady"/> for an example of how to use <c>PerformTensionRelease</c> method.
        /// </example>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformTensionRelease(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Subscribes to receive Server connection state (connected, disconnected) 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <returns>Returns true if the subscription is successful otherwise false</returns>
        /// <example> 
        /// In this code the client subscribes to receive 'MAIN' Server connection state
        /// <code language="cs">
        /// static void Main(string[] args)
        /// {
        ///     ListServiceCallback clListCB = new ListServiceCallback();
        ///     ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///     
        ///     LoginSession _ls = new LoginSession();
        ///     String _serverName = "MAIN";
        /// 
        ///     try
        ///     {
        ///         // Subscribe to receive 'MAIN' Server connection state
        ///         bool res = clList.RegisterConnectionStateListener(_ls, _serverName);
        ///         if (res)
        ///         {
        ///             Console.WriteLine("ConnectionState subscription is successful");
        ///         }
        ///         else
        ///         {
        ///             Console.WriteLine("ConnectionState subscription is unsuccessful");
        ///         }
        ///         
        ///         Thread.Sleep(10000);
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         Console.WriteLine("Error: {0}, StackTrace: {1}",ex.Message,ex.StackTrace);                    
        ///     }
        ///}
        /// </code>
        /// Console result
        /// (ADC Device Server is not runing)
        /// <code language="Console">
        ///ConnectionState subscription is successful
        ///MAIN Disconnected
        ///MAIN Connecting
        ///MAIN Disconnected
        ///MAIN Connecting
        ///MAIN Disconnected
        ///MAIN Connecting
        /// </code>
        /// Starting ADC Device Server
        /// <code language="Console">        
        ///MAIN Disconnected
        ///MAIN Connecting
        ///MAIN Disconnected
        ///MAIN Connecting
        ///MAIN Connected
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        Boolean RegisterConnectionStateListener(LoginSession session, String server);

        #region Description and comments
        /// <summary>
        /// Stops receiving server connection state 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <example> 
        /// <code language="cs">
        /// static void Main(string[] args)
        /// {
        ///     ListServiceCallback clListCB = new ListServiceCallback();
        ///     ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///     
        ///     LoginSession _ls = new LoginSession();
        ///     String _serverName = "MAIN";
        ///     
        ///     try
        ///     {
        ///         // Subscribe to receive 'MAIN' Server connection state
        ///         bool res = clList.RegisterConnectionStateListener(_ls, _serverName);
        ///         if (res)
        ///         {
        ///             Console.WriteLine("ConnectionState subscription is successful");
        ///         }
        ///         else
        ///         {
        ///             Console.WriteLine("ConnectionState subscription is unsuccessful");
        ///         }
        ///         
        ///         
        ///         Thread.Sleep(1000);
        ///         
        ///         // Stop receiving 'MAIN' server connection state.
        ///         clList.UnregisterConnectionStateListener(_ls, _serverName);
        ///         Console.WriteLine("Stop receiving 'MAIN' server connection state.");
        ///
        ///         Thread.Sleep(1000);
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         Console.WriteLine("Error: {0}, StackTrace: {1}",ex.Message,ex.StackTrace);                    
        ///     }
        ///}
        /// </code>
        /// 
        /// Console result
        /// <code language="Console">
        ///ConnectionState subscription is successful
        ///MAIN Disconnected
        ///MAIN Connecting
        ///MAIN Disconnected
        ///MAIN Connecting
        ///Stop receiving 'MAIN' server connection state.
        /// </code>
        /// </example>
        #endregion
        [OperationContract(IsOneWay = true)]
        void UnregisterConnectionStateListener(LoginSession session, String server);

        #region Description and comments
        /// <summary>
        /// Gets the list of currently connected Device Servers
        /// </summary>
        /// <param name="session">User session</param>
        /// <returns>The list of Device Server names</returns>
        /// <example> 
        /// <code language="cs">
        /// static void Main(string[] args)
        /// {
        ///     ListServiceCallback clListCB = new ListServiceCallback();
        ///     ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///     try
        ///     {
        ///         // Get the list of connected Device Servers
        ///         var res = clList.GetAvailableDeviceServers(new LoginSession());
        ///         Console.WriteLine("Count of available servers: {0}", res.Length);
        ///         foreach (String serverName in res)
        ///         {
        ///             Console.WriteLine("{0} is available", serverName);
        ///         }  
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         Console.WriteLine("Error: {0}, StackTrace: {1}",ex.Message,ex.StackTrace);                    
        ///     }
        ///}
        /// </code>
        /// 
        /// Console result (service is configured to connect to the following servers: MAIN (available), MAIN2 (not available), MAIN3 (available))
        /// <code language="Console">
        ///Count of available servers: 2
        ///MAIN is available
        ///MAIN3 is available
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        IEnumerable<String> GetAvailableDeviceServers(LoginSession session);

        #region Description and comments
        /// <summary>
        /// Gets list of all configured Device Servers names
        /// </summary>
        /// <param name="session">User session</param>
        /// <returns>The list of all configured Device Server names</returns>
        /// <example>
        /// <code language="cs">
        /// static void Main(string[] args)
        /// {
        ///     ListServiceCallback clListCB = new ListServiceCallback();
        ///     ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///    try
        ///    {
        ///       // Get configured servers
        ///       string[] allServers = clList.GetAllConfiguredServers(_ls);
        ///       for (int i = 0; i &lt; allServers.Count(); i++)
        ///       {
        ///           Console.WriteLine("Configured Server is {0}", allServers[i]);
        ///       }
        ///    }
        ///    catch (Exception ex)                
        ///    {
        ///        Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);  
        ///    }  
        ///} 
        ///</code>
        /// Console result (service is configured to connect to the following servers: MAIN, ds1 )
        ///<code language="Console">
        /// Configured Server is ds1
        /// Configured Server is MAIN
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        IEnumerable<String> GetAllConfiguredServers(LoginSession session);

        #region Description and comments
        /// <summary>
        /// Gets List Count
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <returns>List Count on a specified server</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// Get the list count on 'MAIN' server
        /// <code>
        /// static void Main(string[] args)
        ///        {
        ///            ListServiceCallback clListCB = new ListServiceCallback();
        ///            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
        ///            
        ///            LoginSession _ls = new LoginSession();
        ///            String _server = "MAIN";
        ///
        ///            try
        ///            {
        ///                Console.WriteLine("The number of lists on the server {0} is {1}",_server,clList.GetListCount(_server).ToString());
        ///            }
        ///            catch (Exception ex)                
        ///            {
        ///                Console.WriteLine(ex.Message);
        ///            }
        ///        }
        /// </code> 
        /// Console result
        /// <code language="Console">
        ///The number of lists on server MAIN is 40
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Int32 GetListCount(String server);

        #region Description and comments
        /// <summary>
        /// Gets a specified number of events after the event with specified Guid
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="start">Starting Guid</param>
        /// <param name="count">Number of Events to read</param>
        /// <returns>The list of events</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[6]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                            SimpleEvent("Demo0005"),
                            SimpleEvent("Demo0006")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    //Gets a specified number of events after the event with specified Guid                    
                    int index = 2;
                    Console.WriteLine("Gets 3 events after {0}nd event", index.ToString());
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[index].AdcEventId;
                    EventDTO[] res = clList.GetListPartial(_ls, _serverName, _list, evGuid, 3);

                    Console.WriteLine("Result:");
                    res.ToList().ForEach(info => 
                        Console.WriteLine("event ID : {0} GUID {1}",info.ID,info.AdcEventId)
                        );

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Gets 3 events after 2nd event
Result:
event ID : Demo0003 GUID b7a13fe1-c6d2-4599-a9e9-7eb29e51df7c
event ID : Demo0004 GUID 093d0f2b-f284-4dce-93ba-3e03ed5d5840
event ID : Demo0005 GUID ef760f38-12d5-4225-ad10-cdc8121c1488
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetListPartial(LoginSession session, String server, Int32 list, Guid start, Int32 count);

        #region Description and comments
        /// <summary>
        /// Locks the specified list for editing by a specified name client.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server Name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="clientName">Identifier of the client that called LockList</param>
        /// <returns><c>true</c> if the list has been locked; otherwise,<c>false</c>.</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the LockList method. 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
             // Initialise the second client
                ListServiceCallback secClListCB = new ListServiceCallback();
                ListServiceClient secClList = new ListServiceClient(new InstanceContext(secClListCB));
                String secClientName = "TheSecondClient";

                //Both clients are subscribed to receive notifications about list changes
                clList.RegisterListListener(_ls, _serverName, _list);
                secClList.RegisterListListener(_ls, _serverName, _list);

                // The first client is locked the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    // Wait for receiving callbacks
                    Thread.Sleep(500);

                    // The second client tries to lock the list
                    Console.WriteLine("The second client tries to lock the list");
                    if (secClList.LockList(_ls, _serverName, _list, secClientName))
                    {
                        Console.WriteLine("Error : The second client was locked the {0} list on the {1} server", _list, _serverName);
                    }
                    else
                    {
                        Console.WriteLine("The second client was not locked the {0} list on the {1} server", _list, _serverName);
                    }

                    // The first client is unlocking list
                   clList.UnlockList(_ls, _serverName, _list, ClientName);
                }
                else
                {
                    Console.WriteLine("Error : First client : List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            } 
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /** 
        ListLocked notification: The list 2 on the server MAIN was locked by the Client_ADC client.
        ListLocked notification: The list 2 on the server MAIN was locked by the Client_ADC client.
        The second client tries to lock the list
        The second client was not locked the 2 list on the MAIN server
        ListUnlocked notification: The list 2 on the server MAIN was unlocked by the Client_ADC client.
        ListUnlocked notification: The list 2 on the server MAIN was unlocked by the Client_ADC client.
        */
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean LockList(LoginSession session, String server, Int32 list, String clientName);

        #region Description and comments
        /// <summary>
        /// Unlocks the list by a specified name client.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server Name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="clientName">Identifier of the client that called LockList</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// See <see cref="LockList"/> example.
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void UnlockList(LoginSession session, String server, Int32 list, String clientName);

        #region Description and comments
        /// <summary>
        /// Checks if the list is available for locking or editing.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server Name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>true if the list is unlocked or locked by this client, otherwise false.</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="ListLockedError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Second client
                ListServiceCallback secondClListCB = new ListServiceCallback();
                ListServiceClient secondClList = new ListServiceClient(new InstanceContext(secondClListCB));

                // Second client checks availability of the list
                if (secondClList.IsListAvailable(_ls, _serverName, _list))
                {
                    Console.WriteLine("Second client : list {0} is available", _list.ToString());
                }
                else
                {
                    Console.WriteLine("Second client : list {0} is not available", _list.ToString());
                }

                // First client is locksing the list
                Console.WriteLine("First client tries to lock the list");
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("First client : ListLocked: {0} list {1}", _serverName, _list.ToString());

                    // Second client checks availability of the list
                    if (secondClList.IsListAvailable(_ls, _serverName, _list))
                    {
                        Console.WriteLine("Second client : list {0} is available", _list.ToString());
                    }
                    else
                    {
                        Console.WriteLine("Second client : list {0} is not available", _list.ToString());
                    }

                    // First client is unlocking the list
                    Console.WriteLine("First client tries to unlock the list");
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("First client : ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("Error : First client : List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }            
        }
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
Second client : list 2 is available
First client tries to lock the list
First client : ListLocked: MAIN list 2
Second client : list 2 is not available
First client tries to unlock the list
First client : ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(ListLockedError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean IsListAvailable(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Checks if the specified number of events are permitted for inserting.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server Name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="numberOfEvents">Specified number of events</param>
        /// <returns>true if the specified number of events are permitted for inserting, otherwise false.</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Boolean CheckListCapacity(LoginSession session, String server, Int32 list, Int32 numberOfEvents);

        #region Description and comments
        /// <summary>
        /// Returns the max number of events a playlist can hold.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server Name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>The max number of events a playlist can hold.</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /// </code>
        /// </example>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        Int32 GetMaxEventsInList(LoginSession session, String server, Int32 list);

        #region Description and comments
        /// <summary>
        /// Gets a specified number of events after the event with a particular index
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="listIndex">List number on Server</param>
        /// <param name="offset">Index of event</param>
        /// <param name="count">Number of events</param>
        /// <returns>The list of events</returns>
        /// <example>
        /// At this example 
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[6]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                            SimpleEvent("Demo0005"),
                            SimpleEvent("Demo0006")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    //Gets a specified number of events after the event with the index number        
                    int index = 1;
                    Console.WriteLine("Gets 3 events after {0}nd event", index.ToString());
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[index].AdcEventId;
                    EventDTO[] res = clList.GetListPage(_ls, _serverName, _list, index, 3);

                    Console.WriteLine("Result:");
                    res.ToList().ForEach(info =>
                        Console.WriteLine("event ID : {0} GUID {1}", info.ID, info.AdcEventId)
                        );

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert the events list
Gets 3 events after the 2nd event
Result:
event ID : Demo0002 GUID 5634fec1-c347-4cad-990d-09094f83bf48
event ID : Demo0003 GUID fddf6b1c-701b-47f9-84c5-9cde65454b18
event ID : Demo0004 GUID f57d0f7b-9dc8-4fe4-af2a-cf3af6efc2c8
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
		[OperationContract]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetListPage(LoginSession session, string server, int listIndex, int offset, int count);

        #region Description and comments
        /// <summary>
        /// Gets a filtered list of events
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="listIndex">List number on Server</param>
        /// <param name="doneCount">The number of events having status 'Done'</param>
        /// <param name="onAirCount">The number of events having status 'Running'</param>
        /// <param name="nextCount">The number of events having status 'Unknown'</param>
        /// <param name="errorCount">The number of events having status 'Error'</param>
        /// <returns>The list of events</returns>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

            try
            {
                // Locking the list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);


                    EventDTO[] ev_list = new EventDTO[10]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                            SimpleEvent("Demo0005"),
                            SimpleEvent("Demo0006"),
                            SimpleEvent("!InvalidID"),
                            SimpleEvent("Demo0008"),
                            SimpleEvent("Demo0009"),
                            SimpleEvent("!InvalidID")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread the list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(20000);

                    //Gets a specified number of events 
                    Console.WriteLine("Gets a specified number of events");
                    EventDTO[] res = clList.GetListFiltered(_ls, _serverName, _list, 5, 1, 3, 5);

                    Console.WriteLine("Result:");
                    res.ToList().ForEach(info =>
                        Console.WriteLine("event ID : {0} GUID {1}", info.ID, info.AdcEventId)
                        );

                    // Unlocking the list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }               
        }
          
          
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        } 
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
Gets a specified number of events
Result:
event ID : Demo0001 GUID f3666eb6-26b4-4353-93c4-6854962cbd64
event ID : Demo0002 GUID 3d3ae106-b25c-41ae-b262-9613b3a5aa59
event ID : Demo0003 GUID bcea6208-e1a8-4605-adfe-51a2d7ed5f32
event ID : Demo0004 GUID 5e303021-1745-40ea-9850-51a764fc95b8
event ID : Demo0005 GUID d43ffae7-fb21-4a8b-8d94-435a2f1db6ca
event ID : !InvalidID GUID 5b67ad6e-04c5-4036-adf9-9cecbff6b2cd
event ID : !InvalidID GUID aa977549-69d6-4e8b-839b-58c8c1599f2b
ListUnlocked: MAIN list 2

         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetListFiltered(LoginSession session, string server, int listIndex, int doneCount, int onAirCount, int nextCount, int errorCount);

        #region Description and comments
        /// <summary>
        /// Gets the collection of lists for a specified period of time
        /// </summary> 
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="listIndexes">The collection of lists indexes</param>
        /// <param name="from">The string timecode value</param>
        /// <param name="to">The string timecode value</param>
        /// <returns>The collection of lists</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ServiceParametersValidationError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the GetListsByPeriod method. 
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /** static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;

          try 
            {
                String fromTC = "16:13:00;00";// To get events from this TimeCode 
                String toTC = "16:13:40;00";// To get events to this TimeCode 

                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("The {0} list is locked on {1} server by {2} client", _list, _serverName, ClientName);

                    EventDTO[] ev_list = new EventDTO[10]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003"),
                            SimpleEvent("Demo0004"),
                            SimpleEvent("Demo0005"),
                            SimpleEvent("Demo0006"),
                            SimpleEvent("Demo0007"),
                            SimpleEvent("Demo0008"),
                            SimpleEvent("Demo0009"),
                            SimpleEvent("Demo0010")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(20000);

                    //Gets specified number of events by period
                    Console.WriteLine("Gets specified number of events by period from {0} to {1} ", fromTC, toTC);
                    int[] mas = new int[1] { _list };
                    EventDTO[][] res = clList.GetListsByPeriod(_ls, _serverName, mas, fromTC, toTC);

                    Console.WriteLine("Result:");
                    res.ToList().ForEach(info =>
                        info.ToList().ForEach(rec =>
                        Console.WriteLine("event ID : {0} GUID {1}", rec.ID, rec.AdcEventId))
                        );

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("The {0} list is unlocked on {1} server by {2} client", _list, _serverName, ClientName);
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
The 2 list is locked on MAIN server by Client_ADC client
Insert events list
Thread list 2
Play list 2
Gets specified number of events by period from 16:13:00;00 to 16:13:40;00
Result:
event ID : Demo0001 GUID 2e7819a9-6406-4bdd-bb48-9ea513669062
event ID : Demo0002 GUID 09e41c66-be3f-4496-a758-94128430a1e1
The 2 list is unlocked on MAIN server by Client_ADC client
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ServiceParametersValidationError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<IEnumerable<EventDTO>> GetListsByPeriod(LoginSession session, string server, IEnumerable<int> listIndexes, string from, string to);

        #region Description and comments
        /// <summary>
        /// Gets secondary events for the primary event with specified Guid
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="listIndex">List number on Server</param>
        /// <param name="relatedPrimary">GUID of the related primary event</param>
        /// <returns>The list of events</returns>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// <code language="cs">
        /**static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO primari_ev = SimpleEvent("Demo0001");
                    EventDTO[] ev_list = new EventDTO[5]
                        {
                            primari_ev,
                            SecondaryEvent("Demo0002",primari_ev.AdcEventId),
                            SecondaryEvent("Demo0003",primari_ev.AdcEventId),
                            SecondaryEvent("Demo0004",primari_ev.AdcEventId),
                            SimpleEvent("Demo0005")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);

                    // Thread list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(2000);

                    // Play list 
                    Console.WriteLine("Play list {0}", _list.ToString());
                    clList.PerformListPlay(_ls, _serverName, _list);
                    Thread.Sleep(20000);

                    //Gets specified number of secondaries events by event with specified Guid
                    Console.WriteLine("Gets specified number of secondaries events for the primary event with specified Guid: {0}, " +
                                      "ID: {1}  ", primari_ev.AdcEventId, primari_ev.ID);
                    Guid evGuid = clList.GetList(_ls, _serverName, _list)[0].AdcEventId;
                    EventDTO[] res = clList.GetListOfSecondaries(_ls, _serverName, _list, evGuid);

                    Console.WriteLine("Result: ");
                    res.ToList().ForEach(info =>
                        Console.WriteLine("Secondary event ID : {0} GUID {1}", info.ID, info.AdcEventId)
                        );

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
         
        // Prepares a simple playable primary event 10 secs long
        public EventDTO SimpleEvent(string newID)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;         
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                DeviceIndex = 4,
                EventType = ADCEventType.Primary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] {},
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,
            };
        }
        
        // Prepares a simple playable secondary event 10 secs long
        public EventDTO SecondaryEvent(string newID, Guid primaryGuid)
        {
            TimeCodeDTO tcDTO = new TimeCodeDTO();
            tcDTO.Second = 10;
            return new EventDTO
            {
                AdcEventId = Guid.NewGuid(),
                Duration = tcDTO,
                RelatedPrimary = primaryGuid,
                DeviceIndex = 4,
                EventType = ADCEventType.Secondary,
                EventControl = new EventControlType[3] 
                { 
                    EventControlType.AutoPlay, EventControlType.AutoThread, EventControlType.AutoSwitch 
                },
                EventStatus = new EventRunStatus[] { },
                ID = newID,
                OnAirTime = tcDTO,
                SegmentNumber = 255,
                Size = 350,
                Som = tcDTO,
                Title = "",
                TransitionEffect = Transition.Cut,
                TransitionEffectRate = SwitchRate.Slow,

            };
        }        
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
Thread list 2
Play list 2
Gets specified number of secondaries events for the primary event with specified Guid: cc74c224-8a20-4901-b980-042ae0f505ae,
ID: Demo0001
Result:
Secondary event ID : Demo0002 GUID 3b0b292d-edcf-4dba-9692-5c3675d657b9
Secondary event ID : Demo0003 GUID 2c2105ad-335b-47eb-b529-91a5fd7d9e3d
Secondary event ID : Demo0004 GUID 89f569cc-ce34-4955-86d8-e570d76c2e25
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
		[OperationContract]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetListOfSecondaries(LoginSession session, string server, int listIndex, Guid relatedPrimary);

        #region Description and comments
        /// <summary>
        /// Process of adjusting the time column to reflect the time of day each
        /// event will play based on the duration of each event. As an event plays to air, the time
        /// field of the events following down the list will adjust as necessary to reflect the
        /// approximate time of day they will execute.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Guid position</param>
        /// <param name="holdtime">The time difference (overlap or gap) between runtime and hardstart event beginning</param>
        /// <returns>Result of the comparison of events</returns>
        /// <example>
        /// The following example demonstrates how you can use the RippleTime method.
        /// <code language="cs">
        /**static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);
                    TimeCodeDTO holdtime = new TimeCodeDTO();

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);
                    var evToggle = clList.GetList(_ls, _serverName, _list);
                    if (evToggle != null)
                    {
                        TimecodeCompareResult result = clList.RippleTime(out holdtime, _ls, _serverName, _list, evToggle.ToList().First().AdcEventId);
                        Console.WriteLine("The result is: {0}", result.ToString());
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
         }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
The result is: Equal
ListUnlocked: MAIN list 2*/
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        TimecodeCompareResult RippleTime(LoginSession session, string server, Int32 list, Guid position, out TimeCodeDTO holdtime);

        #region Description and comments
        /// <summary>
        /// Changes the specified events to the hard start events
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="position">Collection of Guids</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the PerformEventToggleHardStart method.
        /// <code language="cs">
        /**static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Locking list
                if (clList.LockList(_ls, _serverName, _list, ClientName))
                {
                    Console.WriteLine("ListLocked: {0} list {1}", _serverName, _list);

                    EventDTO[] ev_list = new EventDTO[3]
                        {
                            SimpleEvent("Demo0001"),
                            SimpleEvent("Demo0002"),
                            SimpleEvent("Demo0003")
                        };

                    Guid[] ev_guid = new Guid[1];

                    // Insert events
                    Console.WriteLine("Insert events list");
                    clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                    // Wait for inserting events
                    Thread.Sleep(500);
                    var evToggle = clList.GetList(_ls, _serverName, _list);
                    if (evToggle != null)
                    {
                        ev_guid[0] = evToggle.ToList().First().AdcEventId;
                        Console.WriteLine("The first event has following control types:");
                        foreach (var eventDto in evToggle.ToList().First().EventControl)
                        {
                            Console.WriteLine("Event ID: {0} Event type: {1}", evToggle.ToList().First().ID, eventDto);
                        }
                    }

                    // Thread list 
                    Console.WriteLine("Thread list {0}", _list.ToString());
                    clList.PerformListThread(_ls, _serverName, _list);
                    Thread.Sleep(1000);

                    // Changes the first event to the hard start event
                    clList.PerformEventToggleHardStart(_ls, _serverName, _list, ev_guid);
                    Thread.Sleep(1000);
                    Console.WriteLine("The first event has following control types after toggling:");
                    var res = clList.GetList(_ls, _serverName, _list);
                    // Print all event control types for the first event
                    var theFirst = res.ToList().First().EventControl;
                    foreach (var eventDto in theFirst)
                    {
                        Console.WriteLine("Event ID: {0} Event type: {1}", res.ToList().First().ID, eventDto);
                    }

                    // Unlocking list
                    clList.UnlockList(_ls, _serverName, _list, ClientName);
                    Console.WriteLine("ListUnlocked: {0} list {1}", _serverName, _list.ToString());
                }
                else
                {
                    Console.WriteLine("List {0} was not locked", _list.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }         
        */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked: MAIN list 2
Insert events list
The first event has following control types:
Event ID: Demo0001 Event type: AutoPlay
Event ID: Demo0001 Event type: AutoThread
Event ID: Demo0001 Event type: AutoSwitch
Thread list 2
The first event has following control types after toggling:
Event ID: Demo0001 Event type: AutoPlay
Event ID: Demo0001 Event type: AutoThread
Event ID: Demo0001 Event type: AutoSwitch
Event ID: Demo0001 Event type: AutoTimed
ListUnlocked: MAIN list 2
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void PerformEventToggleHardStart(LoginSession session, String server, Int32 list, IEnumerable<Guid> position);

        #region Description and comments
        /// <summary>
        /// Gets list of events that are scheduled on specified playlist but have not been found in assigned air devices. 
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>The list of events</returns>
        /// <example>
        /// <remarks>
        /// Device Server configuration: Demo Video Disk should have the Output Port opened and be assigned 
        /// on the second play list.
        /// </remarks>
        /// The following example demonstrates how you can use the GetPullDataList method.
        /// <code language="cs">
        /**static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Subscribe to recive notifications about list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        EventDTO[] ev_list = new EventDTO[4]
                        {
                            SimpleEvent("MissEvent1"),
                            SimpleEvent("MissEvent2"),
                            SimpleEvent("MissEvent3"),
                            SimpleEvent("MissEvent4"),
                        };

                        // Insert events
                        Console.WriteLine("Insert events list");
                        clList.InsertEventList(_ls, _serverName, _list, ev_list, InsertPlaceType.AtListEnd, Guid.Empty);

                        // Wait for inserting events
                        Thread.Sleep(500);

                        // Thread list 
                        Console.WriteLine("Thread list {0}", _list.ToString());
                        clList.PerformListThread(_ls, _serverName, _list);
                        Thread.Sleep(2000);

                        EventDTO[] res = clList.GetPullDataList(_ls, _serverName, _list);
                        Console.WriteLine("Result the GetPullDataList method: ");
                        res.ToList().ForEach(info =>
                            Console.WriteLine("Event ID : {0} GUID {1}", info.ID, info.AdcEventId)
                            );
                        Thread.Sleep(20000);
                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
         }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked notification: The list 2 on the server MAIN was locked by the Client_ADC client.
Insert events list
OnEventsAdded notification: MAIN list 2 added event : ID MissEvent1 Guid 29c1b3c8-14f7-4143-88c4-9b11526c1e8d
OnEventsAdded notification: MAIN list 2 added event : ID MissEvent2 Guid a8b5f24e-3c7a-47cd-af1d-ac8ab600eafc
OnEventsAdded notification: MAIN list 2 added event : ID MissEvent3 Guid e6f0dcb0-1d98-4d5a-9025-f99264232ac6
OnEventsAdded notification: MAIN list 2 added event : ID MissEvent4 Guid b2eabde7-5f13-4614-8bb4-232d6228579f
OnListChange notification: MAIN list 2 changeType ContentsChanged
Thread list 2
OnListChange notification: MAIN list 2 changeType ContentsChanged
Result the GetPullDataList method:
Event ID : MissEvent1 GUID 00000000-0000-0000-0000-000000000000
Event ID : MissEvent2 GUID 00000000-0000-0000-0000-000000000000
Event ID : MissEvent3 GUID 00000000-0000-0000-0000-000000000000
Event ID : MissEvent4 GUID 00000000-0000-0000-0000-000000000000
        */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        IEnumerable<EventDTO> GetPullDataList(LoginSession session, String server, Int32 list);
		
		#region Description and comments
        /// <summary>
        /// Gets the name for loading list number.
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <returns>The name for loading list number</returns>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// This method has been demonstrated in example for the <see cref="SetExtListName"/> method
        /// </example>
        #endregion
		[OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        String GetExtListName(LoginSession session, String server, Int32 list);
		
		#region Description and comments
        /// <summary>
        /// Sets playlist's associated filename on the DeviceServer
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="filename">Loading filename</param>
        /// <exception cref="System.ServiceModel.FaultException&lt;TDetail>">A client can expect to receive in the normal course of 
        /// an operation the following serializable errors objects:
        /// <para>
        /// A <see cref="ListServiceError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotRunningError"/> object.
        /// </para>
        /// <para>
        /// A <see cref="DeviceServerNotCreatedError"/> object.
        /// </para>
        /// </exception>
        /// <example>
        /// The following example demonstrates how you can use the SetExtListName method.
        /// <code language="cs"> 
        /** 
        static void Main(string[] args)
        {
            ListServiceCallback clListCB = new ListServiceCallback();
            ListServiceClient clList = new ListServiceClient(new InstanceContext(clListCB));
            
            LoginSession _ls = new LoginSession();
            String _serverName = "MAIN";
            String ClientName = "Client_ADC";
            int _list = 2;
            try
            {
                // Subscribe to recive notifications about list changes 
                subscriptionRes = clList.RegisterListListener(_ls, _serverName, _list);

                if (subscriptionRes)
                {
                    // Locking list
                    if (clList.LockList(_ls, _serverName, _list, ClientName))
                    {
                        String nameList;

                        Console.WriteLine("Get a current list name.");
                        nameList = clList.GetExtListName(_ls, _serverName, _list);
                        Console.WriteLine("Set the 'ExtListName' list name.");
                        clList.SetExtListName(_ls, _serverName, _list, "ExtListName");
                        Thread.Sleep(1000);

                        nameList = clList.GetExtListName(_ls, _serverName, _list);
                        Console.WriteLine("The new list name is: {0}", nameList);

                        // Unlocking list
                        clList.UnlockList(_ls, _serverName, _list, ClientName);
                    }
                    else
                    {
                        Console.WriteLine("List {0} was not locked", _list.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("ListListener subscription for list number {0} on {1} server is unsuccessful", _list, _serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
         }
         */
        /// </code>
        /// Console result
        /// <code language="Console">
        /**
ListLocked notification: The list 2 on the server MAIN was locked by the Client_ADC client.
Get a current list name.
Set the 'ExtListName' list name.
OnListChange notification: MAIN list 2 changeType SystemChanged
The new list name is: ExtListName
ListUnlocked notification: The list 2 on the server MAIN was unlocked by the Client_ADC client.
         */
        /// </code>
        /// </example>
        /// 
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        [FaultContract(typeof(DeviceServerNotRunningError))]
        [FaultContract(typeof(DeviceServerNotCreatedError))]
        void SetExtListName(LoginSession session, String server, Int32 list, String filename);

        #region Description and comments
        /// <summary>
        /// Returns the configured list of "Content" types, that indicate the commercial events
        /// </summary>
        /// <param name="ls">User session</param>
        /// <returns>The list of "Content" types</returns>
        #endregion
        [OperationContract]
        IEnumerable<String> GetCommercialContentInfo(LoginSession ls);

        #region Description and comments
        /// <summary>
        /// Looks for an array of IDs in a single list (presense of at least one event with each ID)
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="ids">Array of event material IDs to find</param>
        /// <returns>Array of Boolean values in the same order as the order of the IDs</returns>
        #endregion
        [OperationContract]
        IEnumerable<Boolean> CheckMaterialIds(LoginSession session, String server, Int32 list, IEnumerable<String> ids);

        #region Description and comments

        /// <summary>
        /// Get the actual configuration of BreakAway feature.
        /// </summary>

        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        BreakAwayConfigurationDTO GetBreakAwayConfiguration();

        #region Description and comments

        /// <summary>
        /// Set the current configuration of BreakAway feature.
        /// </summary>

        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        Boolean SetBreakAwayConfiguration(BreakAwayConfigurationDTO breakAwayConfig);

        #region Description and comments

        /// <summary>
        /// Get the current BreakAway status:
        /// -Has BreakAway;
        /// -In/Out BreakAway status;
        /// -Default/Sequence pathes
        /// </summary>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        BreakAwayListStatusDTO GetBreakAwayListStatus(String server,Int32 list);

        #region Description and comments

        /// <summary>
        /// Perform the Break-Away command:
        /// Performt to play the Break-Away content
        /// baList - The content of Break-Away .lst file (Break-Away list);
        /// </summary>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        void PerformBreakAway(String server, Int32 list,IEnumerable<EventDTO> baList);

        #region Description and comments

        /// <summary>
        /// Perform the Break-Away return command:
        /// Return from the Break-Away content to the
        /// main transmission list
        /// </summary>
        #endregion
        [OperationContract]
        [FaultContract(typeof(ListServiceError))]
        void PerformBreakAwayReturn(String server, Int32 list);
       
    }

    #region Description and comments
    /// <summary>
    /// The interface of List Service callbacks.
    /// <para>
    /// Use the Add Service Reference dialog box to get the List Service reference to the current solution, locally, on a local area network, or on the Internet.
    /// </para>
    /// <example>
    /// You can create your own class based on the callback class of service references. For example:
    /// <code language="cs">
    /// using Client.ListServiceReference; //include your namespace that you used for the reference
    /// 
    /// namespace Client
    /// {
    /// public class ListServiceCallback : IListServiceCallback
    /// {
    ///     public void OnListChange(string server, int list, Harris.Automation.ADC.Types.ListChangeType changeType)
    ///     {
    ///         // Do something when notifications of list changes are received. For example
    ///         Console.WriteLine("OnListChange notification: {0} list {1} changeType {2}", server, list.ToString(), changeType.ToString());
    ///     }  
    /// 
    ///     public void OnConnectionStateChange(string server, ServerStatus serverStatus)
    ///     {
    ///         // Do something when the server connection status is received. For example
    ///         Console.WriteLine("Server Name: {0} server status: {1}", server, serverStatus.ToString());
    ///     }    
    ///     
    ///     public void OnEventsUpdated(string server, int list, client.ListServiceReference.EventDTO[] updatedEvents)
    ///     {
    ///         // Do something when notifications of updated events are received. For example
    ///         for (int i = 0; i &lt; updatedEvents.Length; i++)
    ///            Console.WriteLine("OnEventsUpdated  notification: {0} list {1} updated events : ID {2} Guid {3}", 
    ///                server, list.ToString(), updatedEvents[i].ID, updatedEvents[i].AdcEventId.ToString());
    ///     }
    ///     
    ///     public void OnEventsDeleted(string server, int list, client.ListServiceReference.EventDTO[] deletedEvents)
    ///     {
    ///         // Do something when notifications of deleted events are received. For example
    ///         for (int i = 0; i &lt; deletedEvents.Length; i++)
    ///             Console.WriteLine("OnEventsDeleted notification: {0} list {1} deleted event : Guid {2}",
    ///                 server, list.ToString(), deletedEvents[i].ToString());
    ///     }
    ///     
    ///     public void OnEventsAdded(string server, int list, System.Guid afterID, EventDTO[] addedEvents)
    ///     {
    ///         // Do something when notifications of added events are received. For example
    ///          for (int i = 0; i &lt; addedEvents.Length; i++)
    ///              Console.WriteLine("OnEventsAdded notification: {0} list {1} added event : ID {2} Guid {3}",
    ///                server, list.ToString(), addedEvents[i].ID, addedEvents[i].AdcEventId.ToString());
    ///     }
    ///     
    ///     public void OnEventsMoved(string server, int list, Guid source, Guid destination, Guid[] movedEvents)
    ///     {
    ///         //Do something when notifications about moved events are received. For example
    ///         Console.WriteLine("OnEventsMoved notification: The {0} events were moved : from {1} to {2} guid, on '{3}' Server and '{4}' list.",
    ///         movedEvents.Count(), source, destination, server, list.ToString());
    ///     }
    /// 
    ///     public void ListLocked(string server, int list, string clientName)
    ///     {
    ///        Console.WriteLine("ListLocked notification: The list {0} on the server {1} was locked by the {2} client. ", list.ToString(), server, clientName);
    ///     }
    ///
    ///     public void ListUnlocked(string server, int list, string clientName)
    ///     {
    ///         Console.WriteLine("ListUnlocked notification: The list {0} on the server {1} was unlocked by the {2} client. ", list.ToString(), server, clientName);
    ///     }
    ///     
    ///     public void CheckAvailability()
    ///     {
    ///     
    ///     }
    /// }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// 
    #endregion
    public interface IListServiceClient
    {
        #region Description and comments
        /// <summary>
        /// Service notification of list change 
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="changeType">Type of list changes</param>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnListChange(String server, Int32 list, ListChangeType changeType);

        #region Description and comments
        /// <summary>
        /// Notification of connection state change 
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="serverStatus">Status of server connection</param>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnConnectionStateChange(String server, ServerStatus serverStatus);

        #region Description and comments
        /// <summary>
        /// List of updated events, Guid must be used as position 
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="updatedEvents">List of EventDTO objects</param>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnEventsUpdated(String server, Int32 list, IEnumerable<EventDTO> updatedEvents);

        #region Description and comments
        /// <summary>
        /// List of just deleted Guids 
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="deletedEvents">List of EventDTO objects</param>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnEventsDeleted(String server, Int32 list, IEnumerable<EventDTO> deletedEvents);

        #region Description and comments
        /// <summary>
        /// List of added events after specified Guid, several calls on each added portion
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="afterID">Event GUID ID in the list after which were added new events</param>
        /// <param name="addedEvents">List of EventDTO objects</param>
        /// 
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnEventsAdded(String server, Int32 list, Guid afterID, IEnumerable<EventDTO> addedEvents);

        #region Description and comments
        /// <summary>
        /// List of moved events after a specified Guid
        /// </summary>
        /// <param name="server">Device Server name</param>
        /// <param name="list">List number on Server</param>
        /// <param name="source">Guid of source position</param>
        /// <param name="destination">Guid of destination position</param>
        /// <param name="movedEvents">Collection of Guids for moved events </param>
        #endregion
        [OperationContract(IsOneWay = true)]
        void OnEventsMoved(String server, Int32 list, Guid source, Guid destination, IEnumerable<Guid> movedEvents);

        #region Description and comments

        /// <summary>
        /// Notification of locking the list 
        /// </summary>
        /// <param name="server">Server name with list locked</param>
        /// <param name="list">Number locked list</param>
        /// <param name="clientName">Identifier of the client that called LockList</param>

        #endregion
        [OperationContract(IsOneWay = true)]
        void ListLocked(String server, Int32 list, String clientName);

        #region Description and comments

        /// <summary>
        /// Notification of unlocking the list
        /// </summary>
        /// <param name="server">Server name with list unlocked</param>
        /// <param name="list">Number unlocked list</param>
        /// <param name="clientName">Identifier of the client that called UnlockList</param>

        #endregion
        [OperationContract(IsOneWay = true)]
        void ListUnlocked(String server, Int32 list, String clientName);

        #region Description and comments

        /// <summary>
        /// Notification of changing the BreakAway status
        /// </summary>

        #endregion
        [OperationContract(IsOneWay = true)]
        void OnBreakAwayListStatusChanged(String server, Int32 list, BreakAwayListStatusDTO baListStatus);


        #region Description and comments
        /// <summary>
        /// Check availability of a client
        /// </summary>
        #endregion
        [OperationContract(IsOneWay = true)]
        void CheckAvailability();
    }
}