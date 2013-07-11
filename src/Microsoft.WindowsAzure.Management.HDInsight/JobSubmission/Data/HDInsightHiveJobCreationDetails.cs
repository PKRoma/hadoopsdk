namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides creation details for a new Hive job.
    /// </summary>
    public class HDInsightHiveJobCreationDetails : HDInsightJobCreationDetails
    {
        /// <summary>
        /// Gets or sets the query to use for a hive job.
        /// </summary>
        public string Query { get; set; }
    }
}
