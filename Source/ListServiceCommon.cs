using System;
using Harris.Automation.ADC.Services.Common;


namespace Harris.Automation.ADC.Services.ListService
{
    public static class ApplicationInfo
    {
        // This guid is used in the license policy mapping in the Security Service
        public readonly static Guid ApplicationGuid = new Guid("deleted");
    }

    /// <summary>
    /// Represents the error thrown if the Play List is not enabled on Device Server.
    /// </summary>
    [Serializable]
    public class ListServiceListNotEnabledException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListNotEnabledException"/> class.
        /// </summary>
        public ListServiceListNotEnabledException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListNotEnabledException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ListServiceListNotEnabledException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListNotEnabledException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">An instance of Exception that describes the error that caused the current exception.</param>
        public ListServiceListNotEnabledException(String message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListNotEnabledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ListServiceListNotEnabledException(System.Runtime.Serialization.SerializationInfo info,
                                       System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns formatted message
        /// </summary>
        /// <param name="server">Device server name</param>
        /// <param name="list">List number</param>
        /// <returns>Returns message string in format 'Server '|SERVER_NAME|' is not Running'</returns>
        public static String GetFormattedMessage(String server, Int32 list)
        {
            return String.Format("List #{0} is not enabled on Device Server '{1}'", list, server);
        }
    }

    /// <summary>
    /// Exception that indicates an error occurred processing the events.
    /// </summary>
    [Serializable]
    public class ListServiceEventProcessingException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceEventProcessingException"/> class.
        /// </summary>
        public ListServiceEventProcessingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceEventProcessingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ListServiceEventProcessingException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceEventProcessingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">An instance of Exception that describes the error that caused the current exception.</param>
        public ListServiceEventProcessingException(String message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceEventProcessingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ListServiceEventProcessingException(System.Runtime.Serialization.SerializationInfo info,
                                       System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Represents the error thrown if the Play List is locked by other client.
    /// </summary>
    [Serializable]
    public class ListServiceListLockedException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListLockedException"/> class.
        /// </summary>
        public ListServiceListLockedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListLockedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ListServiceListLockedException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListLockedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">An instance of Exception that describes the error that caused the current exception.</param>
        public ListServiceListLockedException(String message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceListLockedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ListServiceListLockedException(System.Runtime.Serialization.SerializationInfo info,
                                       System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
    /// <summary>
    /// Represents the error thrown if the Break-Away operation failed.
    /// </summary>
    [Serializable]
    public class BreakAwayOperationFailedException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreakAwayOperationFailedException"/> class.
        /// </summary>
        public BreakAwayOperationFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreakAwayOperationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BreakAwayOperationFailedException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreakAwayOperationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">An instance of Exception that describes the error that caused the current exception.</param>
        public BreakAwayOperationFailedException(String message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreakAwayOperationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BreakAwayOperationFailedException(System.Runtime.Serialization.SerializationInfo info,
                                       System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}