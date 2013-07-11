namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Represents a list of jobs on an HDInsight cluster.
    /// </summary>
    public class HDInsightJobList : HDInsightJobSubmissionResults
    {
        /// <summary>
        /// Initializes a new instance of the HDInsightJobList class.
        /// </summary>
        public HDInsightJobList()
        {
            this.JobIds = new List<string>();
        }

        /// <summary>
        /// Gets or sets the continuation token returned by the request (if any).
        /// </summary>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Gets the jobIds returned by the request.
        /// </summary>
        public ICollection<string> JobIds { get; private set; }
    }
}
