namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides creation details for a new MapReduce job.
    /// </summary>
    public class HDInsightMapReduceJobCreationDetails : HDInsightJobCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the HDInsightMapReduceJobCreationDetails class.
        /// </summary>
        public HDInsightMapReduceJobCreationDetails()
        {
            this.Arguments = new Collection<string>();
        }

        /// <summary>
        /// Gets or sets the jar file to use for the job.
        /// </summary>
        public string JarFile { get; set; }

        /// <summary>
        /// Gets or sets the application name to use for the job.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the arguments for the job.
        /// </summary>
        public ICollection<string> Arguments { get; private set; }
    }
}
