namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal interface ILocationFinderClientFactory
    {
        ILocationFinderClient Create(IConnectionCredentials creds);
    }
}
