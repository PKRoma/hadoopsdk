namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    internal interface INewAzureHDInsightClusterBase : IRemoveAzureHDInsightClusterBase
    {
        /// <summary>
        /// Gets or sets the Asv Account name to use for the cluster's default container.
        /// </summary>
        string DefaultStorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the Asv Account key to use for the cluster's default container.
        /// </summary>
        string DefaultStorageAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the container to use for the cluster's default container.
        /// </summary>
        string DefaultStorageContainerName { get; set; }

        /// <summary>
        /// Gets or sets the user name to use for the HDInsight cluster.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password to use for the cluster.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the number of data (worker) nodes to use in the cluster.
        /// </summary>
        int ClusterSizeInNodes { get; set; }
    }
}
