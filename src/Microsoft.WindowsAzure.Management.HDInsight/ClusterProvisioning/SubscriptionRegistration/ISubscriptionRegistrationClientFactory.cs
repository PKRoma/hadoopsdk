namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal interface ISubscriptionRegistrationClientFactory
    {
        ISubscriptionRegistrationClient Create(IConnectionCredentials creds);
    }
}
