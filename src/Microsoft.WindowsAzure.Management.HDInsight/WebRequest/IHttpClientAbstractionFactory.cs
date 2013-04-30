namespace Microsoft.WindowsAzure.Management.HDInsight.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a factory for a class that Abstracts Http client requests.
    /// NOTE: This interface is intended for internal use.  It will be marked internal once a problem with mocking is resolved.
    /// </summary>
    // NEIN: This should be internal, only public now because of a moq problem
    public interface IHttpClientAbstractionFactory
    {
        /// <summary>
        /// Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <param name="cert">
        /// The X509 cert to use.
        /// </param>
        /// <returns>
        /// A new instance of the HttpClientAbstraction.
        /// </returns>
        IHttpClientAbstraction Create(X509Certificate2 cert);

        /// <summary>
        /// Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <returns>
        /// A new instance of the HttpClientAbstraction.
        /// </returns>
        IHttpClientAbstraction Create();
    }
}
