namespace Microsoft.WindowsAzure.Management.HDInsight.WebRequest
{
    using System;
    using System.Net;

    /// <summary>
    /// Abstracts Http client responses.
    /// NOTE: This interface is intended for internal use.  It will be marked internal once a problem with mocking is resolved.
    /// </summary>
    // NEIN: This should be internal, only public now because of a moq problem
    public interface IHttpResponseMessageAbstraction : IDisposable
    {
        /// <summary>
        /// Gets the status code for the response.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the content returned by the response.
        /// </summary>
        string Content { get; }
    }
}