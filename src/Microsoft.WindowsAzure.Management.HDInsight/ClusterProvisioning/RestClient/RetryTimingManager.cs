namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Manage the timing used for retry logic.
    /// </summary>
    internal class RetryTimingManager : IRetryTimingManager
    {
        /// <summary>
        /// Initializes a new instance of the RetryTimingManager class.
        /// </summary>
        public RetryTimingManager()
        {
            this.TimeOut = TimeSpan.FromMinutes(3);
            this.PollInterval = TimeSpan.FromSeconds(15);
        }

        /// <inheritdoc />
        public TimeSpan PollInterval { get; set; }

        /// <inheritdoc />
        public TimeSpan TimeOut { get; set; }
    }
}
