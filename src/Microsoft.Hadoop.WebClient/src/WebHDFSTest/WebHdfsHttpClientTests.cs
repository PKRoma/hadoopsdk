using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientTests
{
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using Microsoft.Hadoop.WebClient;
    using Microsoft.Hadoop.WebClient.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Hadoop.WebHDFS;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class WebHdfsHttpClientTests
    {

        private readonly Uri webHdfsUri = new Uri("http://localhost:50070/webhdfs/v1");
        private readonly string testContent = "This is test content";

        [TestInitialize]
        public void SetupTests()
        {
        }

        private MemoryStream GetTestContent(string content)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            return new MemoryStream(byteArray);
        }

        private void CreateFile(Uri webHdfsUri, string path, Stream content)
        {
            var httpClient = new HttpClient(new WebRequestHandler { AllowAutoRedirect = false });

            var createTask = httpClient.PutAsync(webHdfsUri + path + "?op=CREATE&overwrite=true", null);
            createTask.Wait();

            content.Position = 0;
            var sc = new StreamContent(content);
            
            var storeTask = httpClient.PutAsync(createTask.Result.Headers.Location, sc);
            storeTask.Wait();
            storeTask.Result.EnsureSuccessStatusCode();
        }

        private HttpContent OpenFile(Uri webHdfsUri, string path)
        {
            var httpClient = new HttpClient(new WebRequestHandler());

            var openTask = httpClient.GetAsync(webHdfsUri + path + "?op=OPEN");
            openTask.Wait();
            openTask.Result.EnsureSuccessStatusCode();
            return openTask.Result.Content;
        }

        private void RemoveFile(Uri webHdfsUri, string path)
        {
            var httpClient = new HttpClient(new WebRequestHandler());

            var deleteTask = httpClient.DeleteAsync(webHdfsUri + path + "?op=DELETE");
            deleteTask.Wait();
            deleteTask.Result.EnsureSuccessStatusCode();
        }

        private DirectoryEntry GetFileStatus(Uri webHdfsUri, string path)
        {
            var httpClient = new HttpClient(new WebRequestHandler());

            var statusTask = httpClient.GetAsync(webHdfsUri + path + "?op=GETFILESTATUS");
            statusTask.Wait();
            statusTask.Result.EnsureSuccessStatusCode();

            var filesStatusTask =  statusTask.Result.Content.ReadAsAsync<JObject>();
            filesStatusTask.Wait();

            return new DirectoryEntry(filesStatusTask.Result.Value<JObject>("FileStatus"));
        }

        [TestMethod]
        public void OpenFile()
        {
            const string Path = "/foo.txt";
            var memStream = GetTestContent(testContent);
            
            CreateFile(webHdfsUri, Path, memStream);

            var client = new WebHdfsHttpClient(webHdfsUri);
            var contentTask = client.OpenFile(Path);
            contentTask.Wait();
            var contentStream = contentTask.Result.ReadAsStringAsync();
            contentStream.Wait();

            Assert.IsNotNull(contentTask.Result);
            Assert.AreEqual(testContent, contentStream.Result, false, CultureInfo.InvariantCulture);

            RemoveFile(webHdfsUri, Path);
        }

        [TestMethod]
        public void OpenFileDoesNotExist()
        {
            const string Path = "/doesnotexist.txt";

            var client = new WebHdfsHttpClient(webHdfsUri);
            try
            {
                var contentTask = client.OpenFile(Path);
                contentTask.Wait();
            }
            catch (AggregateException e)
            {
                if (!e.InnerException.Message.Contains("404"))
                {
                    Assert.Fail(e.InnerException.Message);
                }
            }
        }

        [TestMethod]
        public void CreateFile()
        {
            const string Path = "/bar.txt";
            var memStream = GetTestContent(testContent);

            var client = new WebHdfsHttpClient(webHdfsUri);
            var createTask = client.CreateFile(Path, memStream, true);
            createTask.Wait();

            var stream = OpenFile(webHdfsUri, Path);
            var content = stream.ReadAsStringAsync();
            content.Wait();

            Assert.IsNotNull(content.Result);
            Assert.AreEqual(testContent, content.Result, false, CultureInfo.InvariantCulture);

            RemoveFile(webHdfsUri, Path);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void DeleteFile()
        {
            const string Path = "/removeme.txt";
            var memStream = GetTestContent(testContent);

            CreateFile(webHdfsUri, Path, memStream);
            
            var client = new WebHdfsHttpClient(webHdfsUri);
            
            try
            {
                var deleteTask = client.Delete(Path, false);
                deleteTask.Wait();

                Assert.IsNotNull(deleteTask.Result);
                Assert.IsTrue(deleteTask.Result);
            }
            catch (Exception e)
            {
                //Since we expect a 404 at the end of this test, this is to ensure that 
                //we do not get a false positive if the delte method throws.
                Assert.Fail("Delete method threw: " + e.Message);
            }

            GetFileStatus(webHdfsUri, Path);
        }

        [TestMethod]
        public void GetFileStatus()
        {
            const string FileName = "foo.txt";
            var path = "/" + FileName;
            var memStream = GetTestContent(testContent);
            var length = memStream.Length;

            CreateFile(webHdfsUri, path, memStream);

            var client = new WebHdfsHttpClient(webHdfsUri);
            var statusTask = client.GetFileStatus(path);
            statusTask.Wait();

            Assert.IsNotNull(statusTask.Result);
            Assert.AreEqual("FILE",statusTask.Result.Type);
            Assert.AreEqual(length, statusTask.Result.Length);

            RemoveFile(webHdfsUri, path);
        }
    }
}
