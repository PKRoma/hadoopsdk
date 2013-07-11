namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class NewAzureHDInsightClusterCommand : AzureHDInsightClusterCommand<AzureHDInsightCluster>, INewAzureHDInsightClusterCommand
    {
        public NewAzureHDInsightClusterCommand()
        {
            this.AdditionalStorageAccounts = new List<AzureHDInsightStorageAccount>();
        }

        /// <inheritdoc />
        public string Location { get; set; }

        /// <inheritdoc />
        public string DefaultStorageAccountName { get; set; }

        /// <inheritdoc />
        public string DefaultStorageAccountKey { get; set; }

        /// <inheritdoc />
        public string DefaultStorageContainerName { get; set; }

        /// <inheritdoc />
        public string UserName { get; set; }

        /// <inheritdoc />
        public string Password { get; set; }

        /// <inheritdoc />
        public int ClusterSizeInNodes { get; set; }

        public override void EndProcessing()
        {
            var client = this.GetClient();

            var createClusterRequest = new HDInsightClusterCreationDetails();
            createClusterRequest.Name = this.Name;
            createClusterRequest.Location = this.Location;
            createClusterRequest.DefaultStorageAccountName = this.DefaultStorageAccountName;
            createClusterRequest.DefaultStorageAccountKey = this.DefaultStorageAccountKey;
            createClusterRequest.DefaultStorageContainer = this.DefaultStorageContainerName;
            createClusterRequest.UserName = this.UserName;
            createClusterRequest.Password = this.Password;
            createClusterRequest.ClusterSizeInNodes = this.ClusterSizeInNodes;
            createClusterRequest.AdditionalStorageAccounts.AddRange(this.AdditionalStorageAccounts.Select(act => new StorageAccountConfiguration(act.StorageAccountName, act.StorageAccountKey)));
            if (this.HiveMetastore.IsNotNull())
            {
                createClusterRequest.HiveMetastore = new HDInsightMetastore(this.HiveMetastore.SqlAzureServerName,
                                                                            this.HiveMetastore.DatabaseName,
                                                                            this.HiveMetastore.UserName,
                                                                            this.HiveMetastore.Password);
            }
            if (this.OozieMetastore.IsNotNull())
            {
                createClusterRequest.OozieMetastore = new HDInsightMetastore(this.OozieMetastore.SqlAzureServerName,
                                                                             this.OozieMetastore.DatabaseName,
                                                                             this.OozieMetastore.UserName,
                                                                             this.OozieMetastore.Password);
            }
            this.Output.Add(new AzureHDInsightCluster(client.CreateCluster(createClusterRequest)));
        }

        public ICollection<AzureHDInsightStorageAccount> AdditionalStorageAccounts { get; private set; }
        
        public AzureHDInsightMetastore HiveMetastore { get; set; }
        
        public AzureHDInsightMetastore OozieMetastore { get; set; }
    }
}
