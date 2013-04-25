using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Hadoop.WebClient.AmbariClient;
using Microsoft.Hadoop.WebClient.AmbariClient.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebClientTests.Ambari
{
    [TestClass]
    public class AmbariClientUnitTests
    {
        private AmbariClient client;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Init()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var uri = string.Format("{0}:563/", TestConfig.ClusterUrl);
            client = new AmbariClient(new Uri(uri), TestConfig.ClusterUser, TestConfig.ClusterPassword);
        }

        [TestMethod]
        public void AbmariGetClusters()
        {
            IList<ClusterInfo> clusterInfos = client.GetClusters();
            Assert.AreEqual(1, clusterInfos.Count, "Cluster count is wrong");

            ClusterInfo clusterInfo = clusterInfos[0];
            string clusterHref = clusterInfo.Href;
            bool hrefIncludeClusterName = clusterHref.StartsWith(TestConfig.ClusterUrl);
            Assert.IsTrue(hrefIncludeClusterName, "Cluster href result  {0} doesn't start with {1}", clusterHref, TestConfig.ClusterUrl);
        }

        [TestMethod]
        public void AbmariGetHostComponentMetric()
        {
            string clusterName = TestConfig.ClusterUrl.Replace(@"https://", string.Empty).Replace(".azurehdinsight.net", string.Empty);
            HostComponentMetric hostComponentMetric = client.GetHostComponentMetric(clusterName);
        }
        
        [TestMethod]
        public void AbmariGetGetAsvMetrics()
        {
            IEnumerable<double> asvMetrics = client.GetAsvMetrics(TestConfig.StorageAccount, DateTime.Now, DateTime.Now);
        }
    }
}
