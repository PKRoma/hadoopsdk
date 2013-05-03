namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    using System.Threading.Tasks;

    internal interface IAsvClient
    {
        Task ValidateAccount(string fullAccount, string key);

        Task ValidateContainer(string fullAccount, string key, string container);
    }
}
