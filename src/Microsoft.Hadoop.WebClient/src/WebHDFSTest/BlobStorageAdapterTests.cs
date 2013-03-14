using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.Hadoop.WebHDFS.Adapters;

namespace WebClientTests
{
    // note, unit tests assume that you have a local cluster running
    // and that you can get to it without any auth
    // once we resolve providing the location, we'll be in a better place. 
    [TestClass]
    public class BlobStorageAdapterTests
    {
        WebHDFSClient client;

        BlobStorageAdapter adapter;

        string container;

        [TestInitialize]
        public void SetupTests()
        {
            container = "asvadapter" + DateTimeOffset.Now.Ticks;
            adapter = new BlobStorageAdapter(TestConfig.StorageAccount, TestConfig.StoragePassword, this.container, true);

            client = new WebHDFSClient("hadoop", adapter);
        }

        [TestCleanup]
        public void Cleanup()
        {
            adapter.DeleteContainer();
            adapter.Disconnect();
        }

        internal string GetBasePath()
        {
            var uri = adapter.BaseUri;
            return "asv://" + this.container + "@" + TestConfig.StorageAccount + ".blob.core.windows.net" + "/";
        }

        [TestMethod]
        public void AdapterGetHomeDirectory()
        {
            bool exceptionWasThrown = false;
            
            var path = client.GetHomeDirectory();

            try
            {
                path.Wait();
            }
            catch (AggregateException e)
            {
                exceptionWasThrown = true;
                Assert.AreEqual(e.InnerException.Message, "BlobStorageAdapter does not support HomeDirectory.");
            }

            Assert.IsTrue(exceptionWasThrown);
        }

        [TestMethod]
        public void AdapterRenameDirectory()
        {
            // TODO - error case, need to catch error
            //var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_23");
            //deleted.Wait();
            //System.Threading.Tasks.Task<DirectoryListing> entries;
            //System.Threading.Tasks.Task<bool> created;
            //CreateDirectory(out entries, out created);
            //var renameDir = client.RenameDirectory("/UNIT_TEST_CREATE", "/UNIT_TEST_CREATE_23");
            //renameDir.Wait();
            //var contentSummaryAfter = client.GetContentSummary("/UNIT_TEST_CREATE_23");
            //contentSummaryAfter.Wait();
            //Assert.AreEqual(1, contentSummaryAfter.Result.DirectoryCount, "Same Number of Directories");

            //var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_23");
            //deleted2.Wait();
        }

        [TestMethod]
        public void AdapterCreateDirectory()
        {
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            Assert.IsTrue(created.Result, "should have created a new directory");
            int numDirectories = entries.Result.Directories.Count();
            var entriesPost = client.GetDirectoryStatus("/");
            entriesPost.Wait();
            Assert.AreEqual(entriesPost.Result.Directories.Count(), entries.Result.Directories.Count() + 1, "Should see an additional directory created");
            DeleteDirectory();

        }

        [TestMethod]
        public void AdapterCreateDeepDirectoryHierarchy()
        {
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;

            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE", true);
            deleted.Wait();
            entries = client.GetDirectoryStatus("/");
            entries.Wait();
            created = client.CreateDirectory("/UNIT_TEST_CREATE/PARENT1/PARENT2");
            created.Wait();

            Assert.IsTrue(created.Result, "should have created a new directory");

            int numDirectories = entries.Result.Directories.Count();
            var entriesPost = client.GetDirectoryStatus("/");
            entriesPost.Wait();
            Assert.AreEqual(entriesPost.Result.Directories.Count(), entries.Result.Directories.Count() + 1, "Should see an additional directory created");

            entriesPost = client.GetDirectoryStatus("/UNIT_TEST_CREATE/PARENT1");
            entriesPost.Wait();
            Assert.AreEqual(entriesPost.Result.Directories.Count(), 1);

            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE/PARENT1/PARENT2");
            deleted2.Wait();

            var deleted3 = client.DeleteDirectory("/UNIT_TEST_CREATE/PARENT1");
            deleted3.Wait();

            var deleted4 = client.DeleteDirectory("/UNIT_TEST_CREATE");
            deleted4.Wait();
        }

        private void DeleteDirectory()
        {
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE");
            deleted2.Wait();
        }

        [TestMethod]
        public void AdapterValidateContentSummary()
        {
            var contentSummaryBefore = client.GetContentSummary("/");
            contentSummaryBefore.Wait();
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            var contentSummaryAfter = client.GetContentSummary("/");
            contentSummaryAfter.Wait();
            Assert.AreEqual(contentSummaryBefore.Result.DirectoryCount + 1, contentSummaryAfter.Result.DirectoryCount, "Same Number of Directories");

            DeleteDirectory();
        }

        private void CreateDirectory(out System.Threading.Tasks.Task<DirectoryListing> entries, out System.Threading.Tasks.Task<bool> created)
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE", true);
            deleted.Wait();
            entries = client.GetDirectoryStatus("/");
            entries.Wait();
            created = client.CreateDirectory("/UNIT_TEST_CREATE");
            created.Wait();
        }

        [TestMethod]
        [DeploymentItem(".\\testfiles\\basicInput.txt")]
        public void AdapterPutFile()
        {
            var createdFile = CreateFile();
            string expectedPath = this.GetBasePath() + "UNIT_TEST_CREATE_FILE/basicInput.txt";
            Assert.IsTrue(createdFile.Equals(expectedPath), "file created");
            DeleteTextFile();
        }

        [TestMethod]
        [DeploymentItem(".\\testfiles\\basicInput.txt")]
        public void AdapterPutWithDeepDirectoryFile()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted.Wait();
            var created = client.CreateDirectory("/UNIT_TEST_CREATE_FILE/A/B/C/D/E");
            created.Wait();
            var createdFile = client.CreateFile("./basicInput.txt", "/UNIT_TEST_CREATE_FILE/A/B/C/D/E/basicInput.txt");
            createdFile.Wait();
            string expectedPath = this.GetBasePath() + "UNIT_TEST_CREATE_FILE/A/B/C/D/E/basicInput.txt";
            Assert.IsTrue(createdFile.Result.Equals(expectedPath), "file created");
            
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted2.Wait();
        }

        private void DeleteTextFile()
        {
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted2.Wait();
        }

        private void DeleteBigTextFile()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE/BIG_FILE_DIR/bigfile.txt");
            deleted.Wait();
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE/BIG_FILE_DIR", false);
            deleted2.Wait();
            var deleted3 = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", false);
            deleted3.Wait();
        }

        [TestMethod]
        [DeploymentItem(".\\testfiles\\basicInput.txt")]
        public void AdapterValidateFile()
        {
            CreateFile();
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.IsTrue(fileStatus.Result.Length > 10, "file exists");
            DeleteTextFile();
        }

        [TestMethod]
        public void AdapterOpenPartOfAFile()
        {
            // TODO - will be error case until switch to blob client that supports random access
            //CreateFile();
            //var fileStatus = client.OpenFile("/UNIT_TEST_CREATE_FILE/basicInput.txt", 3, 3);
            //fileStatus.Wait();
            //var readFile = fileStatus.Result.Content.ReadAsStringAsync();
            //readFile.Wait();
            //Assert.AreEqual("thr", readFile.Result, "correct offset & characters.");
            //DeleteTextFile();
        }

        [TestMethod]
        public void AdapterValidateChecksum()
        {
            // TODO
            //CreateFile();
            //var fileChecksum = client.GetFileChecksum("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileChecksum.Wait();
            //Assert.AreEqual(fileChecksum.Result.Checksum, "0000020000000000000000002b5681a8c222c7cd2deff67f303b8fc800000000", "file exists");
            //DeleteTextFile();
        }

        [TestMethod]
        public void AdapterSetReplicationFactor()
        {
            // TODO
            //CreateFile();
            //var fileChecksum = client.SetReplicationFactor("/UNIT_TEST_CREATE_FILE/basicInput.txt", 1);
            //fileChecksum.Wait();
            //Assert.IsTrue(fileChecksum.Result);
            //DeleteTextFile();
        }

        [TestMethod]
        public void AdapterSetOwner()
        {
            // TODO
            //CreateFile();
            //var response = client.SetOwner("/UNIT_TEST_CREATE_FILE/basicInput.txt", "hadoop");
            //response.Wait();
            //Assert.IsTrue(response.Result, "file changed");
            //var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileStatus.Wait();
            //Assert.AreEqual(fileStatus.Result.Owner, "hadoop");
            //DeleteTextFile();
        }



        [TestMethod]
        public void AdapterSetGroup()
        {
            // TODO
            //CreateFile();
            //var response = client.SetGroup("/UNIT_TEST_CREATE_FILE/basicInput.txt", "admin");
            //response.Wait();
            //Assert.IsTrue(response.Result, "file changed");
            //var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileStatus.Wait();
            //Assert.AreEqual(fileStatus.Result.Group, "admin");
            //DeleteTextFile();
        }

        [TestMethod]
        public void AdapterSetPermission()
        {
            // TODO
            //CreateFile();
            //var response = client.SetPermissions("/UNIT_TEST_CREATE_FILE/basicInput.txt", "333");
            //response.Wait();
            //Assert.IsTrue(response.Result, "file changed");
            //var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileStatus.Wait();
            //Assert.AreEqual(fileStatus.Result.Permission, "333");
            //DeleteTextFile();
        }


        [TestMethod]
        public void AdapterSetAccessTime()
        {
            // TODO
            //CreateFile();
            //var response = client.SetAccessTime("/UNIT_TEST_CREATE_FILE/basicInput.txt", "23");
            //response.Wait();
            //Assert.IsTrue(response.Result, "file changed");
            //var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileStatus.Wait();
            //Assert.AreEqual(fileStatus.Result.AccessTime, "23");
            //DeleteTextFile();
        }

        [TestMethod]
        public void AdapterSetModificationTime()
        {
            //CreateFile();
            //var response = client.SetModificationTime("/UNIT_TEST_CREATE_FILE/basicInput.txt", "23");
            //response.Wait();
            //Assert.IsTrue(response.Result, "file changed");
            //var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            //fileStatus.Wait();
            //Assert.AreEqual(fileStatus.Result.ModificationTime, "23");
            //DeleteTextFile();
        }

        [TestMethod]
        [DeploymentItem(".\\testfiles\\basicInput.txt")]
        public void AdapterOpenAndReadFile()
        {
            CreateFile();
            var fileRead = client.OpenFile("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsStringAsync();
            content.Wait();
            Assert.AreEqual(System.IO.File.ReadAllText(@".\basicInput.txt"), content.Result, "received right file");
            DeleteTextFile();
        }

        [TestMethod]
        [DeploymentItem(".\\titles.txt")]
        public void AdapterOpenAndReadBigFile()
        {
            CreateBigFile();
            var fileRead = client.OpenFile("/UNIT_TEST_CREATE_FILE/BIG_FILE_DIR/bigfile.txt");
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsStringAsync();
            content.Wait();
            Assert.AreEqual(System.IO.File.ReadAllText(@".\titles.txt"), content.Result, "received right file");
            DeleteBigTextFile();
        }

        // TODO - move into seperate file.
        [TestMethod]
        [DeploymentItem(".\\testfiles\\basicInput.txt")]
        public void Regression1177285()
        {
            var deleted = client.DeleteDirectory("/IMPLICIT_DIRECTORY", true);
            deleted.Wait();

            var createdFile = client.CreateFile("./basicInput.txt", "/IMPLICIT_DIRECTORY/basicInput.txt");
            createdFile.Wait();

            string expectedPath = this.GetBasePath() + "IMPLICIT_DIRECTORY/basicInput.txt";
            Assert.IsTrue(createdFile.Result.Equals(expectedPath), "file created");

            var deleted2 = client.DeleteDirectory("/IMPLICIT_DIRECTORY", true);
            deleted2.Wait();
        }

        private string CreateFile()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted.Wait();
            var created = client.CreateDirectory("/UNIT_TEST_CREATE_FILE");
            created.Wait();
            var createdFile = client.CreateFile("./basicInput.txt", "/UNIT_TEST_CREATE_FILE/basicInput.txt");
            createdFile.Wait();
            return createdFile.Result;
        }

        private string CreateBigFile()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted.Wait();
            var created = client.CreateDirectory("/UNIT_TEST_CREATE_FILE");
            created.Wait();
            created = client.CreateDirectory("/UNIT_TEST_CREATE_FILE/BIG_FILE_DIR");
            created.Wait();
            var createdFile = client.CreateFile("./titles.txt", "/UNIT_TEST_CREATE_FILE/BIG_FILE_DIR/bigfile.txt");
            createdFile.Wait();
            return createdFile.Result;
        }
    }
}
