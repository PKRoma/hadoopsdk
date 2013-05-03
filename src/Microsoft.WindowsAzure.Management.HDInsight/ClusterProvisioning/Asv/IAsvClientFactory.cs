namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    internal interface IAsvClientFactory
    {
        IAsvClient Create();
    }
}