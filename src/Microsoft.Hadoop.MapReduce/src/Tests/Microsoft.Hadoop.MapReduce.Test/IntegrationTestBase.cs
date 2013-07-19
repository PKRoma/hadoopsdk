using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Test
{
    using Microsoft.Hadoop.MapReduce.HadoopImplementations;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebHDFS;
    using Microsoft.Hadoop.WebHDFS.Adapters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    public class IntegrationTestBase
    {
        private IntegrationTestManager testManager = new IntegrationTestManager();

        protected static void PrepareForLocalRun()
        {
            Hadoop.MakeLocal = LocalHadoop.Create;
            Hadoop.MakeOneBox = (a, b, c) => LocalHadoop.Create();
            Hadoop.MakeAzure = (a, b, c, d, e, f, g, h) => LocalHadoop.Create();
        }

        protected static void PrepareForWebRun()
        {
            Hadoop.MakeLocal = () => WebHadoop.Create(new Uri(@"http://localhost:50111"), "hadoop", null);
            Hadoop.MakeOneBox = (a, b, c) => WebHadoop.Create(new Uri(@"http://localhost:50111"), "hadoop", null);
            Hadoop.MakeAzure = (a, b, c, d, e, f, g, h) => WebHadoop.Create(new Uri(@"http://localhost:50111"), "hadoop", null);
        }

        protected void PrepareForClusterRun(string configName)
        {
            if (!testManager.RunAzureTests())
            {
                Assert.Inconclusive("Azure tests are not configured on this machine.");
            }

            AzureTestCredentials creds = this.testManager.GetCredentials(configName);

            if (creds == null)
            {
                Assert.Inconclusive("No entry was found in the credential config file for the specified test configuration.");
            }

            string cluster = creds.WellKnownCluster.Cluster;
            string userName = creds.AzureUserName;
            string password = creds.AzurePassword;
            string container = creds.Environments[0].DefaultStorageAccount.Container;
            string hadoopUser = creds.HadoopUserName;
            string blobstorageaccountname = creds.Environments[0].DefaultStorageAccount.Name;
            string blobstorageaccountkey = creds.Environments[0].DefaultStorageAccount.Key;

            var blobAdapter = new BlobStorageAdapter(blobstorageaccountname, blobstorageaccountkey, container, true);
            HdfsFile.InternalHdfsFile = WebHdfsFile.Create(hadoopUser, new WebHDFSClient(hadoopUser, blobAdapter));
            
            Hadoop.MakeLocal = () => HadoopOnAzure.Create(new Uri(cluster), 
                                                          userName, 
                                                          hadoopUser, 
                                                          password,
                                                          blobstorageaccountname, 
                                                          blobstorageaccountkey, 
                                                          container, 
                                                          true);

            // NOTE: We ignore the supplied parameters and replace with the current config.
            Hadoop.MakeOneBox = (a, b, c) => HadoopOnAzure.Create(new Uri(cluster), 
                                                                  userName, 
                                                                  hadoopUser, 
                                                                  password,
                                                                  blobstorageaccountname, 
                                                                  blobstorageaccountkey, 
                                                                  container, 
                                                                  true);

            // NOTE: We ignore the supplied parameters and replace with the current config.
            Hadoop.MakeAzure = (a, b, c, d, e, f, g, h) => HadoopOnAzure.Create(new Uri(cluster), 
                                                                                userName, 
                                                                                hadoopUser, 
                                                                                password,
                                                                                blobstorageaccountname, 
                                                                                blobstorageaccountkey, 
                                                                                container, 
                                                                                true);
        }


        [TestInitialize]
        public void SetupForNextTest()
        {
            Hadoop.MakeLocal = TestSetup.makeLocalHadoop;
            Hadoop.MakeOneBox = TestSetup.makeOneBoxHadoop;
            Hadoop.MakeAzure = TestSetup.makeAzureHadoop;
            HdfsFile.InternalHdfsFile = TestSetup.OriginalHdfsFile;
        }
    }
}
