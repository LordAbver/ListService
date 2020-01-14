using System;
using System.Runtime.Serialization;
using Harris.Automation.ADC.Types.ServiceErrors;

namespace Harris.Automation.ADC.Services.ListService
{
    /// <summary>
    /// The error indicates that List is not enabled on Device Server.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ListNotEnabledError))]
    [KnownType(typeof(ListListenerError))]
    [KnownType(typeof(EventError))]
    [KnownType(typeof(ListLockedError))]
    public class ListServiceError 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceError"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
      public ListServiceError(String source, String message)
        {
            Source = source;
            Message = message;
        }

      /// <summary>
      /// Gets the source of error.
      /// </summary>
      /// <value>
      /// The source of error.
      /// </value>
      [DataMember]
      public string Source { get; private set; }

      /// <summary>
      /// Gets the message.
      /// </summary>
      /// <value>
      /// The message.
      /// </value>
      [DataMember]
      public string Message { get; private set; }
    }

    [DataContract]
    public class ListNotEnabledError : ListServiceError
    {
        public ListNotEnabledError(string source, string message)
            : base(source, message)
        {
        }
    }

    [DataContract]
    public class ListListenerError : ListServiceError
    {
        public ListListenerError(string source, string message) 
            : base(source, message)
        {
        }
    }

    [DataContract]
    public class EventError : ListServiceError
    {
        public EventError(string source, string message) 
            : base(source, message)
        {
        }
    }

    /// <summary>
    /// The error indicates that List on server is locked by another client.
    /// </summary>
    [DataContract]
    public class ListLockedError : ListServiceError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLockedError"/> class.
        /// </summary>
        /// <param name="source">The error source.</param>
        /// <param name="message">The message.</param>
        public ListLockedError(string source, string message)
            :base(source, message)
        {
        }
    }
}