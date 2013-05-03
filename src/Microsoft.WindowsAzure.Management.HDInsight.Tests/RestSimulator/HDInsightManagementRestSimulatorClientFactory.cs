namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator
{
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class HDInsightManagementRestSimulatorClientFactory : IHDInsightManagementRestClientFactory
    {
        public IHDInsightManagementRestClient Create(IConnectionCredentials creds)
        {
            return new HDInsightManagementRestSimulatorClient(creds);
        }
    }
}
