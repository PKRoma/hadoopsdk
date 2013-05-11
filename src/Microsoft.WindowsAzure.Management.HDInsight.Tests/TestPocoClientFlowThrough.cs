namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System.Collections.ObjectModel;
    using Microsoft.WindowsAzure.Management.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;


    internal class TestPocoClientFlowThrough : DisposableObject, IHDInsightManagementPocoClient
    {
        private IHDInsightManagementPocoClient underlying;
        public HDInsightClusterCreationDetails LastCreateRequest { get; private set; }

        public TestPocoClientFlowThrough(IHDInsightManagementPocoClient underlying)
        {
            this.underlying = underlying;
        }

        public Task<Collection<HDInsightCluster>> ListContainers()
        {
            return underlying.ListContainers();
        }

        public Task<HDInsightCluster> ListContainer(string dnsName)
        {
            return underlying.ListContainer(dnsName);
        }

        public Task CreateContainer(HDInsightClusterCreationDetails details)
        {
            this.LastCreateRequest = details;
            return underlying.CreateContainer(details);
        }

        public Task DeleteContainer(string dnsName)
        {
            return underlying.DeleteContainer(dnsName);
        }

        public Task DeleteContainer(string dnsName, string location)
        {
            return underlying.DeleteContainer(dnsName, location);
        }

        public void WaitForClusterCondition(string dnsName, Func<HDInsightCluster, bool> evaluate, TimeSpan interval)
        {
            underlying.WaitForClusterCondition(dnsName, evaluate, interval);
        }
    }
}
