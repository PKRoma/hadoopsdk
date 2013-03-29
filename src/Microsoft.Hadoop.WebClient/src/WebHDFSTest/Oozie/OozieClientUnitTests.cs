using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Hadoop.WebClient.OozieClient;
using Microsoft.Hadoop.WebClient.OozieClient.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using WebClientTests.Common;

namespace WebClientTests.Oozie
{
    [TestClass]
    [DeploymentItem(@".\testfiles\Oozie", OozieTestResources.TestFilesFolder)]
    public class OozieClientUnitTests
    {
        private const string root = "/Oozietests";

        private OozieHttpClient client;

        private string outputPath;
        private string appPath;
        private string inputPath;

        private string nameNodeHost;
        private FileUploaderDownloader fileUploaderDownloader;
        private OozieTestHelper oozieTestHelper;
        private const string jobTrackerHost = "jobtrackerhost:9010";

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Init()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var oozieUrl = string.Format("{0}:563/", TestConfig.ClusterUrl);
            client = new OozieHttpClient(new Uri(oozieUrl), TestConfig.ClusterUser, TestConfig.ClusterPassword);

            fileUploaderDownloader = new FileUploaderDownloader(TestContext, TestConfig.StorageAccount, TestConfig.StoragePassword, TestConfig.ContainerName);

            nameNodeHost = string.Format("asv://{0}@{1}", TestConfig.ContainerName, TestConfig.StorageAccount);

            string rootPath = root + "/" + Guid.NewGuid().ToString("N");

            appPath = rootPath + "/app";
            outputPath = rootPath + "/output";
            inputPath = rootPath + "/input";

            // We must upolad the resources to appPath in order to submit job
            var resourcesFullPath = Path.GetFullPath(OozieTestResources.MapReduceFolder);
            fileUploaderDownloader.UploadDirectory(new DirectoryInfo(resourcesFullPath), appPath);

            oozieTestHelper = new OozieTestHelper(client, TestContext);
        }

        [TestMethod]
        public void OozieCheckStatus()
        {
            var systemMode = oozieTestHelper.GetSystemMode();
            Assert.AreEqual(OozieSystemMode.Normal, systemMode);
        }

        [TestMethod]
        public void OozieSubmitJob()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");
        }

        private Dictionary<string, string> GetProperties()
        {
            var oozieJobProperties = new OozieJobProperties(TestConfig.ClusterUser, nameNodeHost, jobTrackerHost, 
                appPath, inputPath, outputPath);
            
            return oozieJobProperties.ToDictionary();
        }


        [TestMethod]
        public void OozieStartJob()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");

            oozieTestHelper.StartJob(id);
        }

        [TestMethod]
        public void OozieKillJob()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");

            oozieTestHelper.StartJob(id);

            oozieTestHelper.KillJob(id);
        }

        [TestMethod]
        public void OozieSuspendJob()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");

            oozieTestHelper.StartJob(id);

            oozieTestHelper.SuspendJob(id);
        }

        [TestMethod]
        public void OozieResumeJob()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");

            oozieTestHelper.StartJob(id);

            oozieTestHelper.ResumeJob(id);
        }

        private void WriteTrace(JObject res)
        {
            TestContext.WriteLine(res.ToString().Replace("{", "*").Replace("}", "*"));
        }
    }
}


