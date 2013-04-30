namespace Microsoft.WindowsAzure.Management.HDInsight.Asv
{
    internal class AsvClientFactory : IAsvClientFactory
    {
        public IAsvClient Create()
        {
            return new AsvClient();
        }
    }
}
