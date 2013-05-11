namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class SubscriptionRegistrationClientFactory : ISubscriptionRegistrationClientFactory
    {
        public ISubscriptionRegistrationClient Create(IConnectionCredentials creds)
        {
            return new SubscriptionRegistrationClient(creds);
        }
    }
}
