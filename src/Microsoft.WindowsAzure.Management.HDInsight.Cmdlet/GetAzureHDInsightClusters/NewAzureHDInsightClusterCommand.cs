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
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client;
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
            var client = ServiceLocator.Instance.Locate<IHDInsightSyncClientFactory>().Create(this.SubscriptionId,
                                                                                              this.Certificate);
            var createClusterRequest = new CreateClusterRequest();
            createClusterRequest.DnsName = this.Name;
            createClusterRequest.Location = this.Location;
            createClusterRequest.DefaultAsvAccountName = this.DefaultStorageAccountName;
            createClusterRequest.DefaultAsvAccountKey = this.DefaultStorageAccountKey;
            createClusterRequest.DefaultAsvContainer = this.DefaultStorageContainerName;
            createClusterRequest.ClusterUserName = this.UserName;
            createClusterRequest.ClusterUserPassword = this.Password;
            createClusterRequest.WorkerNodeCount = this.ClusterSizeInNodes;
            createClusterRequest.AsvAccounts.AddRange(this.AdditionalStorageAccounts.Select(act => new AsvAccountConfiguration(act.StorageAccountName, act.StorageAccountKey)));
            if (this.HiveMetastore.IsNotNull())
            {
                createClusterRequest.HiveMetastore = new ComponentMetastore(this.HiveMetastore.SqlAzureServerName,
                                                                            this.HiveMetastore.DatabaseName,
                                                                            this.HiveMetastore.UserName,
                                                                            this.HiveMetastore.Password);
            }
            if (this.OozieMetastore.IsNotNull())
            {
                createClusterRequest.OozieMetastore = new ComponentMetastore(this.OozieMetastore.SqlAzureServerName,
                                                                             this.OozieMetastore.DatabaseName,
                                                                             this.OozieMetastore.UserName,
                                                                             this.OozieMetastore.Password);
            }
            this.Output.Add(new AzureHDInsightCluster(client.CreateContainer(createClusterRequest)));
        }

        public ICollection<AzureHDInsightStorageAccount> AdditionalStorageAccounts { get; private set; }
        
        public AzureHDInsightMetastore HiveMetastore { get; set; }
        
        public AzureHDInsightMetastore OozieMetastore { get; set; }
    }
}
