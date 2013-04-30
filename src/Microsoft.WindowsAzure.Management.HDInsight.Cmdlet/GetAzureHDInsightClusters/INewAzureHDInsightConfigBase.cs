namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface INewAzureHDInsightConfigBase
    {
        /// <summary>
        /// Gets or sets the size of the cluster in data nodes.
        /// </summary>
        int ClusterSizeInNodes { get; set; }
    }
}
