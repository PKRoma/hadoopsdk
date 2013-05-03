namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class HDInsightManagementRestClientFactory : IHDInsightManagementRestClientFactory
    {
        public IHDInsightManagementRestClient Create(IConnectionCredentials creds)
        {
            return new HDInsightManagementRestClient(creds);
        }
    }
}
