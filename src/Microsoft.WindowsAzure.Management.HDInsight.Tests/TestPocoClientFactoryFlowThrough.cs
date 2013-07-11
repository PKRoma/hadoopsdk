namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    class TestPocoClientFactoryFlowThrough : IHDInsightManagementPocoClientFactory
    {
        public TestPocoClientFactoryFlowThrough(IHDInsightManagementPocoClientFactory underlying)
        {
            this.underlying = underlying;
            this.Clients = new List<TestPocoClientFlowThrough>();
        }

        private IHDInsightManagementPocoClientFactory underlying;

        public List<TestPocoClientFlowThrough> Clients { get; private set; }

        public IHDInsightManagementPocoClient Create(IConnectionCredentials credentials)
        {
            var client = new TestPocoClientFlowThrough(underlying.Create(credentials));
            this.Clients.Add(client);
            return client;
        }
    }
}
