namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.RestClient;

    internal class HDInsightManagementRestSimulatorClientFactory : IHDInsightManagementRestClientFactory
    {
        public IHDInsightManagementRestClient Create(IConnectionCredentials creds)
        {
            return new HDInsightManagementRestSimulatorClient(creds);
        }
    }
}
