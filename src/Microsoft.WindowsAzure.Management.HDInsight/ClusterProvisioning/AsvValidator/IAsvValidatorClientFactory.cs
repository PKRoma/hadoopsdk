namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    internal interface IAsvValidatorClientFactory
    {
        IAsvValidatorClient Create();
    }
}