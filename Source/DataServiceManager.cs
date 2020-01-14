using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Harris.Automation.ADC.Logger;
using Harris.Automation.ADC.Services.ListService.AdcDataService;
using Harris.Automation.ADC.Services.ListService.AsdbDataService;
using Harris.Automation.ADC.Types;
using Harris.Automation.ADC.Types.Events;

namespace Harris.Automation.ADC.Services.ListService
{
    public class DataServiceManager
    {
        private Boolean _bDisposed;
        private Boolean _stopBackground;
        private Boolean _isStopped;
        private readonly Boolean _useASDB;

        private Thread _backgroundThread;

        private ConcurrentQueue<DataServiceManagerTask> _dbTasks;
        
        public event EventHandler<EventsDBFieldsEventArgs> DataBaseFieldsRetrieved;

        public DataServiceManager()
        {
            if (!Boolean.TryParse(ConfigurationManager.AppSettings.Get("UseASDB"), out _useASDB))
                _useASDB = false;

            _dbTasks = new ConcurrentQueue<DataServiceManagerTask>();
            
            _bDisposed = false;
            _stopBackground = false;
            _isStopped = false;
            _backgroundThread = new Thread(Background);
            _backgroundThread.Start();
        }

        public void CreateTaskOnEventsAdded(String server, Int32 list, IEnumerable<EventEventIdPair> events)
        {
            _dbTasks.Enqueue(new DataServiceManagerTask(server, list, events.ToList(), TaskType.Add));
        }

        public void CreateTaskOnModifyEvent(String server, Int32 list, EventEventIdPair _event)
        {
            _dbTasks.Enqueue(new DataServiceManagerTask(server, list, new List<EventEventIdPair> { _event }, TaskType.Modify));
        }

        private void OnEventsAdded(DataServiceManagerTask task)
        {
            var count = 0;
            var ids = new List<String>();

            var resultAdc = new List<MediaInfo>();
            var resultAsdb = new List<ASDB>();
            
            foreach (var ev in task.Events)
            {
                if (ev.ADCEvent.ID != String.Empty)
                {
                    if (!ids.Contains(ev.ADCEvent.ID))
                    {
                        ids.Add(ev.ADCEvent.ID);
                        count++;
                    }
                }
                
                if (count == 100)
                {
                    if (_useASDB)
                        resultAsdb.AddRange(Execute(GetSearchSimpleExpressionASDB(ids)).ToList());
                    else
                        resultAdc.AddRange(Execute(GetSearchSimpleExpressionADC(ids)).ToList());
                    
                    ids.Clear();
                    count = 0;
                }

                if (_stopBackground)
                    return;
            }

            if (_stopBackground)
                return;

            if (count > 0)
            {
                if (_useASDB)
                {
                    resultAsdb.AddRange(Execute(GetSearchSimpleExpressionASDB(ids)).ToList());
                }
                else
                {
                    resultAdc.AddRange(Execute(GetSearchSimpleExpressionADC(ids)).ToList());
                }

                ids.Clear();
            }

            // Set database fields (e.g. content)
            var fields = new List<EventsDatabaseFields>();
            foreach (var ev in task.Events)
            {
                if (ev.ADCEvent.ID != String.Empty)
                {
                    ev.Content = _useASDB
                                     ? GetContentFromDBResultAsdb(resultAsdb, ev)
                                     : GetContentFromDBResultAdc(resultAdc, ev);
                   fields.Add(new EventsDatabaseFields(ev.ADCEventId, ev.Content));
                }
            }

            if (DataBaseFieldsRetrieved != null)
            {
                
                DataBaseFieldsRetrieved(this,
                                        new EventsDBFieldsEventArgs(task.Server, task.List, fields));
            }
        }

        private void OnEventModified(DataServiceManagerTask task)
        {
            var evt = task.Events.FirstOrDefault();
            if (evt != null)
            {
                if (evt.ADCEvent.ID != String.Empty)
                {
                    if (_useASDB)
                    {
                        Expression<Func<ASDB, Boolean>> exp = info => info.Identifier == evt.ADCEvent.ID;
                        var result = Execute(exp).ToList();
                        evt.Content = GetContentFromDBResultAsdb(result, evt);
                    }
                    else
                    {
                        Expression<Func<MediaInfo, Boolean>> exp = info => info.Identifier == evt.ADCEvent.ID;
                        var result = Execute(exp).ToList();
                        evt.Content = GetContentFromDBResultAdc(result, evt);
                    }

                    if (DataBaseFieldsRetrieved != null)
                    {
                        DataBaseFieldsRetrieved(this,
                                                new EventsDBFieldsEventArgs(task.Server, task.List,
                                                                            task.Events.Select(
                                                                                info =>
                                                                                new EventsDatabaseFields(
                                                                                    info.ADCEventId,
                                                                                    info.Content))));
                    }
                }
            }
        }

        public string GetContentFromDBResultAdc(List<MediaInfo> list, EventEventIdPair ev)
        {
            var records = list.Where(info => info.Identifier == ev.ADCEvent.ID).ToList();
            if (!records.Any())
            {
                return String.Empty;
            }

            if (records.Count() == 1)
            {
                return records.First().ContentTable.Content ?? String.Empty;
            }

            // collision
            if (records.Count() >= 2)
            {
                MediaInfo record;
                if (ev.ADCEvent.SegmentNumber != 255)
                {
                    record = records.FirstOrDefault(info => info.SegmentInfos.Count > 0);
                    if (record != null)
                        return record.ContentTable.Content ?? String.Empty;
                }

                // resolve collision
                record = records.FirstOrDefault(
                    info =>
                    info.Title == ev.ADCEvent.Title && info.Duration == TimeCode.BCDToInt32(ev.ADCEvent.Duration) &&
                    info.StartOfMedia == TimeCode.BCDToInt32(ev.ADCEvent.SOM));
                
                if (record != null)
                    return record.ContentTable.Content ?? String.Empty;
                
                record = records.FirstOrDefault(info => info.SegmentInfos.Count == 0);
                if (record != null)
                    return record.ContentTable.Content ?? String.Empty;

                return records.First().ContentTable.Content ?? String.Empty;
            }

            return String.Empty;
        }

        public string GetContentFromDBResultAsdb(List<ASDB> list, EventEventIdPair ev)
        {
            var records = list.Where(info => info.Identifier == ev.ADCEvent.ID).ToList();
            if (!records.Any())
            {
                return String.Empty;
            }

            if (records.Count() == 1)
            {
                return records.First().Content ?? String.Empty;
            }

            // collision
            if (records.Count() >= 2)
            {
                ASDB record;
                if (ev.ADCEvent.SegmentNumber != 255)
                {
                    record = records.FirstOrDefault(info => info.Type == "m" || info.Type == "M");
                    if (record != null)
                        return record.Content ?? String.Empty;
                    
                }

                // resolve collision
                record = records.FirstOrDefault(
                    info =>
                    info.Title == ev.ADCEvent.Title && info.Duration == TimeCode.BCDToInt32(ev.ADCEvent.Duration) &&
                    info.StartOfMessage == TimeCode.BCDToInt32(ev.ADCEvent.SOM));
                if (record != null)
                    return record.Content ?? String.Empty;

                record = records.FirstOrDefault(info => info.Type == "s" || info.Type == "S");
                if (record != null)
                    return record.Content ?? String.Empty;

                return records.First().Content ?? String.Empty;
            }

            return String.Empty;
        }

        private void Background()
        {
            while (!_stopBackground)
            {
                while (!_dbTasks.IsEmpty)
                {
                    DataServiceManagerTask task;
                    while (!_dbTasks.TryDequeue(out task))
                    {
                        Thread.Sleep(10);
                    }
                    switch (task.Type)
                    {
                        case TaskType.Add:
                            {
                                OnEventsAdded(task);
                                break;
                            }

                        case TaskType.Modify:
                            {
                                OnEventModified(task);
                                break;
                            }
                    }

                }
                Thread.Sleep(100);
            }
            _isStopped = true;
        }

        ~DataServiceManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!_bDisposed)
            {
                _stopBackground = true;
                while (!_isStopped)
                    Thread.Sleep(100);
                if (null != _backgroundThread)
                {
                    _backgroundThread.Interrupt();
                    _backgroundThread.Join();
                    _backgroundThread = null;
                }

                // Unmanaged resources
                if (disposing)
                {
                    GC.Collect();
                }
                _bDisposed = true;
            }
        }

        private IEnumerable<MediaInfo> Execute(Expression<Func<MediaInfo, Boolean>> expression)
        {
            var adcUri = new Uri(
                Config.Instance.ConfigObject.DataServiceConnectionOptions.DataServiceAddress);
            var adcDB = new AdcModelContainer(adcUri);

            var query = adcDB.MediaInfos.Expand(@"ContentTable").Expand(@"SegmentInfos").Where(expression) as DataServiceQuery<MediaInfo>;
            
            var result = new List<MediaInfo>();
            if (query != null)
            {
                try
                {
                    var response = (QueryOperationResponse<MediaInfo>) query.Execute();
                    while (response != null)
                    {
                        var part = new List<MediaInfo>();
                        part.AddRange(response);
                        result.AddRange(part);
                        var continuation = response.GetContinuation();
                        response = continuation != null ? adcDB.Execute(continuation) : null;
                    }
                }
                catch (Exception ex)
                {
                    ServiceLogger.Error(ex.Message, ex);
                    return result;
                }
            }
            return result;
        }

        private IEnumerable<ASDB> Execute(Expression<Func<ASDB, Boolean>> expression)
        {
            var asdbUri = ConfigurationManager.AppSettings.Get("AsdbUri");
            var asdbDB = new ASDBModelContainer(new Uri(asdbUri));

            var query = asdbDB.ASDBs.Where(expression) as DataServiceQuery<ASDB>;

            var result = new List<ASDB>();
            if (query != null)
            {
                try
                {
                    var response = (QueryOperationResponse<ASDB>)query.Execute();
                    while (response != null)
                    {
                        var part = new List<ASDB>();
                        part.AddRange(response);
                        result.AddRange(part);
                        var continuation = response.GetContinuation();
                        response = continuation != null ? asdbDB.Execute(continuation) : null;
                    }
                }
                catch (Exception ex)
                {
                    ServiceLogger.Error(ex.Message, ex);
                    return result;
                }
            }
            return result;
        }

        public List<Expression> SimpleTree(List<Expression> exprList)
        {
            var newList = new List<Expression>();
            if (exprList.Count == 1)
            {
                newList.Add(exprList.First());
                return exprList;
            }

            for (var i = 0; i < exprList.Count; i++)
            {
                var item1 = exprList[i++];
                if (i < exprList.Count)
                {
                    var item2 = exprList[i];
                    newList.Add(Expression.Or(item1, item2));
                }
                else
                {
                    newList.Add(item1);
                }
            }

            return SimpleTree(newList);
        }

        public Expression<Func<ASDB, Boolean>> GetSearchSimpleExpressionASDB(List<String> ids)
        {
            Expression result;
            {
                var exprList = ids.Select(EqualExpressionASDB).ToList();
                result = SimpleTree(exprList).First();
            }
            return GetLambdaASDB(result, Expression.Parameter(typeof(ASDB), "ASDB"));
        }

        private Expression<Func<ASDB, Boolean>> GetLambdaASDB(Expression expr, ParameterExpression lambda)
        {
            if (expr != null)
            {
                return Expression.Lambda<Func<ASDB, Boolean>>(expr, lambda);
            }
            return NothingASDB();
        }

        private static Expression<Func<ASDB, bool>> NothingASDB()
        {
            return f => false;
        }

        public Expression EqualExpressionASDB(String identifier)
        {
            Expression result = null;
            var propertyInfo = typeof(ASDB).GetProperty("Identifier");
            var lambdaParameter = Expression.Parameter(typeof(ASDB), "ASDB");

            if (propertyInfo != null)
            {
                var valueExpression = Expression.Constant(identifier, typeof(String));
                var propertyExpression = Expression.Property(lambdaParameter, propertyInfo);
                result = Expression.Equal(propertyExpression, valueExpression);
            }

            return result;
        }

        public Expression<Func<MediaInfo, Boolean>> GetSearchSimpleExpressionADC(List<String> ids)
        {
            Expression result;
            {
                var exprList = ids.Select(EqualExpression).ToList();
                result = SimpleTree(exprList).First();
            }
            return GetLambdaADC(result, Expression.Parameter(typeof(MediaInfo), "mediaInfo"));
        }

        private Expression<Func<MediaInfo, Boolean>> GetLambdaADC(Expression expr, ParameterExpression lambda)
        {
            if (expr != null)
            {
                return Expression.Lambda<Func<MediaInfo, Boolean>>(expr, lambda);
            }
            return NothingADC();
        }

        private static Expression<Func<MediaInfo, bool>> NothingADC()
        {
            return f => false;
        }

        public Expression EqualExpression(String identifier)
        {
            Expression result = null;
            var propertyInfo = typeof(MediaInfo).GetProperty("Identifier");
            var lambdaParameter = Expression.Parameter(typeof(MediaInfo), "mediaInfo");

            if (propertyInfo != null)
            {
                var valueExpression = Expression.Constant(identifier, typeof(String));
                var propertyExpression = Expression.Property(lambdaParameter, propertyInfo);
                result = Expression.Equal(propertyExpression, valueExpression);
            }

            return result;
        }
    }



    internal class DataServiceManagerTask
    {
        public String Server;
        public Int32 List;
        public List<EventEventIdPair> Events;
        public TaskType Type;

        public DataServiceManagerTask(String server, Int32 list, List<EventEventIdPair> events, TaskType type)
        {
            Server = server;
            List = list;
            Events = events;
            Type = type;
        }
    }

    internal enum TaskType
    {
        Modify,
        Update,
        Delete,
        Add
    }
}
