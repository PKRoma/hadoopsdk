namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Net;

    /// <summary>
    /// Represents the detail results of an HDInsightJob.
    /// </summary>
    public class HDInsightJob : HDInsightJobSubmissionResults
    {
        /// <summary>
        /// Gets or sets the JobId returned by the request .
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets the exit code for the job.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Gets or sets the error output path for the job.
        /// </summary>
        public string ErrorOutputPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the logical output path for the job results.
        /// </summary>
        public string LogicalOutputPath { get; set; }

        /// <summary>
        /// Gets or sets the physical output path for the job results.
        /// </summary>
        public string PhysicalOutputPath { get; set; }

        /// <summary>
        /// Gets or sets the query for the job (if it was a hive job).
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the status code for the job.
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the time the job was submitted.
        /// </summary>
        public DateTime SubmissionTime { get; set; }
    }
}
