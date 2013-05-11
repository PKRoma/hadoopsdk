namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning
{
    using System;

    /// <summary>
    /// Provides the base interface for ClusterProvisioning Clients.
    /// </summary>
    public interface IClusterProvisioningClientBase
    {
        /// <summary>
        /// Gets or sets the polling interval for the CreateCluster\DeleteCluster operations.
        /// </summary>
        TimeSpan PollingInterval { get; set; }
    }
}
