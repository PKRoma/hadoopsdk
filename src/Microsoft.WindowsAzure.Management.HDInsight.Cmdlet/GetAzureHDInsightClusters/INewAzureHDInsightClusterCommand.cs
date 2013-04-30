namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;

    /// <summary>
    /// Worker object for creating a cluster via PowerShell.
    /// </summary>
    internal interface INewAzureHDInsightClusterCommand : IAzureHDInsightCommand<AzureHDInsightCluster>, INewAzureHDInsightClusterBase
    {
        ICollection<AzureHDInsightStorageAccount> AdditionalStorageAccounts { get; }

        AzureHDInsightMetastore HiveMetastore { get; set; }

        AzureHDInsightMetastore OozieMetastore { get; set; }
    }
}
