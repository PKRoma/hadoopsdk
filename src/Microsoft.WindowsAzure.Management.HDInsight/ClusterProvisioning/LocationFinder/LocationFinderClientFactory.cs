namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class LocationFinderClientFactory : ILocationFinderClientFactory
    {
        public ILocationFinderClient Create(IConnectionCredentials creds)
        {
            return new LocationFinderClient(creds);
        }
    }
}
