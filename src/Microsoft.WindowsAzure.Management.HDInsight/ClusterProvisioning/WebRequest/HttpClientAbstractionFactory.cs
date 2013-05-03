namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.WebRequest
{
    using System.Security.Cryptography.X509Certificates;

    internal class HttpClientAbstractionFactory : IHttpClientAbstractionFactory
    {
        public IHttpClientAbstraction Create(X509Certificate2 cert)
        {
            return HttpClientAbstraction.Create(cert);
        }

        public IHttpClientAbstraction Create()
        {
            return HttpClientAbstraction.Create();
        }
    }
}
