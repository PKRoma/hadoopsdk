using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Hadoop.WebClient.OozieClient;
using Microsoft.Hadoop.WebClient.OozieClient.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebClientTests.Common;

namespace WebClientTests.Oozie
{
    [TestClass]
    [DeploymentItem(@".\testfiles\Oozie", OozieTestResources.TestFilesFolder)]
    public class OozieClientIntegrationTests
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
            inputPath = string.Format("{0}/input", appPath);
            outputPath = rootPath + "/output";

            // We must upolad the resources to appPath in order to submit job
            var resourcesFullPath = Path.GetFullPath(OozieTestResources.MapReduceFolder);
            fileUploaderDownloader.UploadDirectory(new DirectoryInfo(resourcesFullPath), appPath);

            oozieTestHelper = new OozieTestHelper(client, TestContext);
        }

        [TestMethod]
        public void OozieRunMapReduce()
        {
            var properties = GetProperties();
            string id = oozieTestHelper.SubmitJob(properties);
            Assert.IsNotNull(id, "Id is null");

            oozieTestHelper.StartJob(id);

            // Wait until job exits Running state and move to the next state
            oozieTestHelper.WaitForStatusChange(id, OozieJobStatus.Running, TimeSpan.FromMinutes(30));

            // Make sure that the job succeeded 
            string status = oozieTestHelper.GetJobStatus(id);
            Assert.AreEqual(OozieJobStatus.Succeeded, status);

            string resultsFilePath = string.Format("{0}/part-00000", outputPath);
            string localdst = Guid.NewGuid().ToString("N");
            fileUploaderDownloader.Download(resultsFilePath, localdst);

            // Validates WF content
            string localExpectedResult = Path.Combine(OozieTestResources.MapReduceFolder, "out", "expected.out");
            ValidateFilesAreEqual(localExpectedResult, localdst);

        }

        private void ValidateFilesAreEqual(string expectedOutputFilePath, string actualOutputFilePath)
        {
            string actual = File.ReadAllText(actualOutputFilePath);
            TestContext.WriteLine("Job output is:");
            TestContext.WriteLine(actual);

            string expected = File.ReadAllText(expectedOutputFilePath);
            Assert.AreEqual(expected, actual, "Output file content is not as expected");
        }

        private Dictionary<string, string> GetProperties()
        {
            var oozieJobProperties = new OozieJobProperties(TestConfig.ClusterUser, nameNodeHost, jobTrackerHost,
                appPath, inputPath, outputPath);

            return oozieJobProperties.ToDictionary();
        }


    }
}


