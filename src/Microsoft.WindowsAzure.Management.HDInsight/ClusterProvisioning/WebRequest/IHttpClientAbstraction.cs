namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracts Http client requests.
    /// NOTE: This interface is intended for internal use.  It will be marked internal once a problem with mocking is resolved.
    /// </summary>
    // NEIN: This should be internal, only public now because of a moq problem
    public interface IHttpClientAbstraction : IDisposable
    {
        /// <summary>
        /// Gets or sets the type of Http request to make.
        /// </summary>
        HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the Request uri.
        /// </summary>
        Uri RequestUri { get; set; }

        /// <summary>
        /// Gets or sets the request content.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Gets the request headers collection.
        /// </summary>
        IDictionary<string, string> RequestHeaders { get; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the time out to use for the request.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Initiates the request and returns a task representing the request asynchronously performed.
        /// </summary>
        /// <returns>
        /// A task representing the response (once it has completed).
        /// </returns>
        Task<IHttpResponseMessageAbstraction> SendAsync();
    }
}