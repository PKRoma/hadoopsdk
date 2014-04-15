namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Manage the timing used for retry logic.
    /// </summary>
    internal class HttpOperationManager : IHttpOperationManager
    {
        /// <summary>
        /// Initializes a new instance of the HttpOperationManager class.
        /// </summary>
        public HttpOperationManager()
        {
            // Retry up to 40 times.
            this.RetryCount = 40;

            // 15 seconds between calls.
            this.RetryInterval = TimeSpan.FromSeconds(15);

            // 2:30 Minutes for call timeouts
            this.HttpOperationTimeout = TimeSpan.FromSeconds(150);
        }

        /// <inheritdoc />
        public TimeSpan RetryInterval { get; set; }

        /// <inheritdoc />
        public int RetryCount { get; set; }

        /// <inheritdoc />
        public TimeSpan HttpOperationTimeout { get; set; }
    }
}
