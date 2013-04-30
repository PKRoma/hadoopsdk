namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IRemoveAzureHDInsightClusterBase : IGetAzureHDInsightClusterBase
    {
        /// <summary>
        /// Gets or sets the Azure location for the HDInsight cluster.
        /// </summary>
        string Location { get; set; }
    }
}
