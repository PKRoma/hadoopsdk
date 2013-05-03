namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that will be raise either when there is an exception on 
    /// the Rest client or when the results != OK.
    /// </summary>
    [Serializable]
    public class HDInsightRestClientException : Exception
    {
        /// <summary>
        /// Gets the StatusCode of the request.
        /// </summary>
        public HttpStatusCode RequestStatusCode { get; private set; }

        /// <summary>
        /// Gets error details of the exception.
        /// Only used per inharitance request.
        /// </summary>
        public string RequestContent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HDInsightRestClientException class.
        /// </summary>
        public HDInsightRestClientException() :
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HDInsightRestClientException class.
        /// </summary>
        /// <param name="message">Message for the base class.</param>
        public HDInsightRestClientException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HDInsightRestClientException class.
        /// </summary>
        /// <param name="message">Message for the base class.</param>
        /// <param name="innerException">Inner Exception.</param>
        public HDInsightRestClientException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HDInsightRestClientException class.
        /// </summary>
        /// <param name="statusCode">Status code recieved from the failed request.</param>
        /// <param name="content">Content of the failed request.</param>
        internal HDInsightRestClientException(System.Net.HttpStatusCode statusCode, string content) :
            base(string.Format(
                                CultureInfo.InvariantCulture,
                                "Request failed with code:{0}\r\nContent:{1}",
                                 statusCode,
                                 content ?? "(null)"))
        {
            this.RequestStatusCode = statusCode;
            this.RequestContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the HDInsightRestClientException class from deserialization.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected HDInsightRestClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.RequestStatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), info.GetString("RequestStatusCode"));
            this.RequestContent = info.GetString("RequestContent");
        }

        /// <summary>
        /// Serializes this object.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        public new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RequestStatusCode", this.RequestStatusCode);
            info.AddValue("RequestContent", this.RequestContent);
        }
    }
}