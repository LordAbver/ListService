using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Harris.Automation.ADC.Services.Common;
using Harris.Automation.ADC.Services.Common.DataTransferObjects.Events;
using Harris.Automation.ADC.Logger;
using Harris.Automation.ADC.Services.Common.Source.DataTransferObjects.BreakAway;
using Harris.Automation.ADC.Types;


namespace Harris.Automation.ADC.Services.ListService
{
    public sealed class AsyncListServiceClient : AsyncServiceClient, IListServiceClient
    {
        #region Private Fields

        private IListServiceClient _callback;

        private List<Channel> _channels;
        private object _channelsLocker = new Object();
        private List<String> _servers;
        private object _serversLocker = new Object();
        private List<Channel> _listLocks;
        private object _listLocksLocker = new Object();
        private bool _isDisposed;

        #endregion

        #region Construction

        public AsyncListServiceClient(IListServiceClient callback)
        {
            _callback = callback;
            _channels = new List<Channel>();
            _servers = new List<string>();
            _listLocks = new List<Channel>();
            _isDisposed = false;
        }

        ~AsyncListServiceClient()
        {
            this.Dispose(false);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(Boolean disposing)
        {
            if (!_isDisposed)
            {
                // Free the Unmanaged Resources
                if (disposing)
                {
                    _callback = null;
                    _channels.Clear();
                    _channels = null;
                    _channelsLocker = null;
                    _listLocks.Clear();
                    _listLocks = null;
                    _listLocksLocker = null;
                    _servers.Clear();
                    _servers = null;
                    _serversLocker = null;
                    GC.Collect();
                }
                base.Dispose(disposing);
                _isDisposed = true;
            }
        }

        #endregion

        #region Public Methods

        public void OnListChange(string server, int list, ListChangeType changeType)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnListChange(server, list, changeType));
                }
            }
        }

        public void OnConnectionStateChange(string server, ServerStatus serverStatus)
        {
            lock (_serversLocker)
            {
                if (_servers.Contains(server))
                {
                    WrapWcfCallback(() => _callback.OnConnectionStateChange(server, serverStatus));
                }
            }
        }

        public void OnEventsUpdated(string server, int list, IEnumerable<EventDTO> updatedEvents)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnEventsUpdated(server, list, updatedEvents));
                }
            }
        }

        public void OnEventsDeleted(string server, int list, IEnumerable<EventDTO> deletedEvents)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnEventsDeleted(server, list, deletedEvents));
                }
            }
        }

        public void OnEventsAdded(string server, int list, Guid afterID, IEnumerable<EventDTO> addedEvents)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnEventsAdded(server, list, afterID, addedEvents));
                }
            }
        }

        public void OnEventsMoved(String server, Int32 list, Guid source, Guid destination, IEnumerable<Guid> movedEvents)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnEventsMoved(server, list, source, destination, movedEvents));
                }
            }
        }

        public void ListLocked(String server, Int32 list, String clientName)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.ListLocked(server, list, clientName));
                }
            }
        }

        public void ListUnlocked(string server, int list, string clientName)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.ListUnlocked(server, list, clientName));
                }
            }
        }

        public void OnBreakAwayListStatusChanged(string server,int list,BreakAwayListStatusDTO breakAwayListStatus)
        {
            lock (_channelsLocker)
            {
                if (_channels.Contains(new Channel(server, list)))
                {
                    WrapWcfCallback(() => _callback.OnBreakAwayListStatusChanged(server, list,breakAwayListStatus));
                }
            }
        }

        public void CheckAvailability()
        {
            WrapWcfCallback(() => _callback.CheckAvailability());
        }

        public Boolean HasCallbacks()
        {
            Int32 tmp;
            lock (_channelsLocker)
            {
                lock (_servers)
                {
                    lock (_listLocksLocker)
                    {
                        tmp = _channels.Count + _servers.Count + _listLocks.Count;
                    }
                }
            }
            return tmp != 0;
        }

        public void AddChannel(Channel channel)
        {
            lock (_channelsLocker)
            {
                if (!_channels.Contains(channel))
                {
                    _channels.Add(channel);
                }
            }
        }

        public void RemoveChannel(Channel channel)
        {
            lock (_channelsLocker)
            {
                _channels.Remove(channel);
            }
        }

        public void RemoveAllChannels(String server)
        {
            lock (_channelsLocker)
            {
                _channels.RemoveAll(info => info.Server == server);
            }
        }

        public void AddServer(String server)
        {
            lock (_serversLocker)
            {
                if (!_servers.Contains(server))
                {
                    _servers.Add(server);
                }
            }
        }

        public void RemoveServer(String server)
        {
            lock (_serversLocker)
            {
                _servers.Remove(server);
            }
        }

        public void RemoveAllServers()
        {
            lock (_serversLocker)
            {
                _servers.Clear();
            }
        }

        public Boolean IsCurrentCallback(IListServiceClient callback)
        {
            return callback.Equals(this._callback);
        }

        public void LockList(Channel channel, String clientName)
        {
            this._clientName = clientName;
            lock (_listLocksLocker)
            {
                if (!_listLocks.Contains(channel))
                {
                    _listLocks.Add(channel);
                }
                ServiceLogger.Informational(String.Format(@"List #{0} on server {1} has been locked by {2}", channel.List, channel.Server, GetClientName()));
            }
        }

        public void UnlockList(Channel channel)
        {
            lock (_listLocksLocker)
            {
                _listLocks.Remove(channel);
                ServiceLogger.Informational(String.Format(@"List #{0} on server {1} has been unlocked by {2}", channel.List, channel.Server, GetClientName()));
            }
        }

        public bool IsListLockedBy(Channel channel)
        {
            Boolean result;
            lock (_listLocksLocker)
            {
                result = _listLocks.Exists(ch => ch == channel);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AsyncListServiceClient)) return false;
            return Equals((AsyncListServiceClient)obj);
        }

        public bool Equals(AsyncListServiceClient other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._channels.SequenceEqual(this._channels) && other._servers.SequenceEqual(this._servers) && Equals(other._callback, this._callback);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (this._channels != null ? this._channels.GetHashCode() : 0);
                result = (result * 397) ^ (this._servers != null ? this._servers.GetHashCode() : 0);
                result = (result * 397) ^ (this._callback != null ? this._callback.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(AsyncListServiceClient left, AsyncListServiceClient right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AsyncListServiceClient left, AsyncListServiceClient right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    public class ListServiceCallbackInfo
    {
        protected Channel _channel;

        protected ListServiceCallbackInfo(Channel channel)
        {
            _channel = channel;
        }

        public virtual void ExecuteCallback(IListServiceClient client)
        {

        }
    }
}