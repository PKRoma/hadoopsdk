namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.PocoClient;

    class TestPocoClientFactoryFlowThrough : IHDInsightManagementPocoClientFactory
    {
        public TestPocoClientFactoryFlowThrough(IHDInsightManagementPocoClientFactory underlying)
        {
            this.underlying = underlying;
            this.Clients = new List<TestPocoClientFlowThrough>();
        }

        private IHDInsightManagementPocoClientFactory underlying;

        public List<TestPocoClientFlowThrough> Clients { get; private set; }

        public IHDInsightManagementPocoClient Create(IConnectionCredentials creds)
        {
            var client = new TestPocoClientFlowThrough(underlying.Create(creds));
            this.Clients.Add(client);
            return client;
        }
    }
}
