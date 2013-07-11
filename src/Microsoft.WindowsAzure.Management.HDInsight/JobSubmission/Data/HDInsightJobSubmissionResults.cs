namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Provides the base class for results returned from 
    /// a job submission request against an HDInsight cluster.
    /// </summary>
    public abstract class HDInsightJobSubmissionResults
    {
        /// <summary>
        /// Gets or sets the HttpStatusCode returned by the request (via the gateway).
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the error code returned by the request (via the gateway).
        /// </summary>
        public string ErrorCode { get; set; }

    }
}
