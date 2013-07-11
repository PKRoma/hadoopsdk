namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    internal interface IAzureHDInsightClusterCommandBase 
    {
        /// <summary>
        /// Gets or sets the Azure Subscription to be used.
        /// </summary>
        Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the certificate File to be used.
        /// </summary>
        X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Gets or sets the EndPoint URI to use (if provided).
        /// </summary>
        Uri EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the cloud service name to use (if provided).
        /// </summary>
        string CloudServiceName { get; set; }

        /// <summary>
        /// Gets or sets the Name for the cluster to return.
        /// </summary>
        string Name { get; set; }
    }
}
