namespace Microsoft.WindowsAzure.Management.HDInsight.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

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
