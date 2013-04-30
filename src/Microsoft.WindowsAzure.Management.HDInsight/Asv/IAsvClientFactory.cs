namespace Microsoft.WindowsAzure.Management.HDInsight.Asv
{
    internal interface IAsvClientFactory
    {
        IAsvClient Create();
    }
}