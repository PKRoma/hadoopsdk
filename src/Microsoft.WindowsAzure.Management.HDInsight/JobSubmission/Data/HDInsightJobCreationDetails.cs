namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides the details of an HDInsight job when creating the job.
    /// </summary>
    public abstract class HDInsightJobCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the HDInsightJobCreationDetails class.
        /// </summary>
        protected HDInsightJobCreationDetails()
        {
            this.Parameters = new Dictionary<string, string>();
            this.Resources = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets the parameters for the job.
        /// </summary>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets the resources for the job.
        /// </summary>
        public IDictionary<string, string> Resources { get; private set; }

        /// <summary>
        /// Gets or sets the output location to use for the job.
        /// </summary>
        public string OutputStorageLocation { get; set; }
    }
}
