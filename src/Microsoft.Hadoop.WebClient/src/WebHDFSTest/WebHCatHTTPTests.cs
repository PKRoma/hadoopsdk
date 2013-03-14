using Microsoft.Hadoop.WebHCat;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Hadoop.WebHCat.Protocol;
using Microsoft.Hadoop.WebHDFS.Adapters;

namespace WebClientTests
{
    [TestClass]
    public class WebHCatHTTPTests
    {
        WebHCatHttpClient httpClient;
        WebHDFSClient webhdfsClient;

        bool useMockServer = false;

        string container;

        BlobStorageAdapter adapter;

        string baseDirectory;

        [TestInitialize]
        public void SetupTests()
        {
            // switch localhost to localhost. if want fiddler support
            if (useMockServer)
            {
                httpClient = new WebHCatHttpClient(new Uri("http://localhost:50111"), "hadoop", null, "hadoop", new MockServer(Microsoft.Hadoop.Version.V1, 10));
            }
            else
            {
                httpClient = new WebHCatHttpClient(new Uri("http://localhost:50111"), "hadoop", null);
            }

            // set up working directory
            webhdfsClient = new WebHDFSClient(new Uri("http://localhost:50070"), "hadoop");

            container = "webhcattests" + DateTimeOffset.Now.Ticks;
            adapter = new BlobStorageAdapter(TestConfig.StorageAccount, TestConfig.StoragePassword, this.container, true);
            //adapter = new BlobStorageAdapter(TestConfig.StorageAccount, TestConfig.StoragePassword, "blob.core.windows-int.net");

            webhdfsClient = new WebHDFSClient("hadoop", adapter);

            baseDirectory = "asv://" + container + "@" + TestConfig.StorageAccount;

            //LoadData();
        }

        private void LoadData()
        {
            var createdFile = webhdfsClient.CreateFile("./testfiles/awards.txt", "/awards.txt");
            createdFile.Wait();

            string outputDir = "/createhivetable";

            this.CreateDirectory(outputDir);

            string hiveQuery = @"CREATE TABLE IF NOT EXISTS Awards(MovieId string, AwardId string, Year int, Won string, Type string, Category string) row format delimited fields terminated by ',';";

            var createTable = httpClient.CreateHiveJob(hiveQuery, null, null, baseDirectory + outputDir, null);
            createTable.Wait();

            var response = createTable.Result;
            var output = response.Content.ReadAsAsync<JObject>();

            string id = output.Result.GetValue("id").ToString();

            var t2 = httpClient.GetQueue(id);
            t2.Wait();
            response = t2.Result;
            response.EnsureSuccessStatusCode();
            output = response.Content.ReadAsAsync<JObject>();
            output.Wait();

            // TODO - Load table
        }

        [TestCleanup]
        public void Cleanup()
        {
            adapter.DeleteContainer();
        }

        [TestMethod]
        public void CreateHiveJobHTTP()
        {
            string outputDir = "/createhivejob";

            this.CreateDirectory(outputDir);

            var t1 = httpClient.CreateHiveJob(@"select * from foobar;", null, null, baseDirectory + outputDir, null);
            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            string id = output.Result.GetValue("id").ToString();
            httpClient.WaitForJobToCompleteAsync(id).Wait();
                  
            // TODO - check results.
        }

        [TestMethod]
        public void CreatePigJobHTTP()
        {
            string outputDir = "/createpigjob";

            this.CreateDirectory(outputDir);

            var t1 = httpClient.CreatePigJob("A = LOAD '" + baseDirectory + "/awards.txt' AS (MovieId:chararray, AwardId:chararray, Year:float, Won:chararray, Type:chararray, Category:chararray); DUMP A;", null, null, null, baseDirectory + outputDir, null);
            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            string id = output.Result.GetValue("id").ToString();
            httpClient.WaitForJobToCompleteAsync(id).Wait();

            // TODO - check results.
        }

        [TestMethod]
        public void CreateMapReduceJarJob()
        {
            string outputDir = "/createmapreducejar";
            this.CreateDirectory(outputDir);

            var createdFile = webhdfsClient.CreateFile("./testfiles/hadoopexamples.jar", "/createmapreducejar/hadoopexamples.jar");
            createdFile.Wait();

            var jar = baseDirectory + outputDir + "/hadoopexamples.jar";
            var className = "pi";
            var statusdir = baseDirectory + outputDir;

            var t1 = httpClient.CreateMapReduceJarJob(jar, className, null, null, new string[] { "4", "100" }, null, statusdir, null);
            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            string id = output.Result.GetValue("id").ToString();
            httpClient.WaitForJobToCompleteAsync(id).Wait();

            // TODO - check results.
        }

        [TestMethod]
        public void CreateMapReduceJobStreaming()
        {
            string outputDir = "/createmapreducestreaming";
            this.CreateDirectory(outputDir);

            var createdFile = webhdfsClient.CreateFile("./testfiles/cat.exe", "/createmapreducestreaming/cat.exe");
            createdFile.Wait();
            createdFile = webhdfsClient.CreateFile("./testfiles/cat.exe", "/createmapreducestreaming/wc.exe");
            createdFile.Wait();
            createdFile = webhdfsClient.CreateFile("./testfiles/davinci.txt", "/createmapreducestreaming/davinci.txt");
            createdFile.Wait();

            var input = baseDirectory + outputDir + "/davinci.txt";
            var mapper = "cat.exe";
            var reducer = "wc.exe";

            var files = new List<string>() { baseDirectory + outputDir + "/cat.exe", baseDirectory + outputDir + "/wc.exe" };

            var outputLocation = baseDirectory + outputDir + "/output";

            var statusdir = baseDirectory + outputDir;

            var t1 = httpClient.CreateMapReduceStreamingJob(input, outputLocation, mapper, reducer, null, null, files, null, null, statusdir, null);

            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            string id = output.Result.GetValue("id").ToString();
            httpClient.WaitForJobToCompleteAsync(id).Wait();
        }

        [TestMethod]
        public void GetQueue()
        {
            var t = httpClient.GetQueue();
            t.Wait();

            var r = t.Result;
            r.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public void GetStatus()
        {
            var t = httpClient.GetStatus();
            t.Wait();

            var r = t.Result;
            r.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public void GetVersion()
        {
            var t = httpClient.GetVersion();
            t.Wait();
            
            var r = t.Result;
            r.EnsureSuccessStatusCode(); 
        }

        [TestMethod]
        public void GetVersionWithVersion()
        {
            var t = httpClient.GetResponseTypes("v1");
            t.Wait();

            var r = t.Result;
            r.EnsureSuccessStatusCode();
        }

        private void CreateDirectory(string name)
        {
            var r = webhdfsClient.CreateDirectory(name);
            r.Wait();
        }

        private void DeleteDirectory(string name)
        {
            var r = webhdfsClient.DeleteDirectory(name, true);
            r.Wait();
        }

    }
}
