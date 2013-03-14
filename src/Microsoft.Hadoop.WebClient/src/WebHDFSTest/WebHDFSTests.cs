// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0   
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT.  
//
// See the Apache Version 2.0 License for specific language governing 
// permissions and limitations under the License. 

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Hadoop.WebHDFS;

namespace WebClientTests
{
    // note, unit tests assume that you have a local cluster running
    // and that you can get to it without any auth
    // once we resolve providing the location, we'll be in a better place. 
    [TestClass]
    public class WebHDFSTests
    {
        WebHDFSClient client;

        [TestInitialize]
        public void SetupTests()
        {
            // switch localhost to localhost. if want fiddler support
            client = new WebHDFSClient(new Uri(@"http://localhost:50070"), null);
        }

        [TestMethod]
        public void GetHomeDirectory()
        {
            var path = client.GetHomeDirectory();
            path.Wait();
            Assert.AreEqual("/user/webuser", path.Result, "Home directory paths are the same");
        }

        [TestMethod]
        public void RenameDirectory()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_23");
            deleted.Wait();
            System.Threading.Tasks.Task<DirectoryListing> entries;
            System.Threading.Tasks.Task<bool> created;
            CreateDirectory(out entries, out created);
            var renameDir = client.RenameDirectory("/UNIT_TEST_CREATE", "/UNIT_TEST_CREATE_23");
            renameDir.Wait();
            var contentSummaryAfter = client.GetContentSummary("/UNIT_TEST_CREATE_23");
            contentSummaryAfter.Wait();
            Assert.AreEqual(1, contentSummaryAfter.Result.DirectoryCount, "Same Number of Directories");

            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_23");
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
            var entriesPost = client.GetDirectoryStatus("/");
            entriesPost.Wait();
            Assert.AreEqual(entriesPost.Result.Directories.Count(), entries.Result.Directories.Count() + 1, "Should see an additional directory created");
            DeleteDirectory();
    
        }

        private void DeleteDirectory()
        {
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE");
            deleted2.Wait();
        }

        [TestMethod]
        public void ValidateContentSummary()
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

        private  void CreateDirectory(out System.Threading.Tasks.Task<DirectoryListing> entries, out System.Threading.Tasks.Task<bool> created)
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE", true);
            deleted.Wait();
            entries = client.GetDirectoryStatus("/");
            entries.Wait();
            created = client.CreateDirectory("/UNIT_TEST_CREATE");
            created.Wait();
        }

        [TestMethod]
        public void PutFile()
        {
            var createdFile = CreateFile();
            Assert.IsTrue(createdFile.Equals("webhdfs://localhost:50070/UNIT_TEST_CREATE_FILE/basicInput.txt") , "file created");
            DeleteTextFile();
        }

        private  void DeleteTextFile()
        {
            var deleted2 = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted2.Wait();
        }


        [TestMethod]
        public void ValidateFile()
        {
            CreateFile();
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.IsTrue(fileStatus.Result.Length > 10, "file exists");
            DeleteTextFile();
        }

        [TestMethod]
        public void OpenPartOfAFile()
        {
            CreateFile();
            var fileStatus = client.OpenFile("/UNIT_TEST_CREATE_FILE/basicInput.txt",3,3);
            fileStatus.Wait();
            var readFile = fileStatus.Result.Content.ReadAsStringAsync();
            readFile.Wait();
            Assert.AreEqual("thr",readFile.Result, "correct offset & characters.");
            DeleteTextFile();
        }

        [TestMethod]
        public void ValidateChecksum()
        {
            CreateFile();
            var fileChecksum = client.GetFileChecksum("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileChecksum.Wait();
            Assert.AreEqual(fileChecksum.Result.Checksum, "0000020000000000000000002b5681a8c222c7cd2deff67f303b8fc800000000", "file exists");
            DeleteTextFile();
        }

        [TestMethod]
        public void SetReplicationFactor()
        {
            CreateFile();
            var fileChecksum = client.SetReplicationFactor("/UNIT_TEST_CREATE_FILE/basicInput.txt",1);
            fileChecksum.Wait();
            Assert.IsTrue(fileChecksum.Result);
            DeleteTextFile();
        }

        [TestMethod]
        public void SetOwner()
        {
            CreateFile();
            var response = client.SetOwner("/UNIT_TEST_CREATE_FILE/basicInput.txt", "hadoop");
            response.Wait();
            Assert.IsTrue(response.Result, "file changed"); 
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.AreEqual(fileStatus.Result.Owner,"hadoop");
            DeleteTextFile();
        }



        [TestMethod]
        public void SetGroup()
        {
            CreateFile();
            var response = client.SetGroup("/UNIT_TEST_CREATE_FILE/basicInput.txt", "admin");
            response.Wait();
            Assert.IsTrue(response.Result, "file changed");
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.AreEqual(fileStatus.Result.Group, "admin");
            DeleteTextFile();
        }

        [TestMethod]
        public void SetPermission()
        {
            CreateFile();
            var response = client.SetPermissions("/UNIT_TEST_CREATE_FILE/basicInput.txt", "333");
            response.Wait();
            Assert.IsTrue(response.Result, "file changed");
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.AreEqual(fileStatus.Result.Permission, "333");
            DeleteTextFile();
        }


        [TestMethod]
        public void SetAccessTime()
        {
            CreateFile();
            var response = client.SetAccessTime("/UNIT_TEST_CREATE_FILE/basicInput.txt", "23");
            response.Wait();
            Assert.IsTrue(response.Result, "file changed");
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.AreEqual(fileStatus.Result.AccessTime, "23");
            DeleteTextFile();
        }

        [TestMethod]
        public void SetModificationTime()
        {
            CreateFile();
            var response = client.SetModificationTime("/UNIT_TEST_CREATE_FILE/basicInput.txt", "23");
            response.Wait();
            Assert.IsTrue(response.Result, "file changed");
            var fileStatus = client.GetFileStatus("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileStatus.Wait();
            Assert.AreEqual(fileStatus.Result.ModificationTime, "23");
            DeleteTextFile();
        }

        [TestMethod]
        public void OpenAndReadFile()
        {
            CreateFile();
            var fileRead = client.OpenFile("/UNIT_TEST_CREATE_FILE/basicInput.txt");
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsStringAsync();
            content.Wait();
            Assert.AreEqual(System.IO.File.ReadAllText(@".\testfiles\basicInput.txt"), content.Result, "received right file");  
            DeleteTextFile();
        }

        private  string CreateFile()
        {
            var deleted = client.DeleteDirectory("/UNIT_TEST_CREATE_FILE", true);
            deleted.Wait();
            var created =  client.CreateDirectory("/UNIT_TEST_CREATE_FILE");
            created.Wait();
            var createdFile = client.CreateFile("./testfiles/basicInput.txt", "/UNIT_TEST_CREATE_FILE/basicInput.txt");
            createdFile.Wait();
            return createdFile.Result;
        }

    }
}
