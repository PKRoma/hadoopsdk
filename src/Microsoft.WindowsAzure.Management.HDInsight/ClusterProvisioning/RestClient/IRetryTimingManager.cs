namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Manage the timing used for retry logic.
    /// </summary>
    internal interface IRetryTimingManager
    {
        /// <summary>
        /// Gets or sets the polling interval for retry attempts.
        /// </summary>
        TimeSpan PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the timeout for retry attempts.
        /// </summary>
        TimeSpan TimeOut { get; set; }
    }
}
