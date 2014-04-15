namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Manage the timing used for retry logic.
    /// </summary>
    internal interface IHttpOperationManager
    {
        /// <summary>
        /// Gets or sets the polling interval for retry attempts.
        /// </summary>
        TimeSpan RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts to perform an operation.
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the timeout for an Http Operation.
        /// </summary>
        TimeSpan HttpOperationTimeout { get; set; }
    }
}
