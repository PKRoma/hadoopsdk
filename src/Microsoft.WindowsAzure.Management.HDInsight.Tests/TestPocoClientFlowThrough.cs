namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System.Collections.ObjectModel;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.RestClient;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    internal class TestPocoClientFlowThrough : DisposableObject, IHDInsightManagementPocoClient
    {
        private IHDInsightManagementPocoClient underlying;
        public CreateClusterRequest LastCreateRequest { get; private set; }

        public TestPocoClientFlowThrough(IHDInsightManagementPocoClient underlying)
        {
            this.underlying = underlying;
        }

        public Task<Collection<ListClusterContainerResult>> ListContainers()
        {
            return underlying.ListContainers();
        }

        public Task<ListClusterContainerResult> ListContainer(string dnsName)
        {
            return underlying.ListContainer(dnsName);
        }

        public Task CreateContainer(CreateClusterRequest cluster)
        {
            this.LastCreateRequest = cluster;
            return underlying.CreateContainer(cluster);
        }

        public Task DeleteContainer(string dnsName)
        {
            return underlying.DeleteContainer(dnsName);
        }

        public Task DeleteContainer(string dnsName, string location)
        {
            return underlying.DeleteContainer(dnsName, location);
        }

        public void WaitForClusterCondition(string dnsName, Func<ListClusterContainerResult, bool> evaluate, TimeSpan interval)
        {
            underlying.WaitForClusterCondition(dnsName, evaluate, interval);
        }
    }
}
