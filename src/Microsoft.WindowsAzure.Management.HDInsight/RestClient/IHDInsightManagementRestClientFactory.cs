namespace Microsoft.WindowsAzure.Management.HDInsight.RestClient
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal interface IHDInsightManagementRestClientFactory
    {
        IHDInsightManagementRestClient Create(IConnectionCredentials creds);
    }
}
