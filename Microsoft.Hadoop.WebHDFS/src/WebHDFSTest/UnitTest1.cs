using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Hadoop.WebHDFS;

namespace WebHDFSTest
{
    // note, unit tests assume that you have a local cluster running
    // and that you can get to it without any auth
    // once we resolve providing the location, we'll be in a better place. 
    [TestClass]
    public class OperationalTests
    {


        [TestMethod]
        public void RenameDirectory()
        {
            var deleted = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE_23");
            deleted.Wait();
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            var renameDir = WebHDFSClient.RenameDirectory("/UNIT_TEST_CREATE", "/UNIT_TEST_CREATE_23");
            renameDir.Wait();
            var contentSummaryAfter = WebHDFSClient.GetContentSummary("/UNIT_TEST_CREATE_23");
            contentSummaryAfter.Wait();
            Assert.AreEqual(1, contentSummaryAfter.Result.DirectoryCount, "Same Number of Directories");

            var deleted2 = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE_23");
            deleted2.Wait();
        }

        [TestMethod]
        public void CreateDirectory()
        {
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            Assert.IsTrue(created.Result, "should have created a new directory");
            int numDirectories = entries.Result.Directories.Count();
            var entriesPost = WebHDFSClient.GetDirectoryStatus("/");
            entriesPost.Wait();
            Assert.AreEqual(entriesPost.Result.Directories.Count(), entries.Result.Directories.Count() + 1, "Should see an additional directory created");
            DeleteDirectory();
    
        }

        private static void DeleteDirectory()
        {
            var deleted2 = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE");
            deleted2.Wait();
        }

        [TestMethod]
        public void ValidateContentSummary()
        {
            var contentSummaryBefore = WebHDFSClient.GetContentSummary("/");
            contentSummaryBefore.Wait();
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            var contentSummaryAfter = WebHDFSClient.GetContentSummary("/");
            contentSummaryAfter.Wait();
            Assert.AreEqual(contentSummaryBefore.Result.DirectoryCount + 1, contentSummaryAfter.Result.DirectoryCount, "Same Number of Directories");

            DeleteDirectory();
        }

        private static void CreateDirectory(out System.Threading.Tasks.Task<DirectoryListing> entries, out System.Threading.Tasks.Task<bool> created)
        {
            var deleted = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE", true);
            deleted.Wait();
            entries = WebHDFSClient.GetDirectoryStatus("/");
            entries.Wait();
            created = WebHDFSClient.CreateDirectory("/UNIT_TEST_CREATE");
            created.Wait();
        }

        [TestMethod]
        public void PutFile()
        {
            var createdFile = CreateFile();
            Assert.IsTrue(createdFile.Equals("webhdfs://localhost:50070/UNIT_TEST_CREATE_FILE/basicInput.txt") , "file created");
            DeleteTextFile();
        }

        private static void DeleteTextFile()
        {
            var deleted2 = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted2.Wait();
        }


        [TestMethod]
        public void ValidateFile()
        {
            CreateFile();
            var fileStatus = WebHDFSClient.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.IsTrue(fileStatus.Result.Length > 10, "file exists");
            DeleteTextFile();
        }

        [TestMethod]
        public void ValidateChecksum()
        {
            CreateFile();
            var fileChecksum = WebHDFSClient.GetFileChecksum("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileChecksum.Wait();
            Assert.AreEqual(fileChecksum.Result.Checksum, "0000020000000000000000002b5681a8c222c7cd2deff67f303b8fc800000000", "file exists");
            DeleteTextFile();
        }

        [TestMethod]
        public void OpenAndReadFile()
        {
            CreateFile();
            var fileRead = WebHDFSClient.OpenFile("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsStringAsync();
            content.Wait();
            Assert.AreEqual(System.IO.File.ReadAllText(@".\basicInput.txt"), content.Result, "received right file");  
            DeleteTextFile();
        }

        private static string CreateFile()
        {
            var deleted = WebHDFSClient.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted.Wait();
            var created = WebHDFSClient.CreateDirectory("/UNIT_TEST_CREATE_FILE");
            created.Wait();
            var createdFile = WebHDFSClient.CreateFile("./basicInput.txt", "/UNIT_TEST_CREATE_FILE/basicInput.txt");
            createdFile.Wait();
            return createdFile.Result;
        }

    }
}
