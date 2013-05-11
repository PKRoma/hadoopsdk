namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;

    /// <summary>
    /// Represents an Azure HD Insight Cluster for the PowerShell Cmdlets.
    /// </summary>
    public class AzureHDInsightCluster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureHDInsightCluster"/> class.
        /// </summary>
        /// <param name="cluster">
        /// The underlying SDK data object representing the cluster.
        /// </param>
        public AzureHDInsightCluster(HDInsightCluster cluster)
        {
            this.cluster = cluster;
        }

        private HDInsightCluster cluster;
        
        /// <summary>
        /// Gets the name of the Azure HD Insight Cluster.
        /// </summary>
        public string Name
        {
            get { return this.cluster.Name; }
        }

        /// <summary>
        /// Gets the connection Url for the Azure HD Insight Cluster.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "this is a read only property coming from the server.  It is safer to leave as a string. [tgs]")]
        public string ConnectionUrl
        {
            get { return this.cluster.ConnectionUrl; }
        }

        /// <summary>
        /// Gets the ClusterState for the Azure HD Insight Cluster.
        /// </summary>
        public ClusterState State
        {
            get { return this.cluster.State; }
        }

        /// <summary>
        /// Gets the Date the Azure HD Insight Cluster was created.
        /// </summary>
        public DateTime CreateDate
        {
            get { return this.cluster.CreatedDate; }
        }

        /// <summary>
        /// Gets the username used when creating the Azure HD Insight Cluster.
        /// </summary>
        public string UserName
        {
            get { return this.cluster.UserName; }
        }

        /// <summary>
        /// Gets the Azure location where the Azure HD Insight Cluster is located.
        /// </summary>
        public string Location
        {
            get { return this.cluster.UserName; }
        }

        /// <summary>
        /// Gets the size of the Azure HD Insight cluster in units of worker nodes.
        /// </summary>
        public int ClusterSizeInNodes
        {
            get { return this.cluster.ClusterSizeInNodes; }
        }
    }
}
