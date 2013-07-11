namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides the results from a job creation request.
    /// </summary>
    public class HDInsightJobCreationResults : HDInsightJobSubmissionResults
    {
        /// <summary>
        /// Gets or sets the JobId for the job that was created.
        /// </summary>
        public string JobId { get; set; }
    }
}
