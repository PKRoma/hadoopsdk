namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    internal class AsvValidatorValidatorClientFactory : IAsvValidatorClientFactory
    {
        public IAsvValidatorClient Create()
        {
            return new AsvValidatorValidatorClient();
        }
    }
}
