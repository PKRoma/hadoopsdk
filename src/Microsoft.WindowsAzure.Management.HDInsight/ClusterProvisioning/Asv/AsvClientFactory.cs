namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    internal class AsvClientFactory : IAsvClientFactory
    {
        public IAsvClient Create()
        {
            return new AsvClient();
        }
    }
}
