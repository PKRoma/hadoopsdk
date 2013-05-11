namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator
{
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class SubscriptionRegistrationSimulatorClientFactory : ISubscriptionRegistrationClientFactory
    {
        public ISubscriptionRegistrationClient Create(IConnectionCredentials creds)
        {
            return new SubscriptionRegistrationSimulatorClient(creds);
        }
    }
}
